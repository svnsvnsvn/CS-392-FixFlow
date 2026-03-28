using fixflow.web.Data;
using fixflow.web.Domain.Enums;
using fixflow.web.Pages;
using fixflow.web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text.Json;

namespace fixflow.web.Pages.Admin
{
    [Authorize(Roles = "Admin,Manager,Employee")]
    public class UsersModel : UserAdminPageModel
    {
        private readonly FfDbContext _context;

        public UsersModel(FfDbContext context, UserManager<AppUser> userManager, IAdminService adminService)
            : base(adminService, userManager)
        {
            _context = context;
        }

        public IList<FfUserProfile> Users { get; set; } = default!;
        public IList<UserRowViewModel> UserRows { get; set; } = default!;
        public int TotalUsers { get; set; }
        public int ResidentCount { get; set; }
        public int StaffCount { get; set; }
        public int PendingCount { get; set; }

        public string? InitialEditUserId { get; set; }
        public string? InitialEditUserLabel { get; set; }

        /// <summary>JSON <c>{"id","label"}</c> for deep-link from table Edit (admin only).</summary>
        public string? InitialEditJson { get; set; }

        [BindProperty] public string? SelectedUserId { get; set; }
        [BindProperty] public string? SelectedUserRole { get; set; }
        [BindProperty] public bool ResetPassOnLogin { get; set; }

        [BindProperty] public string? EditFName { get; set; }
        [BindProperty] public string? EditLName { get; set; }
        [BindProperty] public string? EditUserName { get; set; }
        [BindProperty] public string? EditEmail { get; set; }
        [BindProperty] public string? EditPhone { get; set; }
        [BindProperty] public int EditLocationCode { get; set; }
        [BindProperty] public int EditUnit { get; set; }

        public List<SelectListItem> BuildingOptions { get; set; } = new();

