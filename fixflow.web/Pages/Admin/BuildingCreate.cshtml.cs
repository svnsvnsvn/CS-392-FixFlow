using fixflow.web.Data;
using fixflow.web.Domain.Constants;
using fixflow.web.Domain.Enums;
using fixflow.web.Dto;
using fixflow.web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace fixflow.web.Pages.Admin
{
    [Authorize(Roles = nameof(RoleTypes.Admin))]
    public class BuildingCreateModel : PageModel
    {
        private readonly IAdminService _adminService;
        private readonly UserManager<AppUser> _userManager;

        public BuildingCreateModel(IAdminService adminService, UserManager<AppUser> userManager) 
        {
            _adminService = adminService;
            _userManager = userManager;
        }
        
        [BindProperty]
        public BuildingInput Input { get; set; } = new();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Account/Login");
            }

            var newBuilding = new NewBuildingDto();
            
            newBuilding.LocationName = Input.LocationName;
            newBuilding.ComplexName = Input.ComplexName;
            newBuilding.BuildingNumber = Input.BuildingNumber;
            newBuilding.NumUnits = Input.NumUnits;
            newBuilding.LocationLat = Input.LocationLat;
            newBuilding.LocationLon = Input.LocationLon;

            // Determine user role
            RoleTypes userRole = RoleTypes.Resident;
            if (User.IsInRole(RoleNames.Admin))
                userRole = RoleTypes.Admin;
            else if (User.IsInRole(RoleNames.Manager))
                userRole = RoleTypes.Manager;
            else if (User.IsInRole(RoleNames.Employee))
                userRole = RoleTypes.Employee;

            var result = await _adminService.AddBuilding(user.Id, userRole, newBuilding);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Error ?? "Building could not be created.");
                return Page();
            }

            // TODO: Wire to AdminService to create the building.
            return RedirectToPage("./Buildings");
        }
    }

    public class BuildingInput
    {
        public string LocationName { get; set; } = string.Empty;
        public string ComplexName { get; set; } = string.Empty;
        public int BuildingNumber { get; set; }
        public int NumUnits { get; set; }
        public decimal? LocationLat { get; set; }
        public decimal? LocationLon { get; set; }
    }
}
