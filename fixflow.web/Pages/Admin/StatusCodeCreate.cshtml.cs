using fixflow.web.Domain.Enums;
using fixflow.web.Dto;
using fixflow.web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace fixflow.web.Pages.Admin
{
    [Authorize(Roles = nameof(RoleTypes.Admin))]
    public class StatusCodeCreateModel : AppPageModel
    {
        public readonly IAdminService _adminService;
        public readonly ITicketService _ticketService;

        public StatusCodeCreateModel(IAdminService adminService, ITicketService ticketService)
        {
            _adminService = adminService;
            _ticketService = ticketService;
        }

        [BindProperty]
        public StatusCodeInput Input { get; set; } = new();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            NewStatusCodeDto newCode = new NewStatusCodeDto();
            newCode.StatusName = Input.StatusName;
            newCode.StatusCode = Input.StatusCode;

            var result = await _adminService.AddStatusCode(LoggedInUser.UserId, (RoleTypes)LoggedInUser.Role, newCode);
            return RedirectToPage("./StatusCodes");
        }
    }

    public class StatusCodeInput
    {
        public int StatusCode { get; set; }
        public string StatusName { get; set; } = string.Empty;
    }
}
