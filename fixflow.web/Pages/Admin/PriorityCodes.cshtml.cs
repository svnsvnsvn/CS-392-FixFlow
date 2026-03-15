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
    public class PriorityCodesModel : AppPageModel
    {
        private readonly FfDbContext _context;
        public readonly IAdminService _adminService;

        public PriorityCodesModel(FfDbContext context, IAdminService adminService)
        {
            _context = context;
            _adminService = adminService;
        }

        public IList<FfPriorityCodes> PriorityCodes { get; set; } = default!;

        public async Task OnGetAsync()
        {
            PriorityCodes = await _context.FfPriorityCodess.OrderBy(p => p.PriorityCode).ToListAsync();
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
