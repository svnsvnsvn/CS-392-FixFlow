using fixflow.web.Data;
using fixflow.web.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace fixflow.web.Pages.Admin
{
    [Authorize(Roles = "Admin,Manager,Employee")]
    public class UsersModel : PageModel
    {
        private readonly FfDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public UsersModel(FfDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<FfUserProfile> Users { get; set; } = default!;
        public IList<UserRowViewModel> UserRows { get; set; } = default!;
        public int TotalUsers { get; set; }
        public int ResidentCount { get; set; }
        public int StaffCount { get; set; }
        public int PendingCount { get; set; }

        public async Task OnGetAsync()
        {
            Users = await _context.FfUserProfiles
                .Include(p => p.FfUser)
                .Include(p => p.Location)
                .OrderBy(p => p.LName)
                .ThenBy(p => p.FName)
                .ToListAsync();

            var rows = new List<UserRowViewModel>();
            foreach (var profile in Users)
            {
                var user = profile.FfUser;
                var roleName = "Resident";
                var status = "Active";
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains("Pending")) { roleName = "Pending"; status = "Pending"; }
                    else if (roles.Contains("Admin")) roleName = "Admin";
                    else if (roles.Contains("Manager")) roleName = "Manager";
                    else if (roles.Contains("Employee")) roleName = "Employee";
                    else if (roles.Contains("Resident")) roleName = "Resident";
                    else if (roles.Count > 0) roleName = roles[0];
                }
                rows.Add(new UserRowViewModel
                {
                    Profile = profile,
                    RoleName = roleName,
                    Status = status,
                    Joined = "-",
                    BuildingDisplay = profile.Location?.LocationName ?? "-"
                });
            }
            UserRows = rows;

            TotalUsers = UserRows.Count;
            ResidentCount = UserRows.Count(r => r.RoleName == "Resident");
            StaffCount = UserRows.Count(r => r.RoleName == "Admin" || r.RoleName == "Manager" || r.RoleName == "Employee");
            PendingCount = UserRows.Count(r => r.Status == "Pending");
        }

        public class UserRowViewModel
        {
            public FfUserProfile Profile { get; set; } = default!;
            public string RoleName { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public string Joined { get; set; } = string.Empty;
            public string BuildingDisplay { get; set; } = string.Empty;
        }
    }
}