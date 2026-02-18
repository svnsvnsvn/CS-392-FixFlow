using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using fixflow.web.Data;
using fixflow.web.Domain.Constants;

namespace fixflow.web.Pages.Admin
{
    [Authorize(Roles = RoleNames.Admin)]
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
