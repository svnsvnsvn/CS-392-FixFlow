using fixflow.web.Data;
using fixflow.web.Domain.Enums;
using fixflow.web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace fixflow.web.Pages.Admin
{
    [Authorize(Roles = nameof(RoleTypes.Admin))]
    public class StatusCodesModel : AppPageModel
    {
        private readonly FfDbContext _context;
        private readonly IAdminService _adminService;

        public StatusCodesModel(FfDbContext context, IAdminService adminService)
        {
            _context = context;
            _adminService = adminService;
        }

        public IList<FfStatusCodes> StatusCodes { get; set; } = default!;

        public async Task OnGetAsync()
        {
            StatusCodes = await _context.FfStatusCodes.OrderBy(p => p.StatusCode).ToListAsync();
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
