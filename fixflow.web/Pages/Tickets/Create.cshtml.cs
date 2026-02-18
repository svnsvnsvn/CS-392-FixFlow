using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using fixflow.web.Data;
using fixflow.web.Domain.Constants;
using fixflow.web.Dto;
using fixflow.web.Services;
using System.ComponentModel.DataAnnotations;

namespace fixflow.web.Pages.Tickets
{
    public class CreateModel : PageModel
    {
        private readonly FfDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly ITicketService _ticketService;

        public CreateModel(FfDbContext context, UserManager<AppUser> userManager, ITicketService ticketService)
        {
            _context = context;
            _userManager = userManager;
            _ticketService = ticketService;
        }

        public List<SelectListItem> Buildings { get; set; } = new();
        public List<SelectListItem> TicketTypes { get; set; } = new();
        public List<SelectListItem> Residents { get; set; } = new();
        public bool IsStaff { get; set; }

        [BindProperty]
        public TicketInput Input { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            // Check if user is staff (Manager, Technician, or Admin)
            IsStaff = User.IsInRole(RoleNames.Manager) || User.IsInRole(RoleNames.Employee) || User.IsInRole(RoleNames.Admin);
            
            await LoadDropdownData();
            
            // Pre-populate location and unit from user profile if available (for residents only)
            if (!IsStaff)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var userProfile = await _context.FfUserProfiles
                        .FirstOrDefaultAsync(p => p.FfUserId == user.Id);
                        
                    if (userProfile != null)
                    {
                        Input.LocationCode = userProfile.LocationCode;
                        Input.Unit = userProfile.Unit;
                    }
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            IsStaff = User.IsInRole(RoleNames.Manager) || User.IsInRole(RoleNames.Employee) || User.IsInRole(RoleNames.Admin);
            
            if (!ModelState.IsValid)
            {
                await LoadDropdownData();
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Account/Login");
            }

            var request = new TicketCreateRequest
            {
                LocationCode = Input.LocationCode,
                Unit = Input.Unit,
                TicketTypeCode = Input.TicketTypeCode,
                Description = Input.Description,
                ResidentId = Input.ResidentId,
                DueDate = Input.DueDate
            };

            var result = await _ticketService.CreateTicketAsync(user.Id, IsStaff, request);
            if (!result.Success || result.Data == null)
            {
                ModelState.AddModelError(string.Empty, result.Error ?? "Ticket could not be created.");
                await LoadDropdownData();
                return Page();
            }

            TempData["SuccessMessage"] = $"Ticket {result.Data.TicketShortCode} created successfully!";
            return RedirectToPage("./List");
        }

        private async Task LoadDropdownData()
        {
            Buildings = await _context.FfBuildingDirectories
                .Where(b => b.LocationName != "Unassigned")
                .OrderBy(b => b.LocationName)
                .Select(b => new SelectListItem
                {
                    Value = b.LocationCode.ToString(),
                    Text = b.LocationName
                })
                .ToListAsync();

            TicketTypes = await _context.FfTicketTypes
                .OrderBy(t => t.TypeName)
                .Select(t => new SelectListItem
                {
                    Value = t.Code.ToString(),
                    Text = t.TypeName
                })
                .ToListAsync();

            // Load residents for staff to select
            if (IsStaff)
            {
                var residents = await _userManager.GetUsersInRoleAsync("Resident");
                Residents = residents
                    .OrderBy(r => r.UserName)
                    .Select(r => new SelectListItem
                    {
                        Value = r.Id,
                        Text = $"{r.UserName} ({r.Email})"
                    })
                    .ToList();
            }
        }
    }

    public class TicketInput
    {
        [Required(ErrorMessage = "Please select a building/location")]
        [Display(Name = "Building/Location")]
        public int LocationCode { get; set; }

        [Required(ErrorMessage = "Please enter your unit number")]
        [Display(Name = "Unit Number")]
        [Range(1, 9999, ErrorMessage = "Please enter a valid unit number")]
        public int Unit { get; set; }

        [Required(ErrorMessage = "Please select an issue type")]
        [Display(Name = "Issue Type")]
        public int TicketTypeCode { get; set; }

        [Required(ErrorMessage = "Please describe the issue")]
        [Display(Name = "Description")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 2000 characters")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Resident")]
        public string? ResidentId { get; set; }

        [Display(Name = "Due Date")]
        public DateTime? DueDate { get; set; }
    }
}
