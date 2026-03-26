using fixflow.web.Data;
using fixflow.web.Domain.Enums;
using fixflow.web.Dto;
using fixflow.web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

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
        public List<SelectListItem> Priorities { get; set; } = new();
        public List<SelectListItem> Residents { get; set; } = new();
        public bool IsStaff { get; set; }

        /// <summary>JSON <c>{ "location": n, "unit": n }</c> for signed-in resident profile (client autofill).</summary>
        public string? ResidentProfileAutofillJson { get; set; }

        /// <summary>JSON map of resident user id → location/unit for staff “on behalf of” autofill.</summary>
        public string? StaffResidentProfilesJson { get; set; }

        [BindProperty]
        public TicketInput Input { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            // Get logged in user data
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Page();
            }

            // Get users role
            var roles = await _userManager.GetRolesAsync(user);
            if (roles == null)
            {
                return Page();
            }
            RoleTypes userRole = Enum.Parse<RoleTypes>(roles.FirstOrDefault());


            // Check if user is staff (Manager, Technician, or Admin)
            IsStaff = (userRole == RoleTypes.Manager || userRole == RoleTypes.Employee || userRole == RoleTypes.Admin);
                        

            await LoadDropdownDataAsync(user);

            // Pre-populate location and unit from user profile if available (for residents only)
            if (!IsStaff && user != null)
            {
                var userProfile = await _context.FfUserProfiles
                    .FirstOrDefaultAsync(p => p.FfUserId == user.Id);

                if (userProfile != null)
                {
                    Input.LocationCode = userProfile.LocationCode;
                    Input.Unit = userProfile.Unit;
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Get logged in user data
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Page();
            }

            // Get users role
            var roles = await _userManager.GetRolesAsync(user);
            if (roles == null)
            {
                return Page();
            }
            RoleTypes userRole = Enum.Parse<RoleTypes>(roles.FirstOrDefault());

            IsStaff = (userRole == RoleTypes.Manager || userRole == RoleTypes.Employee || userRole == RoleTypes.Admin);

            if (!ModelState.IsValid)
            {
                await LoadDropdownDataAsync(user);
                return Page();
            }

            if (user == null)
            {
                return RedirectToPage("/Account/Login");
            }

            var submittedStatus = await _ticketService.GetStatusCode("Submitted");
            if (!submittedStatus.Success)
            {
                ModelState.AddModelError(string.Empty, "Ticket status “Submitted” is not configured. Ask an admin to seed status codes.");
                await LoadDropdownDataAsync(user);
                return Page();
            }

            int priorityValue;
            if (IsStaff)
            {
                if (Input.TicketPriorityCode <= 0)
                {
                    ModelState.AddModelError(nameof(Input.TicketPriorityCode), "Please select a priority.");
                    await LoadDropdownDataAsync(user);
                    return Page();
                }

                priorityValue = Input.TicketPriorityCode;
            }
            else
            {
                var medium = await _ticketService.GetPriorityCode("Medium");
                var normal = await _ticketService.GetPriorityCode("Normal");
                if (medium.Success)
                    priorityValue = medium.Data;
                else if (normal.Success)
                    priorityValue = normal.Data;
                else
                {
                    var firstPri = await _context.FfPriorityCodess.OrderBy(p => p.PriorityCode).FirstOrDefaultAsync();
                    if (firstPri == null)
                    {
                        ModelState.AddModelError(string.Empty, "No priority codes are configured.");
                        await LoadDropdownDataAsync(user);
                        return Page();
                    }

                    priorityValue = firstPri.PriorityCode;
                }
            }

            var subject = $"Unit {Input.Unit} — maintenance request";

            // Map to the DTO that the real backend expects
            var newTicketDto = new NewTicketDto
            {
                RequestedBy = string.IsNullOrEmpty(Input.ResidentId) ? null : Input.ResidentId,
                Location = Input.LocationCode,
                Unit = Input.Unit,
                TicketTroubleType = Input.TicketTypeCode,
                TicketPriority = priorityValue,
                TicketStatus = submittedStatus.Data,
                TicketSubject = subject,
                TicketDescription = Input.Description
            };

            var result = await _ticketService.AddNewTicket(user.Id, userRole, newTicketDto);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Error ?? "Ticket could not be created.");
                await LoadDropdownDataAsync(user);
                return Page();
            }

            TempData["SuccessMessage"] = $"Ticket created successfully!";
            if (userRole == RoleTypes.Resident || userRole == RoleTypes.Pending)
            {
                return RedirectToPage("/Dashboard");
            }

            return RedirectToPage("./List");
        }

        private async Task LoadDropdownDataAsync(AppUser? currentUser)
        {
            ResidentProfileAutofillJson = null;
            StaffResidentProfilesJson = null;

            var buildingResult = await _ticketService.GetBuildings();
            if ((buildingResult.Success)&&(buildingResult.Data != null))
            {
                Buildings = await _context.FfBuildingDirectorys
                    .Where(b => b.LocationName != "Unassigned")
                    .OrderBy(b => b.LocationName)
                    .Select(b => new SelectListItem
                    {
                        Value = b.LocationCode.ToString(),
                        Text = b.LocationName + " (#" + b.BuildingNumber + ")"
                    })
                    .ToListAsync();
            }
            else
            {
                Buildings.Add(new SelectListItem
                {
                    Value = "X",
                    Text = buildingResult.Error
                });
            }

            var ticketTypeResult = await _ticketService.GetTicketTypes();
            if ((ticketTypeResult.Success) && (ticketTypeResult.Data != null))
            {
                TicketTypes = ticketTypeResult.Data.Select(t => new SelectListItem 
                { 
                    Value = t.Id.ToString(),
                    Text = t.TypeName
                }).ToList();
            }
            else
            {
                TicketTypes.Add(new SelectListItem
                {
                    Value = "X",
                    Text = ticketTypeResult.Error
                });
            }


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

                var ids = residents.Select(r => r.Id).ToList();
                var profileRows = await _context.FfUserProfiles.AsNoTracking()
                    .Where(p => ids.Contains(p.FfUserId))
                    .Select(p => new { p.FfUserId, p.LocationCode, p.Unit })
                    .ToListAsync();
                var byId = profileRows.ToDictionary(
                    r => r.FfUserId,
                    r => new Dictionary<string, int> { ["location"] = r.LocationCode, ["unit"] = r.Unit });
                StaffResidentProfilesJson = JsonSerializer.Serialize(byId);

                Priorities = await _context.FfPriorityCodess
                    .OrderBy(p => p.PriorityCode)
                    .Select(p => new SelectListItem
                    {
                        Value = p.PriorityCode.ToString(),
                        Text = p.PriorityName
                    })
                    .ToListAsync();
            }
            else if (currentUser != null)
            {
                var prof = await _context.FfUserProfiles.AsNoTracking()
                    .FirstOrDefaultAsync(p => p.FfUserId == currentUser.Id);
                if (prof != null)
                {
                    ResidentProfileAutofillJson = JsonSerializer.Serialize(
                        new Dictionary<string, int> { ["location"] = prof.LocationCode, ["unit"] = prof.Unit });
                }
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

        [Display(Name = "Priority")]
        public int TicketPriorityCode { get; set; }

        [Required(ErrorMessage = "Please describe the issue")]
        [Display(Name = "Description")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 2000 characters")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Resident")]
        public string? ResidentId { get; set; }
    }
}