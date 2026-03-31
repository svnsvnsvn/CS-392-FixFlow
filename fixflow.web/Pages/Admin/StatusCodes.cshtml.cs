using fixflow.web.Domain.Enums;
using fixflow.web.Dto;
using fixflow.web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace fixflow.web.Pages.Admin
{
    [Authorize(Roles = nameof(RoleTypes.Admin))]
    public class StatusCodesModel : AppPageModel
    {
        private readonly IAdminService _adminService;

        public StatusCodesModel(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public IList<StatusCodeDto> StatusCodes { get; set; } = default!;

        public async Task OnGetAsync()
        {
            var results = await _adminService.GetStatusCodeList();
            StatusCodes = results.Success && results.Data != null
                ? results.Data
                : new List<StatusCodeDto>();
        }
        public async Task<IActionResult> OnGetIncreaseStatusAsync(int id)
        {
            await _adminService.IncrementStatusCode(LoggedInUser.UserId, (RoleTypes)LoggedInUser.Role, id);
            return RedirectToPage();
        }
        public async Task<IActionResult> OnGetDecreaseStatusAsync(int id)
        {
            await _adminService.DecrementStatusCode(LoggedInUser.UserId, (RoleTypes)LoggedInUser.Role, id);
            return RedirectToPage();
        }
        public async Task<IActionResult> OnGetDeleteStatusAsync(int id)
        {
            await _adminService.DeleteStatusCode(LoggedInUser.UserId, (RoleTypes)LoggedInUser.Role, id);
            return RedirectToPage();
        }
    }
}
