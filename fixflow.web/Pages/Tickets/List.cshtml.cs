using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using fixflow.web.Data;

namespace fixflow.web.Pages.Tickets
{
    public class ListModel : PageModel
    {
        private readonly FfDbContext _context;

        public ListModel(FfDbContext context)
        {
            _context = context;
        }

        public IList<FfTicketRegister> Tickets { get; set; } = default!;

        public async Task OnGetAsync()
        {
            // Load tickets list here
            Tickets = await _context.FfTicketRegister.ToListAsync();
        }
    }
}
