using fixflow.web.Domain.Enums;
using fixflow.web.Dto;
using fixflow.web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace fixflow.web.Pages.Admin
{
    [Authorize(Roles = nameof(RoleTypes.Admin))]
    public class TicketTypesModel : AppPageModel
    {
        private readonly IAdminService _adminService;

        public TicketTypesModel(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public IList<TicketTypeDto> TicketTypes { get; set; } = default!;

        public async Task OnGetAsync()
        {
            var results = await _adminService.GetTicketTypeList();
            TicketTypes = results.Success && results.Data != null
                ? results.Data
                : new List<TicketTypeDto>();
        }
        public async Task<IActionResult> OnGetDeleteStatusAsync(int id)
        {
            await _adminService.DeleteTicketType(LoggedInUser.UserId, (RoleTypes)LoggedInUser.Role, id);
            return RedirectToPage();
        }
    }
}