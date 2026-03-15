using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using fixflow.web.Dto;
using fixflow.web.Services;
using fixflow.web.Data;

namespace fixflow.web.Pages
{
    [Authorize]
    public class UserAdminPageModel : SearchPageModel
    {
        public readonly UserManager<AppUser> _userManager;
        public UserAdminPageModel(IAdminService adminService, UserManager<AppUser> userManager) : base(adminService)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetUserSettingsAsync(string selectedUser)
        {
            // Find target employee
            var targetUser = await _userManager.FindByIdAsync(selectedUser);

            if ((string.IsNullOrWhiteSpace(selectedUser)) || (targetUser == null))
                return new JsonResult(new UserSettingsListItemDTO { });

            var results = await _adminService.GetUserSettings(LoggedInUser.UserId, (Domain.Enums.RoleTypes)LoggedInUser.Role, selectedUser);
            return new JsonResult(results.Data);
        }
    }
}
