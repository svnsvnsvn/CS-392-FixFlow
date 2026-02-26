<<<<<<< HEAD
using fixflow.web.Data;
using fixflow.web.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace fixflow.web.Pages.Admin
{
    [Authorize(Roles = nameof(RoleTypes.Admin))]
=======
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using fixflow.web.Data;
using fixflow.web.Domain.Constants;

namespace fixflow.web.Pages.Admin
{
    [Authorize(Roles = RoleNames.Admin)]
>>>>>>> origin/ZZ-development/dashboard-refresh
    public class PriorityCodesModel : PageModel
    {
        private readonly FfDbContext _context;

        public PriorityCodesModel(FfDbContext context)
        {
            _context = context;
        }

        public IList<FfPriorityCodes> PriorityCodes { get; set; } = default!;

        public async Task OnGetAsync()
        {
            PriorityCodes = await _context.FfPriorityCodes.ToListAsync();
        }
    }
}
