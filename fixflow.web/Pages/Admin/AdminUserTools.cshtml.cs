using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using fixflow.web.Data;
using fixflow.web.Domain.Enums;
using System.Security.Claims;
using fixflow.web.Pages.Account;
using System.Threading.Tasks;
using fixflow.web.Services;
using fixflow.web.Dto;


namespace fixflow.web.Pages.Admin
{
    [Authorize(Roles = nameof(RoleTypes.Admin))] // Restrict access to only Admin users
    public class AdminUserToolsModel : UserAdminPageModel
    {
        private readonly FfDbContext _db;
        //private readonly UserManager<AppUser> _userManager;

        public string? Query { get; set; }
        public IList<UserListItemDto>? SearchResults { get; set; } = new List<UserListItemDto>();

        // bind posted form fields
        [BindProperty] public string? SelectedUserId { get; set; }
        [BindProperty] public string? SelectedUserRole { get; set; }
        [BindProperty] public bool ResetPassOnLogin { get; set; }
        public string? CurrentRole { get; set; }



        public AdminUserToolsModel(FfDbContext db, UserManager<AppUser> userManager, IAdminService adminService) : base(adminService,userManager)
        {
            _db = db;
        }

        public async Task OnGetAsync()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // validate a user was selected

            if (string.IsNullOrWhiteSpace(SelectedUserId))
            {
                ModelState.AddModelError(string.Empty, "No user selected.");
                await OnGetAsync(); // repopulate lists
                return Page();
            }

            if (string.IsNullOrWhiteSpace(SelectedUserRole))
            {
                ModelState.AddModelError(string.Empty, "Select a target role.");
                await OnGetAsync();
                return Page();
            }

            var SelectedUser = await _userManager.FindByIdAsync(SelectedUserId);
            if (SelectedUser == null)
            {
                ModelState.AddModelError(string.Empty, "Unable to find selected user.");
                await OnGetAsync();
                return Page();
            }


            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);

            if ((!string.IsNullOrEmpty(user.Id)) && (roles.Count > 0))
            {
                var result = await _adminService.ChangeUserRole(user.Id.ToString(), Enum.Parse<RoleTypes>(roles[0]), SelectedUserId, Enum.Parse<RoleTypes>(SelectedUserRole));
                if (!result.Success)
                {
                    ModelState.AddModelError(string.Empty, "Unable to change role. " + result.Error);
                    await OnGetAsync();
                    return Page();
                }
            }

            SelectedUser.ResetPassOnLogin = ResetPassOnLogin;

            await _userManager.UpdateAsync(SelectedUser);

            return RedirectToPage(); // refresh page after success
        }
    }
}
