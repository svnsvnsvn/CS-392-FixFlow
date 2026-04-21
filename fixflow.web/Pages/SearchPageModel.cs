using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using fixflow.web.Services;
using fixflow.web.Dto;

namespace fixflow.web.Pages
{
    [Authorize]
    public class SearchPageModel : AppPageModel
    {
        public readonly IAdminService _adminService;


        public SearchPageModel(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public async Task<IActionResult> OnGetSearchAsync(string term, string? role)
        {
            if (string.IsNullOrWhiteSpace(term) || term.Length < 3)
                return new JsonResult(Array.Empty<UserListItemDto>());

            var results = await _adminService.SearchUsers(term);

            // If list was not entered and a role filter was supplied then
            if ((role != null) && (results.Data != null))
            {
                results.Data = results.Data
                    .Where(x => x.Role.ToLower() == role.ToLower())
                    .ToList();
            }
                
            return new JsonResult(results.Data);
        }
    }
}
