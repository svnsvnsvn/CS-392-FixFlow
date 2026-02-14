using Microsoft.AspNetCore.Mvc.RazorPages;
using fixflow.web.Data;

namespace fixflow.web.Pages.Profile
{
    public class IndexModel : PageModel
    {
        private readonly FfDbContext _context;

        public IndexModel(FfDbContext context)
        {
            _context = context;
        }

        public FfUserProfile UserProfile { get; set; } = default!;

        public void OnGet()
        {
            // Load current user profile here
        }
    }
}
