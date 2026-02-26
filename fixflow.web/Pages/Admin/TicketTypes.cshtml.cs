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
    public class TicketTypesModel : PageModel
    {
        private readonly FfDbContext _context;

        public TicketTypesModel(FfDbContext context)
        {
            _context = context;
        }

        public IList<FfTicketTypes> TicketTypes { get; set; } = default!;

        public async Task OnGetAsync()
        {
            TicketTypes = await _context.FfTicketTypes.ToListAsync();
        }
    }
}
