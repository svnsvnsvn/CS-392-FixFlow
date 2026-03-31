using fixflow.web.Domain.Enums;
using fixflow.web.Dto;
using fixflow.web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace fixflow.web.Pages.Admin
{
    [Authorize(Roles = nameof(RoleTypes.Admin))]
    public class PriorityCodesModel : AppPageModel
    {
        public readonly IAdminService _adminService;

        public PriorityCodesModel(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public IList<PriorityCodeDto> PriorityCodes { get; set; } = default!;

        public async Task OnGetAsync()
        {
            var results = await _adminService.GetPriorityCodeList();
            PriorityCodes = results.Success && results.Data != null
                ? results.Data
                : new List<PriorityCodeDto>();
        }
        public async Task<IActionResult> OnGetIncreasePriorityAsync(int id)
        {
            await _adminService.IncrementPriorityCode(LoggedInUser.UserId, (RoleTypes)LoggedInUser.Role, id);
            return RedirectToPage();
        }
        public async Task<IActionResult> OnGetDecreasePriorityAsync(int id)
        {
            await _adminService.DecrementPriorityCode(LoggedInUser.UserId, (RoleTypes)LoggedInUser.Role, id);
            return RedirectToPage();
        }
        public async Task<IActionResult> OnGetDeletePriorityAsync(int id)
        {
            await _adminService.DeletePriorityCode(LoggedInUser.UserId, (RoleTypes)LoggedInUser.Role, id);
            return RedirectToPage();
        }
    }
}
