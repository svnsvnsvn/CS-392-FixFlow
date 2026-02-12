using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using fixflow.web.Data;

namespace fixflow.web.Pages.Admin
{
    public class BuildingsModel : PageModel
    {
        private readonly FfDbContext _context;

        public BuildingsModel(FfDbContext context)
        {
            _context = context;
        }

        public IList<FfBuildingDirectory> Buildings { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Buildings = await _context.FfBuildingDirectory.ToListAsync();
        }
    }
}
