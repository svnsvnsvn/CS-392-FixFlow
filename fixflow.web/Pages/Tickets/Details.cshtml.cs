using fixflow.web.Data;
using fixflow.web.Domain.Constants;
using fixflow.web.Domain.Enums;
using fixflow.web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace fixflow.web.Pages.Tickets
{
    public class DetailsModel : PageModel
    {
        private readonly FfDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly ITicketService _ticketService;

        public DetailsModel(FfDbContext context, UserManager<AppUser> userManager, ITicketService ticketService)
        {
            _context = context;
            _userManager = userManager;
            _ticketService = ticketService;
        }

        public string UserRole { get; set; } = "Client";
        public bool IsOwnTicket { get; set; } = false;
        public TicketDetailViewModel Ticket { get; set; } = new();
        public List<CommentViewModel> PublicComments { get; set; } = new();
        public List<CommentViewModel> InternalNotes { get; set; } = new();
        public List<ActivityViewModel> ActivityHistory { get; set; } = new();
        public List<SelectListItem> AvailableTechnicians { get; set; } = new();

        [BindProperty]
        public string SelectedTechnicianId { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            // Determine user role
            if (User.IsInRole(RoleNames.Admin))
                UserRole = "Admin";
            else if (User.IsInRole(RoleNames.Manager))
                UserRole = "Manager";
            else if (User.IsInRole(RoleNames.Employee))
                UserRole = "Technician";
            else
                UserRole = "Client";

            // Try to parse as GUID first
            FfTicketRegister? ticket = null;
            
            if (Guid.TryParse(id, out var ticketId))
            {
                ticket = await _context.FfTicketRegisters
                    .Include(t => t.TicketType)
                    .Include(t => t.PriorityCode)
                    .Include(t => t.StatusCode)
                    .Include(t => t.Building)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.TicketId == ticketId);
            }
            else
            {
                // Try to find by short code
                ticket = await _context.FfTicketRegisters
                    .Include(t => t.TicketType)
                    .Include(t => t.PriorityCode)
                    .Include(t => t.StatusCode)
                    .Include(t => t.Building)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.TicketShortCode == id);
            }

            if (ticket == null)
            {
                return NotFound();
            }

            var profiles = await _context.FfUserProfiles
                .AsNoTracking()
                .ToDictionaryAsync(profile => profile.FfUserId, profile => profile);

            var flows = await _context.FfTicketFlows
                .Where(flow => flow.TicketId == ticket.TicketId)
                .OrderBy(flow => flow.TimeStamp)
                .AsNoTracking()
                .ToListAsync();

            var statusCodes = await _context.FfStatusCodes
                .AsNoTracking()
                .ToDictionaryAsync(code => code.Code, code => code.StatusName);

            var createdDate = flows.Select(flow => flow.TimeStamp).FirstOrDefault();
            var lastFlow = flows.LastOrDefault();

            var submittedByName = profiles.TryGetValue(ticket.RequestedBy, out var requester)
                ? $"{requester.FName} {requester.LName}".Trim()
                : ticket.RequestedBy;

            var assigneeName = lastFlow != null && profiles.TryGetValue(lastFlow.NewAssignee, out var assignee)
                ? $"{assignee.FName} {assignee.LName}".Trim()
                : null;

            Ticket = new TicketDetailViewModel
            {
                Id = string.IsNullOrWhiteSpace(ticket.TicketShortCode)
                    ? ticket.TicketId.ToString()
                    : ticket.TicketShortCode,
                Title = ticket.TicketType?.TypeName ?? "Maintenance request",
                Description = "Details will appear once the request is fully documented.",
                Status = ticket.StatusCode?.StatusName ?? TicketStatusNames.Submitted,
                Priority = ticket.PriorityCode?.PriorityName ?? "Normal",
                Category = ticket.TicketType?.TypeName ?? "General",
                Building = ticket.Building?.LocationName ?? "Unknown building",
                RoomNumber = ticket.Unit > 0 ? ticket.Unit.ToString() : "-",
                SubmittedBy = string.IsNullOrWhiteSpace(submittedByName) ? "Unknown" : submittedByName,
                CreatedDate = createdDate == default ? DateTime.MinValue : createdDate,
                AssignedTo = assigneeName,
                DueDate = null,
                CompletedDate = TicketStatusIsCompleted(ticket.StatusCode?.StatusName)
                    ? (lastFlow?.TimeStamp ?? DateTime.UtcNow)
                    : null
            };

            PublicComments = await _context.FfExternalNotes
                .Where(note => note.TicketId == ticket.TicketId)
                .OrderByDescending(note => note.TimeStamp)
                .AsNoTracking()
                .Select(note => new CommentViewModel
                {
                    AuthorName = note.CreatedBy,
                    AuthorRole = string.Empty,
                    Text = note.Content,
                    CreatedDate = note.TimeStamp,
                    IsInternal = false
                })
                .ToListAsync();

            InternalNotes = await _context.FfInternalNotes
                .Where(note => note.TicketId == ticket.TicketId)
                .OrderByDescending(note => note.TimeStamp)
                .AsNoTracking()
                .Select(note => new CommentViewModel
                {
                    AuthorName = note.CreatedBy,
                    AuthorRole = "Staff",
                    Text = note.Content,
                    CreatedDate = note.TimeStamp,
                    IsInternal = true
                })
                .ToListAsync();

            ActivityHistory = flows
                .OrderByDescending(flow => flow.TimeStamp)
                .Select(flow => new ActivityViewModel
                {
                    Action = statusCodes.TryGetValue(flow.NewTicketStatus, out var statusName)
                        ? $"Status set to '{statusName}'"
                        : "Ticket updated",
                    PerformedBy = profiles.TryGetValue(flow.NewAssignee, out var actor)
                        ? $"{actor.FName} {actor.LName}".Trim()
                        : "System",
                    Timestamp = flow.TimeStamp
                })
                .ToList();

            // Check if current user owns this ticket (for client view)
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            IsOwnTicket = (UserRole == "Client" && !string.IsNullOrEmpty(currentUserId) && ticket.RequestedBy == currentUserId);

            // Load available technicians for assignment (only for managers)
            if (UserRole == "Manager" || UserRole == "Admin")
            {
                var technicians = await _userManager.GetUsersInRoleAsync(RoleNames.Employee);
                var techniciansWithProfiles = await _context.FfUserProfiles
                    .Where(p => technicians.Select(t => t.Id).Contains(p.FfUserId))
                    .ToListAsync();

                AvailableTechnicians = techniciansWithProfiles
                    .Select(p => new SelectListItem
                    {
                        Value = p.FfUserId,
                        Text = $"{p.FName} {p.LName}".Trim()
                    })
                    .OrderBy(t => t.Text)
                    .ToList();

                AvailableTechnicians.Insert(0, new SelectListItem { Value = "", Text = "-- Select Technician --" });
            }

            return Page();
        }

        private static bool TicketStatusIsCompleted(string? statusName)
        {
            return string.Equals(statusName, TicketStatusNames.Completed, StringComparison.OrdinalIgnoreCase);
        }

        public async Task<IActionResult> OnPostAddCommentAsync(string ticketId, string commentText)
        {
            if (string.IsNullOrWhiteSpace(commentText))
            {
                return RedirectToPage(new { id = ticketId });
            }

            // Backend will add comment to database:
            // var comment = new TicketComment
            // {
            //     TicketId = ticketId,
            //     UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            //     CommentText = commentText,
            //     IsInternalNote = false,
            //     CreatedDate = DateTime.UtcNow
            // };
            // await _context.TicketComments.AddAsync(comment);
            // await _context.SaveChangesAsync();

            // Also add to activity history:
            // var activity = new TicketHistory
            // {
            //     TicketId = ticketId,
            //     ChangedById = User.FindFirstValue(ClaimTypes.NameIdentifier),
            //     ChangeType = "Comment Added",
            //     Notes = commentText.Substring(0, Math.Min(100, commentText.Length)),
            //     ChangedDate = DateTime.UtcNow
            // };

            return RedirectToPage(new { id = ticketId });
        }

        public async Task<IActionResult> OnPostAddInternalNoteAsync(string ticketId, string noteText)
        {
            if (string.IsNullOrWhiteSpace(noteText))
            {
                return RedirectToPage(new { id = ticketId });
            }

            // Backend will add internal note to database:
            // var note = new TicketComment
            // {
            //     TicketId = ticketId,
            //     UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            //     CommentText = noteText,
            //     IsInternalNote = true,  // This is the key difference!
            //     CreatedDate = DateTime.UtcNow
            // };
            // await _context.TicketComments.AddAsync(note);
            // await _context.SaveChangesAsync();

            return RedirectToPage(new { id = ticketId });
        }

        public async Task<IActionResult> OnPostAssignTechnicianAsync(string ticketId)
        {
            if (string.IsNullOrWhiteSpace(SelectedTechnicianId))
            {
                return RedirectToPage(new { id = ticketId });
            }

            // Parse ticket ID
            FfTicketRegister? ticket = null;
            if (Guid.TryParse(ticketId, out var ticketGuid))
            {
                ticket = await _context.FfTicketRegisters.FirstOrDefaultAsync(t => t.TicketId == ticketGuid);
            }
            else
            {
                ticket = await _context.FfTicketRegisters.FirstOrDefaultAsync(t => t.TicketShortCode == ticketId);
            }

            if (ticket == null)
            {
                return NotFound();
            }

            // Get current user
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToPage("/Account/Login");
            }

            // Determine user role
            RoleTypes userRole = RoleTypes.Manager;
            if (User.IsInRole(RoleNames.Admin))
                userRole = RoleTypes.Admin;

            // Get "Assigned" status code
            var assignedStatus = await _context.FfStatusCodes
                .FirstOrDefaultAsync(s => s.StatusName == TicketStatusNames.Assigned);

            if (assignedStatus == null)
            {
                TempData["ErrorMessage"] = "System configuration error: Assigned status not found.";
                return RedirectToPage(new { id = ticketId });
            }

            // Use the real backend method: ReassignTicket
            var result = await _ticketService.ReassignTicket(
                currentUser.Id,
                userRole,
                ticket.TicketId,
                SelectedTechnicianId,
                assignedStatus.Code
            );

            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Error ?? "Ticket assignment failed.";
            }
            else
            {
                TempData["SuccessMessage"] = "Ticket assigned successfully!";
            }

            return RedirectToPage(new { id = ticketId });
        }

        // ViewModels
        public class TicketDetailViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Building { get; set; } = string.Empty;
        public string RoomNumber { get; set; } = string.Empty;
        public string SubmittedBy { get; set; } = string.Empty;
        public string? AssignedTo { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }
    }

    public class CommentViewModel
    {
        public string AuthorName { get; set; } = string.Empty;
        public string AuthorRole { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public bool IsInternal { get; set; }
    }

    public class ActivityViewModel
    {
        public string Action { get; set; } = string.Empty;
        public string PerformedBy { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}
