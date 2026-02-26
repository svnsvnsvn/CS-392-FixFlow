using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using fixflow.web.Domain.Constants;

namespace fixflow.web.Pages.Admin
{
    [Authorize(Roles = RoleNames.Admin)]
    public class BuildingCreateModel : PageModel
    {
        [BindProperty]
        public BuildingInput Input { get; set; } = new();

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // TODO: Wire to AdminService to create the building.
            return RedirectToPage("./Buildings");
        }
    }

    public class BuildingInput
    {
        public string LocationName { get; set; } = string.Empty;
        public string ComplexName { get; set; } = string.Empty;
        public int BuildingNumber { get; set; }
        public int NumUnits { get; set; }
        public decimal? LocationLat { get; set; }
        public decimal? LocationLon { get; set; }
    }
}
