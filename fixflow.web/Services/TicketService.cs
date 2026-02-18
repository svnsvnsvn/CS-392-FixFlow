using Microsoft.EntityFrameworkCore;
using fixflow.web.Data;
using fixflow.web.Domain.Constants;
using fixflow.web.Dto;

namespace fixflow.web.Services
{
    public class TicketService : ITicketService
    {
        private const string TicketShortCodePrefix = "T-";
        private const int TicketShortCodeSeed = 1000;

        private readonly FfDbContext _db;

        public TicketService(FfDbContext db)
        {
            _db = db;
        }

        public async Task<List<FfTicketRegister>> GetTicketsForListAsync()
        {
            return await _db.FfTicketRegisters
                .AsNoTracking()
                .Include(ticket => ticket.TicketType)
                .Include(ticket => ticket.PriorityCode)
                .Include(ticket => ticket.StatusCode)
                .Include(ticket => ticket.Building)
                .Include(ticket => ticket.RequestedByUser)
                .Include(ticket => ticket.EnteredByUser)
                .ToListAsync();
        }

        public async Task<Dictionary<Guid, string>> GetLatestAssigneesAsync(IReadOnlyCollection<Guid> ticketIds)
        {
            if (ticketIds.Count == 0)
            {
                return new Dictionary<Guid, string>();
            }

            var latestFlows = await _db.FfTicketFlows
                .Where(flow => ticketIds.Contains(flow.TicketId))
                .GroupBy(flow => flow.TicketId)
                .Select(group => group.OrderByDescending(flow => flow.TimeStamp).FirstOrDefault())
                .ToListAsync();

            var assigneeIds = latestFlows
                .Where(flow => flow != null && !string.IsNullOrWhiteSpace(flow.NewAssignee))
                .Select(flow => flow!.NewAssignee)
                .Distinct()
                .ToList();

            var profiles = await _db.FfUserProfiles
                .Where(profile => assigneeIds.Contains(profile.FfUserId))
                .ToDictionaryAsync(profile => profile.FfUserId, profile => $"{profile.FName} {profile.LName}".Trim());

            var result = new Dictionary<Guid, string>();
            foreach (var flow in latestFlows.Where(flow => flow != null))
            {
                if (profiles.TryGetValue(flow!.NewAssignee, out var name))
                {
                    result[flow.TicketId] = name;
                }
            }

            return result;
        }

        public async Task<ServiceResult<TicketCreateResult>> CreateTicketAsync(string userId, bool isStaff, TicketCreateRequest request)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return ServiceResult<TicketCreateResult>.Fail("Invalid user.");
            }

            var submittedStatus = await _db.FfStatusCodes
                .AsNoTracking()
                .FirstOrDefaultAsync(status => status.StatusName == TicketStatusNames.Submitted);

            if (submittedStatus == null)
            {
                return ServiceResult<TicketCreateResult>.Fail("System error: Status codes not configured.");
            }

            var shortCode = await GenerateNextTicketShortCodeAsync();

            var ticket = new FfTicketRegister
            {
                TicketId = Guid.NewGuid(),
                TicketShortCode = shortCode,
                EnteredBy = userId,
                RequestedBy = isStaff && !string.IsNullOrWhiteSpace(request.ResidentId) ? request.ResidentId : userId,
                Location = request.LocationCode,
                Unit = request.Unit,
                TicketTroubleType = request.TicketTypeCode,
                TicketStatus = submittedStatus.Code,
                TicketPriority = 2,
                DueDate = request.DueDate
            };

            _db.FfTicketRegisters.Add(ticket);

            if (!string.IsNullOrWhiteSpace(request.Description))
            {
                var note = new FfExternalNotes
                {
                    TicketId = ticket.TicketId,
                    CreatedBy = userId,
                    TimeStamp = DateTime.UtcNow,
                    Content = request.Description
                };
                _db.FfExternalNotes.Add(note);
            }

            await _db.SaveChangesAsync();

            return ServiceResult<TicketCreateResult>.Ok(new TicketCreateResult
            {
                TicketId = ticket.TicketId,
                TicketShortCode = shortCode
            });
        }

        public async Task<ServiceResult<bool>> AssignTicketAsync(Guid ticketId, string assigneeId)
        {
            var ticket = await _db.FfTicketRegisters.FindAsync(ticketId);
            if (ticket == null)
            {
                return ServiceResult<bool>.Fail("Ticket not found.");
            }

            if (string.IsNullOrWhiteSpace(assigneeId))
            {
                return ServiceResult<bool>.Fail("Invalid assignee.");
            }

            var assignedStatus = await _db.FfStatusCodes
                .FirstOrDefaultAsync(status => status.StatusName == TicketStatusNames.Assigned);

            if (assignedStatus == null)
            {
                return ServiceResult<bool>.Fail("System error: Status codes not configured.");
            }

            ticket.TicketStatus = assignedStatus.Code;

            var flowEntry = new FfTicketFlow
            {
                TicketId = ticket.TicketId,
                TimeStamp = DateTime.UtcNow,
                NewAssignee = assigneeId,
                NewTicketStatus = assignedStatus.Code
            };

            _db.FfTicketFlows.Add(flowEntry);
            await _db.SaveChangesAsync();

            return ServiceResult<bool>.Ok(true);
        }

        private async Task<string> GenerateNextTicketShortCodeAsync()
        {
            var shortCodes = await _db.FfTicketRegisters
                .AsNoTracking()
                .Select(ticket => ticket.TicketShortCode)
                .ToListAsync();

            var maxNumber = TicketShortCodeSeed;
            foreach (var code in shortCodes)
            {
                if (string.IsNullOrWhiteSpace(code) || !code.StartsWith(TicketShortCodePrefix))
                {
                    continue;
                }

                if (int.TryParse(code.Substring(TicketShortCodePrefix.Length), out var parsed))
                {
                    if (parsed > maxNumber)
                    {
                        maxNumber = parsed;
                    }
                }
            }

            var nextNumber = maxNumber + 1;
            var shortCode = $"{TicketShortCodePrefix}{nextNumber}";

            while (await _db.FfTicketRegisters.AnyAsync(ticket => ticket.TicketShortCode == shortCode))
            {
                nextNumber += 1;
                shortCode = $"{TicketShortCodePrefix}{nextNumber}";
            }

            return shortCode;
        }
    }
}
