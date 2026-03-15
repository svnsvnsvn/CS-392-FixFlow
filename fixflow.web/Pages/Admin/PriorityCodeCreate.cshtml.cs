using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using fixflow.web.Domain.Enums;
using fixflow.web.Services;
using System.Threading.Tasks;
using fixflow.web.Dto;

namespace fixflow.web.Pages.Admin
{
    [Authorize(Roles = nameof(RoleTypes.Admin))]

    public class PriorityCodeCreateModel : AppPageModel
    {
        public readonly IAdminService _adminService;
        public readonly ITicketService _ticketService;

        public PriorityCodeCreateModel (IAdminService adminService, ITicketService ticketService)
        {
            _adminService = adminService;
            _ticketService = ticketService;
        }


        [BindProperty]
        public PriorityCodeInput Input { get; set; } = new();



        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            NewPriorityCodeDto newCode = new NewPriorityCodeDto();
            newCode.PriorityName = Input.PriorityName;
            newCode.PriorityCode = Input.PriorityCode;

            var result = await _adminService.AddPriorityCode(LoggedInUser.UserId, (RoleTypes)LoggedInUser.Role, newCode);
            return RedirectToPage("./PriorityCodes");
        }
    }

    public class PriorityCodeInput
    {
        public int PriorityCode { get; set; }
        public string PriorityName { get; set; } = string.Empty;
    }
}
