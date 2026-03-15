using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using fixflow.web.Dto;
using fixflow.web.Domain.Enums;

namespace fixflow.web.Pages
{
    public class AppPageModel : PageModel
    {
        public AppPageModel() { }

        protected UserCredentialDTO LoggedInUser => new UserCredentialDTO
        {
            UserId = User?.FindFirstValue(ClaimTypes.NameIdentifier),
            Role = Enum.TryParse<RoleTypes>(
                User?.FindFirstValue(ClaimTypes.Role),
                out var role)
                ? role
                : null
        };
    }
}
