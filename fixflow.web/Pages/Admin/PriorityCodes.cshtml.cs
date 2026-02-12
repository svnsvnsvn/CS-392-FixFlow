using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using fixflow.web.Data;

namespace fixflow.web.Pages.Admin
{
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
