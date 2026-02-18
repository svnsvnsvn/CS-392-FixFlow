using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using fixflow.web.Data;
using fixflow.web.Domain.Constants;

namespace fixflow.web.Pages.Admin
{
    [Authorize(Roles = RoleNames.Admin)]
    public class StatusCodesModel : PageModel
    {
        private readonly FfDbContext _context;

        public StatusCodesModel(FfDbContext context)
        {
            _context = context;
        }

        public IList<FfStatusCodes> StatusCodes { get; set; } = default!;

        public async Task OnGetAsync()
        {
            StatusCodes = await _context.FfStatusCodes.ToListAsync();
        }
    }
}
