using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using fixflow.web.Data;

namespace fixflow.web.Pages.Profile
{
    public class EditModel : PageModel
    {
        private readonly FfDbContext _context;

        public EditModel(FfDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public FfUserProfile UserProfile { get; set; } = default!;

        public IActionResult OnGet()
        {
            // Load current user profile here
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Add profile update logic here

            return RedirectToPage("./Index");
        }
    }
}
