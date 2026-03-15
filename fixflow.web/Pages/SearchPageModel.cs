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

        public async Task<IActionResult> OnGetSearchAsync(string term)
        {
            if (string.IsNullOrWhiteSpace(term) || term.Length < 3)
                return new JsonResult(Array.Empty<UserListItemDto>());

            var results = await _adminService.SearchUsers(term);
            return new JsonResult(results.Data);
        }
    }
}
