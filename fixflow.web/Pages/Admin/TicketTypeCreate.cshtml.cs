using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using fixflow.web.Domain.Enums;
using fixflow.web.Services;
using fixflow.web.Dto;

namespace fixflow.web.Pages.Admin
{
    [Authorize(Roles = nameof(RoleTypes.Admin))]
    public class TicketTypeCreateModel : AppPageModel
    {
        private readonly IAdminService _adminService;

        public TicketTypeCreateModel(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [BindProperty]
        public TicketTypeInput Input { get; set; } = new();

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            NewTicketTypeDto newTicket = new NewTicketTypeDto();
            newTicket.TypeName = Input.TypeName;

            _adminService.AddTicketType(LoggedInUser.UserId, (RoleTypes)LoggedInUser.Role, newTicket);
            return RedirectToPage("./TicketTypes");
        }
    }

    public class TicketTypeInput
    {
        public string TypeName { get; set; } = string.Empty;
    }
}