        public async Task OnGetAsync()
        {
            await LoadBuildingOptionsAsync();

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
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains("Pending")) roleName = "Pending";
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
                    Joined = "-",
                    BuildingDisplay = profile.Location?.LocationName ?? "-"
                });
            }
            UserRows = rows;

            TotalUsers = UserRows.Count;
            ResidentCount = UserRows.Count(r => r.RoleName == "Resident");
            StaffCount = UserRows.Count(r => r.RoleName == "Admin" || r.RoleName == "Manager" || r.RoleName == "Employee");
            PendingCount = UserRows.Count(r => r.RoleName == "Pending");

            if (User.IsInRole("Admin"))
            {
                var editUser = Request.Query["editUser"].FirstOrDefault();
                if (!string.IsNullOrEmpty(editUser))
                {
                    var prof = Users.FirstOrDefault(u => u.FfUserId == editUser);
                    if (prof != null)
                    {
                        InitialEditUserId = editUser;
                        var f = (prof.FName ?? "").Trim();
                        var l = (prof.LName ?? "").Trim();
                        InitialEditUserLabel = $"{l}, {f}".Trim().TrimEnd(',').Trim();
                        if (string.IsNullOrWhiteSpace(InitialEditUserLabel))
                            InitialEditUserLabel = prof.FfUser?.Email ?? editUser;
                        InitialEditJson = JsonSerializer.Serialize(new Dictionary<string, string>
                        {
                            ["id"] = editUser,
                            ["label"] = InitialEditUserLabel
                        });
                    }
                }
            }
        }

        private async Task LoadBuildingOptionsAsync()
        {
            BuildingOptions = await _context.FfBuildingDirectorys
                .AsNoTracking()
                .Where(b => b.LocationName != "Unassigned")
                .OrderBy(b => b.LocationName)
                .Select(b => new SelectListItem
                {
                    Value = b.LocationCode.ToString(),
                    Text = b.LocationName + " (#" + b.BuildingNumber + ")"
                })
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.IsInRole("Admin"))
                return Forbid();

            await LoadBuildingOptionsAsync();

            if (string.IsNullOrWhiteSpace(SelectedUserId))
            {
                ModelState.AddModelError(string.Empty, "No user selected.");
                await OnGetAsync();
                return Page();
            }

            var selectedUser = await _userManager.FindByIdAsync(SelectedUserId);
            if (selectedUser == null)
            {
                ModelState.AddModelError(string.Empty, "Unable to find selected user.");
                await OnGetAsync();
                return Page();
            }

            var profile = await _context.FfUserProfiles.FirstOrDefaultAsync(p => p.FfUserId == SelectedUserId);
            if (profile == null)
            {
                ModelState.AddModelError(string.Empty, "This account has no profile record to update.");
                await OnGetAsync();
                return Page();
            }

            EditFName = EditFName?.Trim() ?? string.Empty;
            EditLName = EditLName?.Trim() ?? string.Empty;
            EditUserName = EditUserName?.Trim() ?? string.Empty;
            EditEmail = EditEmail?.Trim() ?? string.Empty;
            EditPhone = string.IsNullOrWhiteSpace(EditPhone) ? null : EditPhone.Trim();

            if (string.IsNullOrWhiteSpace(EditFName))
                ModelState.AddModelError(nameof(EditFName), "First name is required.");
            if (string.IsNullOrWhiteSpace(EditLName))
                ModelState.AddModelError(nameof(EditLName), "Last name is required.");
            if (string.IsNullOrWhiteSpace(EditUserName))
                ModelState.AddModelError(nameof(EditUserName), "Username is required.");
            if (string.IsNullOrWhiteSpace(EditEmail))
                ModelState.AddModelError(nameof(EditEmail), "Email is required.");

            if (EditLocationCode != 0)
            {
                var building = await _context.FfBuildingDirectorys.AsNoTracking()
                    .FirstOrDefaultAsync(b => b.LocationCode == EditLocationCode);
                if (building == null)
                    ModelState.AddModelError(nameof(EditLocationCode), "Choose a valid building or “Not assigned”.");
                else if (EditUnit > building.NumUnits || EditUnit < 0)
                    ModelState.AddModelError(nameof(EditUnit), $"Unit must be from 0 to {building.NumUnits} for this building.");
            }

            var otherEmail = await _userManager.FindByEmailAsync(EditEmail);
            if (otherEmail != null && otherEmail.Id != SelectedUserId)
                ModelState.AddModelError(nameof(EditEmail), "That email is already in use.");

            var otherUserName = await _userManager.FindByNameAsync(EditUserName);
            if (otherUserName != null && otherUserName.Id != SelectedUserId)
                ModelState.AddModelError(nameof(EditUserName), "That username is already in use.");

            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var roles = currentUser != null ? await _userManager.GetRolesAsync(currentUser) : new List<string>();

            if (!string.IsNullOrWhiteSpace(SelectedUserRole))
            {
                if (!Enum.TryParse<RoleTypes>(SelectedUserRole, out var newRole))
                    ModelState.AddModelError(nameof(SelectedUserRole), "Invalid role selection.");
                else if (currentUser != null && roles.Count > 0)
                {
                    var roleChange = await _adminService.ChangeUserRole(
                        currentUser.Id,
                        Enum.Parse<RoleTypes>(roles[0]),
                        SelectedUserId,
                        newRole);
                    if (!roleChange.Success)
                        ModelState.AddModelError(string.Empty, "Unable to change role. " + roleChange.Error);
                }
            }

            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            var nameResult = await _userManager.SetUserNameAsync(selectedUser, EditUserName);
            if (!nameResult.Succeeded)
            {
                foreach (var err in nameResult.Errors)
                    ModelState.AddModelError(nameof(EditUserName), err.Description);
                await OnGetAsync();
                return Page();
            }

            var emailResult = await _userManager.SetEmailAsync(selectedUser, EditEmail);
            if (!emailResult.Succeeded)
            {
                foreach (var err in emailResult.Errors)
                    ModelState.AddModelError(nameof(EditEmail), err.Description);
                await OnGetAsync();
                return Page();
            }

            selectedUser.PhoneNumber = EditPhone;
            selectedUser.ResetPassOnLogin = ResetPassOnLogin;
            var updateIdentity = await _userManager.UpdateAsync(selectedUser);
            if (!updateIdentity.Succeeded)
            {
                foreach (var err in updateIdentity.Errors)
                    ModelState.AddModelError(string.Empty, err.Description);
                await OnGetAsync();
                return Page();
            }

            profile.FName = EditFName;
            profile.LName = EditLName;
            profile.LocationCode = EditLocationCode;
            profile.Unit = EditUnit;
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public class UserRowViewModel
        {
            public FfUserProfile Profile { get; set; } = default!;
            public string RoleName { get; set; } = string.Empty;
            public string Joined { get; set; } = string.Empty;
            public string BuildingDisplay { get; set; } = string.Empty;
        }
    }
}
