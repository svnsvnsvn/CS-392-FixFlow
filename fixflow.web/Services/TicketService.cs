using fixflow.web.Data;
using fixflow.web.Domain.Enums;
using fixflow.web.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using System.Collections.Generic;

namespace fixflow.web.Services
{
    public class TicketService : ITicketService
    {
        private readonly FfDbContext _db;
        private readonly IMongoCollection<FfAccountNote> _notes;

        public TicketService(FfDbContext db, IMongoDatabase mongoDb)
        {
            _db = db;
            _notes = mongoDb.GetCollection<FfAccountNote>("AccountNotes");
        }

        public async Task<ServiceResult<Guid>> AddNewTicket(string _requestorId, RoleTypes _requestorRole, NewTicketDto _newTicketData)
        {
            try
            {
                // Validate requestor inputs
                bool validRequestor = await _db.FfUserProfiles.AnyAsync(u => u.FfUserId == _requestorId);

                if ((_requestorId == null) || (validRequestor == false))
                {
                    return ServiceResult<Guid>.Fail("Invalid requestor Id.");
                }

                if (!Enum.IsDefined(typeof(RoleTypes), _requestorRole))
                {
                    return ServiceResult<Guid>.Fail("Invalid role.");
                }

                if (_requestorRole == RoleTypes.Pending)
                {
                    return ServiceResult<Guid>.Fail("Insufficient privileges.");
                }

                // Validate new ticket info and create record
                var newTicket = new FfTicketRegister();
                newTicket.TicketId = Guid.NewGuid();

                var result = await GetNextShortCode();
                if ((result == null) || (!result.Success))
                {
                    return ServiceResult<Guid>.Fail(result?.Error ?? "Could not create new short code");
                }
                newTicket.TicketShortCode = result.Data;
                   

                // Store requestor who called this as creator of ticket
                newTicket.EnteredBy = _requestorId;

                // Store requestor as the id provided, if null use the submitter, of ID not found fail
                if (_newTicketData.RequestedBy == null)
                {
                    newTicket.RequestedBy = _requestorId;
                }
                else
                {
                    bool exists = await _db.FfUserProfiles.AnyAsync(u => u.FfUserId == _newTicketData.RequestedBy);
                    if (exists)
                    {
                        newTicket.RequestedBy = _newTicketData.RequestedBy;
                    }
                    else
                    {
                        return ServiceResult<Guid>.Fail("RequestedBy does not exist");
                    }
                }

                // Validate location and store code in ticket
                var validLocation = await _db.FfBuildingDirectorys.FindAsync(_newTicketData.Location);
                if (validLocation == null)
                {
                    return ServiceResult<Guid>.Fail("Location does not exist");
                }
                newTicket.Location = _newTicketData.Location;

                // Validate locations unit number
                if ((_newTicketData.Unit > validLocation.NumUnits) || (_newTicketData.Unit < 0))
                {
                    return ServiceResult<Guid>.Fail("Invalid unit number");
                }
                newTicket.Unit = _newTicketData.Unit;

                // Validate trouble ticket type
                bool validTicketType = await _db.FfTicketTypess.AnyAsync(u => u.Id == _newTicketData.TicketTroubleType);
                if (!validTicketType)
                {
                    return ServiceResult<Guid>.Fail("Invalid ticket type");
                }
                newTicket.TicketTroubleType = _newTicketData.TicketTroubleType;

                // Validate priority code (FfPriorityCodess.PriorityCode)
                bool validPriorityCode = await _db.FfPriorityCodess.AnyAsync(u => u.PriorityCode == _newTicketData.TicketPriority);
                if (!validPriorityCode)
                {
                    return ServiceResult<Guid>.Fail("Invalid priority code");
                }
                newTicket.TicketPriority = _newTicketData.TicketPriority;

                // Validate status code (FfStatusCodes.StatusCode)
                bool validStatusCode = await _db.FfStatusCodes.AnyAsync(u => u.StatusCode == _newTicketData.TicketStatus);
                if (!validStatusCode)
                {
                    return ServiceResult<Guid>.Fail("Invalid status code");
                }
                newTicket.TicketStatus = _newTicketData.TicketStatus;

                // Validate ticket subject
                if (_newTicketData.TicketSubject == null)
                {
                    return ServiceResult<Guid>.Fail("Missing ticket subject");
                }
                newTicket.TicketSubject = _newTicketData.TicketSubject;

                // Validate ticket description
                if (_newTicketData.TicketDescription == null)
                {
                    return ServiceResult<Guid>.Fail("Missing ticket description");
                }
                newTicket.TicketDescription = _newTicketData.TicketDescription;

                // Create initial TicketFlow entry for this ticket
                var newTicketFlow = new FfTicketFlow();

                newTicketFlow.TicketId = newTicket.TicketId;
                newTicketFlow.NewTicketStatus = newTicket.TicketStatus;
                newTicketFlow.NewAssignee = _requestorId;
                newTicketFlow.TimeStamp = DateTime.UtcNow;


                // Create transaction object and write ticket to DB's
                using var userCreationTransaction = await _db.Database.BeginTransactionAsync();
                try
                {
                    _db.FfTicketRegisters.Add(newTicket);
                    _db.FfTicketFlows.Add(newTicketFlow);

                    await _db.SaveChangesAsync();

                    await userCreationTransaction.CommitAsync();
                }
                catch
                {
                    await userCreationTransaction.RollbackAsync();          // Stop db writes if something failed. Prevent half transactions.
                    return ServiceResult<Guid>.Fail("Database write failed");
                }

                return ServiceResult<Guid>.Ok(newTicket.TicketId);
            }
            catch (Exception ex)
            {
                return ServiceResult<Guid>.Fail(ex.Message);
            }
        }
        public async Task<ServiceResult<bool>> UpdateTicket(string _requestorId, RoleTypes _requestorRole, TicketDataDto _updatedTicketData)
        {
            try
            {
                // Validate requestor inputs
                bool validRequestor = await _db.FfUserProfiles.AnyAsync(u => u.FfUserId == _requestorId);

                if ((_requestorId == null) || (validRequestor == false))
                {
                    return ServiceResult<bool>.Fail("Invalid requestor Id.");
                }

                // Validate ticket Id exists
                bool validTicket = await _db.FfTicketRegisters.AnyAsync(u => u.TicketId == _updatedTicketData.TicketId);
                if (!validTicket)
                {
                    return ServiceResult<bool>.Fail("Invalid TIcketId provided.");
                }

                if (!Enum.IsDefined(typeof(RoleTypes), _requestorRole))
                {
                    return ServiceResult<bool>.Fail("Invalid role.");
                }

                if (_requestorRole == RoleTypes.Pending)
                {
                    return ServiceResult<bool>.Fail("Insufficient privileges.");
                }

                var ticketToUpdate = await _db.FfTicketRegisters.FindAsync(_updatedTicketData.TicketId);
                if (ticketToUpdate == null)
                {
                    return ServiceResult<bool>.Fail("Ticket to update not found.");
                }

                ticketToUpdate.RequestedBy = _updatedTicketData.RequestedBy;
                ticketToUpdate.Location = _updatedTicketData.Location;
                ticketToUpdate.Unit = _updatedTicketData.Unit;
                ticketToUpdate.TicketTroubleType = _updatedTicketData.TicketTroubleType;
                ticketToUpdate.TicketPriority = _updatedTicketData.TicketPriority;
                ticketToUpdate.TicketSubject = _updatedTicketData.TicketSubject;
                ticketToUpdate.TicketDescription = _updatedTicketData.TicketDescription;

                await _db.SaveChangesAsync();

                return ServiceResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Fail(ex.Message);
            }
        }
        public async Task<ServiceResult<long>> ReassignTicket(string _requestorId, RoleTypes _requestorRole, Guid _ticketIdToUpdate, string _newAssigneeId, int _newStatus)
        {
            try
            {
                // Validate requestor inputs
                bool validRequestor = await _db.FfUserProfiles.AnyAsync(u => u.FfUserId == _requestorId);

                if ((_requestorId == null) || (validRequestor == false))
                {
                    return ServiceResult<long>.Fail("Invalid requestor Id.");
                }

                // Validate new asignee inputs
                bool validAssignee = await _db.FfUserProfiles.AnyAsync(u => u.FfUserId == _newAssigneeId);

                if ((_newAssigneeId == null) || (validAssignee == false))
                {
                    return ServiceResult<long>.Fail("Invalid assignee Id.");
                }

                // Validate ticket Id exists
                bool validTicket = await _db.FfTicketRegisters.AnyAsync(u => u.TicketId == _ticketIdToUpdate);
                if (!validTicket)
                {
                    return ServiceResult<long>.Fail("Invalid TicketId provided.");
                }

                // Validate new status code exists
                bool validStatus = await _db.FfStatusCodes.AnyAsync(u => u.StatusCode == _newStatus);
                if (!validStatus)
                {
                    return ServiceResult<long>.Fail("invalid status code provided.");
                }

                if (!Enum.IsDefined(typeof(RoleTypes), _requestorRole))
                {
                    return ServiceResult<long>.Fail("Invalid role.");
                }

                if (_requestorRole == RoleTypes.Pending)
                {
                    return ServiceResult<long>.Fail("Insufficient privileges.");
                }

                var ticketFlowUpdate = new FfTicketFlow()
                {
                    TicketId = _ticketIdToUpdate,
                    NewTicketStatus = _newStatus,
                    NewAssignee = _newAssigneeId,
                    TimeStamp = DateTime.UtcNow
                };

                try
                {
                    await _db.FfTicketFlows.AddAsync(ticketFlowUpdate);
                    await _db.SaveChangesAsync();
                    return ServiceResult<long>.Ok(ticketFlowUpdate.ActionId);
                }
                catch (Exception ex)
                {
                    return ServiceResult<long>.Fail("Database write failed.");
                }
            }
            catch (Exception ex)
            {
                return ServiceResult<long>.Fail(ex.Message);
            }
        }
        public async Task<ServiceResult<string>> GetNextShortCode()
        {
            try
            {
                var ticketSeriesData = await _db.FfTicketConstructoror
                    .FirstOrDefaultAsync(a => a.SeriesIsActive);
                if (ticketSeriesData == null)
                {
                    return ServiceResult<string>.Fail(
                        "No active ticket number series (FfTicketConstructoror). Restart the app after seeding, or ask an admin to add one.");
                }

                string ticketNum = (ticketSeriesData.LastTicketUsed + 1).ToString();
                int leadingZerosNeeded = 4 - ticketNum.Length;
                for (int i = 0; i < leadingZerosNeeded; i++)
                {
                    ticketNum = "0" + ticketNum;
                }

                string newShortCode = ticketSeriesData.TicketPrefix +
                    "-" + ticketSeriesData.TicketSeries.ToString() +
                    "-" + ticketNum;

                ticketSeriesData.LastTicketUsed++;

                await _db.SaveChangesAsync();

                return ServiceResult<string>.Ok(newShortCode);


            }
            catch (Exception ex)
            {
                return ServiceResult<string>.Fail(ex.Message);
            }
        }
        public async Task<ServiceResult<List<TicketTypeDto>>> GetTicketTypes()
        {
            try
            {
                var ticketTypes = await _db.FfTicketTypess
                    .OrderBy(a => a.Id)
                    .Select(a => new TicketTypeDto
                    {
                        Id = a.Id,
                        TypeName = a.TypeName
                    })
                    .ToListAsync();

                if (ticketTypes == null || ticketTypes.Count == 0)
                {
                    return ServiceResult<List<TicketTypeDto>>.Fail("No Ticket Types Defined");
                }
                
                return ServiceResult<List<TicketTypeDto>>.Ok(ticketTypes);

            }
            catch (Exception ex)
            {
                return ServiceResult<List<TicketTypeDto>>.Fail(ex.Message);
            }
        }
        public async Task<ServiceResult<List<BuildingDto>>> GetBuildings()
        {
            try
            {
                var buildingOptions = await _db.FfBuildingDirectorys
                    .Where(a => a.LocationName != "Unassigned")
                    .OrderBy(a => a.LocationCode)
                    .Select(a => new BuildingDto
                    {
                        LocationCode = a.LocationCode,
                        LocationName = a.LocationName,
                        ComplexName = a.ComplexName,
                        BuildingNumber = a.BuildingNumber,
                        NumUnits = a.NumUnits,
                        LocationLat = a.LocationLat,
                        LocationLon = a.LocationLon
                    })
                    .ToListAsync();

                if (buildingOptions == null || buildingOptions.Count == 0)
                {
                    return ServiceResult<List<BuildingDto>>.Fail("No Buildings Defined");
                }
                
                return ServiceResult<List<BuildingDto>>.Ok(buildingOptions);

            }
            catch (Exception ex)
            {
                return ServiceResult<List<BuildingDto>>.Fail(ex.Message);
            }
        }
        public async Task<ServiceResult<List<StatusCodeDto>>> GetStatusCodeList()
        {
            try
            {
                var statusCodeOptions = await _db.FfStatusCodes
                    .OrderBy(a => a.StatusCode)
                    .Select(a => new StatusCodeDto
                    {
                        StatusCode = a.StatusCode,
                        StatusName = a.StatusName
                    })
                    .ToListAsync();

                if (statusCodeOptions == null || statusCodeOptions.Count == 0)
                {
                    return ServiceResult<List<StatusCodeDto>>.Fail("No Status Codes Defined");
                }

                return ServiceResult<List<StatusCodeDto>>.Ok(statusCodeOptions);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<StatusCodeDto>>.Fail(ex.Message);
            }
        }
        public async Task<ServiceResult<int>> GetStatusCode(string _StatusName)
        {
            try
            {
                var result = await _db.FfStatusCodes.SingleAsync(a => a.StatusName == _StatusName);
                return ServiceResult<int>.Ok(result.StatusCode);
            }
            catch (Exception ex)
            {
                return ServiceResult<int>.Fail(ex.Message);
            }
        }
        public async Task<ServiceResult<string>> GetStatusCode(int _StatusCode)
        {
            try
            {
                var result = await _db.FfStatusCodes.SingleAsync(a => a.StatusCode == _StatusCode);
                return ServiceResult<string>.Ok(result.StatusName);
            }
            catch (Exception ex)
            {
                return ServiceResult<string>.Fail(ex.Message);
            }
        }
        public async Task<ServiceResult<StatusCodeDto>> GetStatusCodeFromId(int _Id)
        {
            try
            {
                var result = await _db.FfStatusCodes.SingleAsync(a => a.Id == _Id);
                StatusCodeDto returnCode = new StatusCodeDto();
                returnCode.Id = result.Id;
                returnCode.StatusCode = result.StatusCode;
                returnCode.StatusName = result.StatusName;

                return ServiceResult<StatusCodeDto>.Ok(returnCode);
            }
            catch (Exception ex)
            {
                return ServiceResult<StatusCodeDto>.Fail(ex.Message);
            }

        }
        public async Task<ServiceResult<int>> GetPriorityCode(string _PriorityName)
        {
            try
            {
                var result = await _db.FfPriorityCodess.SingleAsync(a => a.PriorityName == _PriorityName);
                return ServiceResult<int>.Ok(result.PriorityCode);
            }
            catch (Exception ex)
            {
                return ServiceResult<int>.Fail(ex.Message);
            }
        }
        public async Task<ServiceResult<string>> GetPriorityCode(int _PriorityCode)
        {
            try
            {
                var result = await _db.FfPriorityCodess.SingleAsync(a => a.PriorityCode == _PriorityCode);
                return ServiceResult<string>.Ok(result.PriorityName);
            }
            catch (Exception ex)
            {
                return ServiceResult<string>.Fail(ex.Message);
            }
        }
        public async Task<ServiceResult<PriorityCodeDto>> GetPriorityCodeFromId(int _Id)
        {
            try
            {
                var result = await _db.FfPriorityCodess.SingleAsync(a => a.Id == _Id);
                PriorityCodeDto returnCode = new PriorityCodeDto();
                returnCode.Id = result.Id;
                returnCode.PriorityCode = result.PriorityCode;
                returnCode.PriorityName = result.PriorityName;

                return ServiceResult<PriorityCodeDto>.Ok(returnCode);
            }
            catch (Exception ex)
            {
                return ServiceResult<PriorityCodeDto>.Fail(ex.Message);
            }
        }
        public async Task<ServiceResult<List<TicketDataDto>>> GetTicketsByRequestor(string _RequestorId)
        {
            try
            {
                // Verify _RequestorId is valid and exists exists
                if (_RequestorId == null)
                {
                    return ServiceResult<List<TicketDataDto>>.Fail("_RequestorId not provided");
                }

                bool requestorExists = await _db.FfUserProfiles.AnyAsync(a => a.FfUserId == _RequestorId);
                if (!requestorExists)
                {
                    return ServiceResult<List<TicketDataDto>>.Fail("_RequestorId not found");
                }

                var requestorTickets = await _db.FfTicketRegisters
                    .AsNoTracking()
                    .Where(a => a.RequestedBy == _RequestorId)
                    .ToListAsync();

                List<TicketDataDto> ticketsFound = new List<TicketDataDto>();

                foreach (var ticket in requestorTickets)
                {
                    ticketsFound.Add(new TicketDataDto
                    {
                        TicketId = ticket.TicketId,
                        RequestedBy = ticket.RequestedBy,
                        Location = ticket.Location,
                        Unit = ticket.Unit,
                        TicketPriority = ticket.TicketPriority,
                        TicketTroubleType = ticket.TicketTroubleType,
                        TicketSubject = ticket.TicketSubject,
                        TicketDescription = ticket.TicketDescription,
                    });
                }



                return ServiceResult<List<TicketDataDto>>.Ok(ticketsFound);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<TicketDataDto>>.Fail(ex.Message);
            }
        }
        public async Task<ServiceResult<List<TicketDataDto>>> GetTicketsByAssignee(string _AssigneeId)
        {
            try
            {
                // Verify _AssigneeId is valid and exists exists
                if (_AssigneeId == null)
                {
                    return ServiceResult<List<TicketDataDto>>.Fail("_AssigneeId not provided");
                }

                bool asigneeExists = await _db.FfUserProfiles.AnyAsync(a => a.FfUserId == _AssigneeId);
                if (!asigneeExists)
                {
                    return ServiceResult<List<TicketDataDto>>.Fail("_AsigneeId not found");
                }

                // This grabs all tickets ever assigned to the assignee
                //var asigneeTickets = await _db.FfTicketRegisters
                //    .AsNoTracking()
                //    .Where(b => _db.FfTicketFlows
                //        .Where(a => a.NewAssignee == _AssigneeId)
                //        .Select(a => a.TicketId)
                //        .Distinct()
                //        .Contains(b.TicketId))
                //    .ToListAsync();

                // Get latest ticket flow for each ticket
                var mostRecentFlowPerTicketWithAssignee = await _db.FfTicketFlows
                    .GroupBy(f => f.TicketId)
                    .Select(g => g.OrderByDescending(f => f.TimeStamp).First())
                    .Where(f => f.NewAssignee == _AssigneeId)
                    .Select(f => f.TicketId)
                    .ToListAsync();

                // Get tickets for the 
                var assigneeTickets = await _db.FfTicketRegisters
                    .Where(t => mostRecentFlowPerTicketWithAssignee.Contains(t.TicketId))
                    .ToListAsync();



                List<TicketDataDto> ticketsFound = new List<TicketDataDto>();

                foreach (var ticket in assigneeTickets)
                {
                    ticketsFound.Add(new TicketDataDto
                    {
                        TicketId = ticket.TicketId,
                        RequestedBy = ticket.RequestedBy,
                        Location = ticket.Location,
                        Unit = ticket.Unit,
                        TicketPriority = ticket.TicketPriority,
                        TicketTroubleType = ticket.TicketTroubleType,
                        TicketSubject = ticket.TicketSubject,
                        TicketDescription = ticket.TicketDescription,
                    });
                }
                
                return ServiceResult<List<TicketDataDto>>.Ok(ticketsFound);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<TicketDataDto>>.Fail(ex.Message);
            }
        }
        public async Task<ServiceResult<List<TicketHistoryItemDto>>> GetTicketHistory(Guid _TicketId)
        {
            try
            {
                // Verify _TicketId is valid and exists exists
                if (_TicketId == Guid.Empty)
                {
                    return ServiceResult<List<TicketHistoryItemDto>>.Fail("_TicketId not provided");
                }

                bool ticketExists = await _db.FfTicketRegisters.AnyAsync(a => a.TicketId == _TicketId);
                if (!ticketExists)
                {
                    return ServiceResult<List<TicketHistoryItemDto>>.Fail("_TicketId not found");
                }

                var flowsFound = await _db.FfTicketFlows
                    .AsNoTracking()
                    .Where(x => x.TicketId == _TicketId)
                    .ToListAsync();

                List<TicketHistoryItemDto> historyFound = new List<TicketHistoryItemDto>();

                foreach (var ticket in flowsFound)
                {
                    historyFound.Add(new TicketHistoryItemDto
                    {
                        TicketStatus = ticket.NewTicketStatus,
                        Assignee = ticket.NewAssignee,
                        TimeStamp = ticket.TimeStamp
                    });
                }

                historyFound = historyFound
                    .OrderByDescending(x => x.TimeStamp)
                    .ToList();

                return ServiceResult<List<TicketHistoryItemDto>>.Ok(historyFound);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<TicketHistoryItemDto>>.Fail(ex.Message);
            }
        }
        public async Task<ServiceResult<FfTicketRegister>> GetTicketById(Guid ticketId)
        {
            try
            {
                if (ticketId == Guid.Empty)
                    return ServiceResult<FfTicketRegister>.Fail("Ticket id not provided.");

                var ticket = await _db.FfTicketRegisters
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.TicketId == ticketId);
                if (ticket == null)
                    return ServiceResult<FfTicketRegister>.Fail("Ticket not found.");
                return ServiceResult<FfTicketRegister>.Ok(ticket);
            }
            catch (Exception ex)
            {
                return ServiceResult<FfTicketRegister>.Fail(ex.Message);
            }
        }
        public async Task<ServiceResult<FfTicketRegister>> GetTicketByIdentifier(string ticketIdOrCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ticketIdOrCode))
                    return ServiceResult<FfTicketRegister>.Fail("Ticket id not provided.");

                FfTicketRegister? ticket;
                if (Guid.TryParse(ticketIdOrCode, out var ticketId))
                {
                    ticket = await _db.FfTicketRegisters
                        .Include(t => t.TicketType)
                        .Include(t => t.PriorityCode)
                        .Include(t => t.StatusCode)
                        .Include(t => t.Building)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(t => t.TicketId == ticketId);
                }
                else
                {
                    ticket = await _db.FfTicketRegisters
                        .Include(t => t.TicketType)
                        .Include(t => t.PriorityCode)
                        .Include(t => t.StatusCode)
                        .Include(t => t.Building)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(t => t.TicketShortCode == ticketIdOrCode);
                }

                if (ticket == null)
                    return ServiceResult<FfTicketRegister>.Fail("Ticket not found.");
                return ServiceResult<FfTicketRegister>.Ok(ticket);
            }
            catch (Exception ex)
            {
                return ServiceResult<FfTicketRegister>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<List<FfTicketFlow>>> GetTicketFlows(Guid ticketId)
        {
            try
            {
                var flows = await _db.FfTicketFlows
                    .Where(flow => flow.TicketId == ticketId)
                    .OrderBy(flow => flow.TimeStamp)
                    .AsNoTracking()
                    .ToListAsync();
                return ServiceResult<List<FfTicketFlow>>.Ok(flows);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<FfTicketFlow>>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<Dictionary<int, string>>> GetStatusCodeNameMap()
        {
            try
            {
                var statusCodes = await _db.FfStatusCodes
                    .AsNoTracking()
                    .ToDictionaryAsync(code => code.Id, code => code.StatusName);
                return ServiceResult<Dictionary<int, string>>.Ok(statusCodes);
            }
            catch (Exception ex)
            {
                return ServiceResult<Dictionary<int, string>>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<List<FfExternalNotes>>> GetExternalNotes(Guid ticketId)
        {
            try
            {
                var notes = await _db.FfExternalNotess
                    .Where(note => note.TicketId == ticketId)
                    .OrderByDescending(note => note.TimeStamp)
                    .AsNoTracking()
                    .ToListAsync();
                return ServiceResult<List<FfExternalNotes>>.Ok(notes);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<FfExternalNotes>>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<List<FfInternalNotes>>> GetInternalNotes(Guid ticketId)
        {
            try
            {
                var notes = await _db.FfInternalNotess
                    .Where(note => note.TicketId == ticketId)
                    .OrderByDescending(note => note.TimeStamp)
                    .AsNoTracking()
                    .ToListAsync();
                return ServiceResult<List<FfInternalNotes>>.Ok(notes);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<FfInternalNotes>>.Fail(ex.Message);
            }
        }
        public async Task<ServiceResult<TicketQueryBundleDto>> GetTicketListBundle()
        {
            try
            {
                var tickets = await _db.FfTicketRegisters
                    .Include(t => t.TicketType)
                    .Include(t => t.PriorityCode)
                    .Include(t => t.StatusCode)
                    .Include(t => t.Building)
                    .Include(t => t.RequestedByUser)
                    .AsNoTracking()
                    .ToListAsync();

                var ticketIds = tickets.Select(t => t.TicketId).ToList();
                var flows = ticketIds.Count == 0
                    ? new List<FfTicketFlow>()
                    : await _db.FfTicketFlows
                        .Where(f => ticketIds.Contains(f.TicketId))
                        .AsNoTracking()
                        .ToListAsync();

                var latestFlows = flows
                    .GroupBy(f => f.TicketId)
                    .Select(g => g.OrderByDescending(f => f.TimeStamp).First())
                    .ToList();

                var assigneeIds = latestFlows
                    .Where(f => !string.IsNullOrWhiteSpace(f.NewAssignee))
                    .Select(f => f.NewAssignee)
                    .Distinct()
                    .ToList();

                var profileDisplayNames = assigneeIds.Count == 0
                    ? new Dictionary<string, string>()
                    : await _db.FfUserProfiles
                        .Where(p => assigneeIds.Contains(p.FfUserId))
                        .ToDictionaryAsync(p => p.FfUserId, p => $"{p.FName} {p.LName}".Trim());

                return ServiceResult<TicketQueryBundleDto>.Ok(new TicketQueryBundleDto
                {
                    Tickets = tickets,
                    Flows = flows,
                    ProfileDisplayNames = profileDisplayNames
                });
            }
            catch (Exception ex)
            {
                return ServiceResult<TicketQueryBundleDto>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<TicketQueryBundleDto>> GetDashboardBundle(string requestorId, RoleTypes requestorRole)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(requestorId))
                    return ServiceResult<TicketQueryBundleDto>.Fail("Requestor Id not provided.");

                var ticketsQuery = _db.FfTicketRegisters
                    .Include(ticket => ticket.TicketType)
                    .Include(ticket => ticket.PriorityCode)
                    .Include(ticket => ticket.StatusCode)
                    .Include(ticket => ticket.Building)
                    .Include(ticket => ticket.RequestedByUser)
                    .AsNoTracking();

                if (requestorRole == RoleTypes.Resident || requestorRole == RoleTypes.Pending)
                    ticketsQuery = ticketsQuery.Where(ticket => ticket.RequestedBy == requestorId);
                else if (requestorRole == RoleTypes.Employee)
                    ticketsQuery = ticketsQuery.Where(ticket => ticket.EnteredBy == requestorId);

                var tickets = await ticketsQuery.ToListAsync();
                var ticketIds = tickets.Select(ticket => ticket.TicketId).ToList();
                var flows = ticketIds.Count == 0
                    ? new List<FfTicketFlow>()
                    : await _db.FfTicketFlows
                        .Where(flow => ticketIds.Contains(flow.TicketId))
                        .AsNoTracking()
                        .ToListAsync();

                var statusCodeNames = await _db.FfStatusCodes.AsNoTracking()
                    .ToDictionaryAsync(code => code.Id, code => code.StatusName);

                var profileIds = tickets.Select(t => t.RequestedBy)
                    .Concat(flows.Select(f => f.NewAssignee))
                    .Where(id => !string.IsNullOrWhiteSpace(id))
                    .Distinct()
                    .ToList();

                var profileDisplayNames = profileIds.Count == 0
                    ? new Dictionary<string, string>()
                    : await _db.FfUserProfiles.AsNoTracking()
                        .Where(profile => profileIds.Contains(profile.FfUserId))
                        .ToDictionaryAsync(profile => profile.FfUserId, profile => $"{profile.FName} {profile.LName}".Trim());

                return ServiceResult<TicketQueryBundleDto>.Ok(new TicketQueryBundleDto
                {
                    Tickets = tickets,
                    Flows = flows,
                    ProfileDisplayNames = profileDisplayNames,
                    StatusCodeNames = statusCodeNames
                });
            }
            catch (Exception ex)
            {
                return ServiceResult<TicketQueryBundleDto>.Fail(ex.Message);
            }
        }
        public async Task<ServiceResult<bool>> AddNewNote(UserCredentialDTO _SubmitterId, NoteDto _NewNote)
        {
            try
            {
                // Verify _RequestorId is valid and exists exists
                if (_SubmitterId.UserId == null)
                {
                    return ServiceResult<bool>.Fail("_SubmitterId not provided");
                }

                bool submitterExists = await _db.FfUserProfiles.AnyAsync(a => a.FfUserId == _SubmitterId.UserId);
                if (!submitterExists)
                {
                    return ServiceResult<bool>.Fail("_SubmitterId not found");
                }

                // Validate new note contents
                bool ticketExists = await _db.FfTicketRegisters.AnyAsync(a => a.TicketId == _NewNote.TicketId);
                if (!ticketExists)
                {
                    return ServiceResult<bool>.Fail("TicketId related to _newNote not found");
                }

                if (_NewNote.NoteText.Length < 1)
                {
                    return ServiceResult<bool>.Fail("NoteText not valid.");
                }

                FfAccountNote newNote = new FfAccountNote();

                newNote.TicketId = _NewNote.TicketId;
                newNote.NoteText = _NewNote.NoteText;
                newNote.InternalOnly = _NewNote.InternalOnly;
                newNote.TimeStamp = DateTime.UtcNow;
                newNote.EnteredByUserId = _SubmitterId.UserId;

                // Create new note
                await _notes.InsertOneAsync(newNote);
     
                return ServiceResult<bool>.Ok(true);
                 }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<List<NoteDto>>> GetAllNote(UserCredentialDTO _SubmitterId, Guid _TicketId)
        {

        }

    }
}