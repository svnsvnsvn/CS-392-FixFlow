using fixflow.web.Data;
using fixflow.web.Domain.Enums;
using fixflow.web.Services;
using fixflow.web.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace fixflow.web.Pages.Admin
{
    [Authorize(Roles = nameof(RoleTypes.Admin))] // Restrict access to only admin users
    public class BuildingsModel : PageModel
    {
        private readonly FfDbContext _context;
        private readonly ITicketService _ticketService;

        public BuildingsModel(FfDbContext context, ITicketService ticketService)
        {
            _context = context;
            _ticketService = ticketService;
        }

        public List<BuildingDto> Buildings { get; set; } = default!;

        public async Task OnGetAsync()
        {
            //Buildings = await _context.FfBuildingDirectorys.ToListAsync();

            var buildingResult = await _ticketService.GetBuildings();
            if ((buildingResult.Success) && (buildingResult.Data != null))
            {
                Buildings = await _context.FfBuildingDirectorys
                    .Where(b => b.LocationName != "Unassigned")
                    .OrderBy(b => b.LocationName)
                    .Select(b => new BuildingDto
                    {
                        LocationCode = b.LocationCode,
                        LocationName = b.LocationName,
                        ComplexName = b.ComplexName,
                        BuildingNumber = b.BuildingNumber,
                        NumUnits = b.NumUnits,
                        LocationLat = b.LocationLat,
                        LocationLon = b.LocationLon
                    })
                    .ToListAsync();
            }
        }
    }
}
