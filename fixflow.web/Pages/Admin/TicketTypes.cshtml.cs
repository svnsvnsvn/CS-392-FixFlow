using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using fixflow.web.Data;

namespace fixflow.web.Pages.Admin
{
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
            TicketTypes = await _context.FfTicketTypess.ToListAsync();
        }
    }
}
