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
    public class TicketTypesModel : AppPageModel
    {
        private readonly FfDbContext _context;
        private readonly IAdminService _adminService;

        public TicketTypesModel(FfDbContext context, IAdminService adminService)
        {
            _context = context;
            _adminService = adminService;
        }

        public IList<FfTicketTypes> TicketTypes { get; set; } = default!;

        public async Task OnGetAsync()
        {
            TicketTypes = await _context.FfTicketTypess.ToListAsync();
        }
        public async Task<IActionResult> OnGetDeleteStatusAsync(int id)
        {
            await _adminService.DeleteTicketType(LoggedInUser.UserId, (RoleTypes)LoggedInUser.Role, id);
            return RedirectToPage();
        }
    }
}