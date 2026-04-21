using fixflow.web.Data;
using fixflow.web.Domain.Enums;
using fixflow.web.Dto;
using fixflow.web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace fixflow.web.Pages.Tickets
{
    public class CreateModel : SearchPageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITicketService _ticketService;
        private readonly IAdminService _adminService;

        public CreateModel(UserManager<AppUser> userManager, ITicketService ticketService, IAdminService adminService) :base(adminService)
        {
            _userManager = userManager;
            _ticketService = ticketService;
            _adminService = adminService;
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
                var userProfileResult = await _adminService.GetUserProfileById(user.Id);
                var userProfile = userProfileResult.Success ? userProfileResult.Data : null;

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
                    var priorityListResult = await _adminService.GetPriorityCodeList();
                    var firstPri = priorityListResult.Success && priorityListResult.Data != null
                        ? priorityListResult.Data.OrderBy(p => p.PriorityCode).FirstOrDefault()
                        : null;
                    if (firstPri == null)
                    {
                        ModelState.AddModelError(string.Empty, "No priority codes are configured.");
                        await LoadDropdownDataAsync(user);
                        return Page();
                    }
                }
                var priResult = await _ticketService.GetPriorityCode("Unassigned");
                priorityValue = priResult.Data;   // Unassigned, staff will assign priority
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
                return RedirectToPage("/Tickets/Create");
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
                Buildings = buildingResult.Data
                    .OrderBy(b => b.LocationName)
                    .Select(b => new SelectListItem
                    {
                        Value = b.LocationCode.ToString(),
                        Text = b.LocationName + " (#" + b.BuildingNumber + ")"
                    })
                    .ToList();
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
                var profilesResult = await _adminService.GetUserProfilesByIds(ids);
                var profileRows = profilesResult.Success && profilesResult.Data != null
                    ? profilesResult.Data
                    : new List<FfUserProfile>();
                var byId = profileRows.ToDictionary(
                    p => p.FfUserId,
                    p => new Dictionary<string, int> { ["location"] = p.LocationCode, ["unit"] = p.Unit });
                StaffResidentProfilesJson = JsonSerializer.Serialize(byId);

                var priorityListResult = await _adminService.GetPriorityCodeList();
                var priorities = priorityListResult.Success && priorityListResult.Data != null
                    ? priorityListResult.Data
                    : new List<PriorityCodeDto>();
                Priorities = priorities
                    .OrderBy(p => p.PriorityCode)
                    .Select(p => new SelectListItem
                    {
                        Value = p.PriorityCode.ToString(),
                        Text = p.PriorityName
                    })
                    .ToList();
            }
            else if (currentUser != null)
            {
                var profileResult = await _adminService.GetUserProfileById(currentUser.Id);
                var prof = profileResult.Success ? profileResult.Data : null;
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