using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using fixflow.web.Data;
using fixflow.web.Domain.Constants;

namespace fixflow.web.Pages.Admin
{
    [Authorize(Roles = RoleNames.Admin)]
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
            Buildings = await _context.FfBuildingDirectories.ToListAsync();
        }
    }
}
