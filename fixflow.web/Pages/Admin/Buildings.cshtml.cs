using fixflow.web.Domain.Enums;
using fixflow.web.Services;
using fixflow.web.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace fixflow.web.Pages.Admin
{
    [Authorize(Roles = nameof(RoleTypes.Admin))] // Restrict access to only admin users
    public class BuildingsModel : PageModel
    {
        private readonly IAdminService _adminService;

        public BuildingsModel(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public List<BuildingDto> Buildings { get; set; } = default!;

        public async Task OnGetAsync()
        {
            var buildingResult = await _adminService.GetBuildingList();
            Buildings = (buildingResult.Success && buildingResult.Data != null)
                ? buildingResult.Data
                : new List<BuildingDto>();
        }
    }
}
