using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using fixflow.web.Data;

namespace fixflow.web.Pages.Admin
{
    public class UsersModel : PageModel
    {
        private readonly FfDbContext _context;

        public UsersModel(FfDbContext context)
        {
            _context = context;
        }

        public IList<FfUserProfile> Users { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Users = await _context.FfUserProfiles.ToListAsync();
        }
    }
}
