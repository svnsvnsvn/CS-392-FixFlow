using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace fixflow.web.Pages.Tickets
{
    public class DetailsModel : PageModel
    {
        public string UserRole { get; set; } = "Client";
        public bool IsOwnTicket { get; set; } = false;
        public TicketDetailViewModel Ticket { get; set; } = new();
        public List<CommentViewModel> PublicComments { get; set; } = new();
        public List<CommentViewModel> InternalNotes { get; set; } = new();
        public List<ActivityViewModel> ActivityHistory { get; set; } = new();

        public IActionResult OnGet(string id, string? role = null)
        {
            // FOR DEMO: Allow testing different roles via query string
            // In production: UserRole = GetUserRole();
            UserRole = role ?? "Client";

            // Load ticket details
            LoadTicketData(id);

            // Check if current user owns this ticket (for client view)
            IsOwnTicket = (UserRole == "Client" && Ticket.SubmittedBy == "Demo User");

            return Page();
        }

        private void LoadTicketData(string ticketId)
        {
            // MOCK DATA - Backend will replace with real database query
            Ticket = new TicketDetailViewModel
            {
                Id = ticketId,
                Title = "Leaking faucet in kitchen - urgent repair needed",
                Description = "The faucet in the kitchen has been leaking steadily for the past two days. Water is dripping from the base and creating a small puddle. The leak seems to be getting worse over time. This needs immediate attention as it's wasting water and could potentially cause water damage to the cabinet below.",
                Status = "In Progress",
                Priority = "High",
                Category = "Plumbing",
                Building = "Building A",
                RoomNumber = "205",
                SubmittedBy = "john.doe",
                CreatedDate = DateTime.Now.AddDays(-3),
                AssignedTo = "Mike Johnson (Technician)",
                DueDate = DateTime.Now.AddDays(1),
                CompletedDate = null
            };

            // Public Comments (visible to everyone)
            PublicComments = new List<CommentViewModel>
            {
                new CommentViewModel
                {
                    AuthorName = "john.doe",
                    AuthorRole = "Client",
                    Text = "I submitted this ticket because the leak is getting worse. Water is now pooling on the counter.",
                    CreatedDate = DateTime.Now.AddDays(-3),
                    IsInternal = false
                },
                new CommentViewModel
                {
                    AuthorName = "Sarah Manager",
                    AuthorRole = "Manager",
                    Text = "Thank you for reporting this. I've assigned this to our plumbing technician. He will be there tomorrow morning.",
                    CreatedDate = DateTime.Now.AddDays(-2),
                    IsInternal = false
                },
                new CommentViewModel
                {
                    AuthorName = "Mike Johnson",
                    AuthorRole = "Technician",
                    Text = "I've started working on this. The washer needs to be replaced. I'll have it fixed by end of day.",
                    CreatedDate = DateTime.Now.AddHours(-2),
                    IsInternal = false
                }
            };

            // Internal Notes (only visible to staff)
            InternalNotes = new List<CommentViewModel>
            {
                new CommentViewModel
                {
                    AuthorName = "Sarah Manager",
                    AuthorRole = "Manager",
                    Text = "This is the third leak in Building A this month. We may need to schedule a plumbing inspection for the entire building.",
                    CreatedDate = DateTime.Now.AddDays(-2).AddHours(1),
                    IsInternal = true
                },
                new CommentViewModel
                {
                    AuthorName = "Mike Johnson",
                    AuthorRole = "Technician",
                    Text = "Parts ordered from supplier. ETA tomorrow 9 AM. Will need about 1 hour to complete the repair.",
                    CreatedDate = DateTime.Now.AddDays(-1),
                    IsInternal = true
                },
                new CommentViewModel
                {
                    AuthorName = "Mike Johnson",
                    AuthorRole = "Technician",
                    Text = "Parts arrived. Starting work now. The faucet base also has corrosion that may need addressing in the future.",
                    CreatedDate = DateTime.Now.AddHours(-2),
                    IsInternal = true
                }
            };

            // Activity History
            ActivityHistory = new List<ActivityViewModel>
            {
                new ActivityViewModel
                {
                    Action = "Ticket created",
                    PerformedBy = "john.doe",
                    Timestamp = DateTime.Now.AddDays(-3)
                },
                new ActivityViewModel
                {
                    Action = "Status changed to 'In Review'",
                    PerformedBy = "Sarah Manager",
                    Timestamp = DateTime.Now.AddDays(-2).AddHours(2)
                },
                new ActivityViewModel
                {
                    Action = "Priority set to 'High'",
                    PerformedBy = "Sarah Manager",
                    Timestamp = DateTime.Now.AddDays(-2).AddHours(2)
                },
                new ActivityViewModel
                {
                    Action = "Assigned to Mike Johnson",
                    PerformedBy = "Sarah Manager",
                    Timestamp = DateTime.Now.AddDays(-2).AddHours(3)
                },
                new ActivityViewModel
                {
                    Action = "Status changed to 'In Progress'",
                    PerformedBy = "Mike Johnson",
                    Timestamp = DateTime.Now.AddHours(-2)
                }
            };
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
