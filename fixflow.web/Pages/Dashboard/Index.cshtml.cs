using Microsoft.AspNetCore.Mvc.RazorPages;
using fixflow.web.Data;

namespace fixflow.web.Pages.Dashboard
{
    public class IndexModel : PageModel
    {
        private readonly FfDbContext _context;

        public IndexModel(FfDbContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
            // Load dashboard data here
        }
    }
}
