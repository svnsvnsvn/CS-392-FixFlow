using fixflow.web.Data;
using fixflow.web.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace fixflow.web.Pages.Admin
{
    [Authorize(Roles = nameof(RoleTypes.Admin))] // Restrict access to only admin users
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
            Buildings = await _context.FfBuildingDirectorys.ToListAsync();
        }
    }
}
