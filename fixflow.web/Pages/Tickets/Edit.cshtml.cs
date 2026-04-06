using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using fixflow.web.Data;
using fixflow.web.Domain.Enums;
using fixflow.web.Dto;
using fixflow.web.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace fixflow.web.Pages.Tickets
{
    public class EditModel : AppPageModel
    {
        private readonly ITicketService _ticketService;
        private readonly IAdminService _adminService;

        public EditModel(ITicketService ticketService, IAdminService adminService)
        {
            _ticketService = ticketService;
            _adminService = adminService;
        }

        public FfTicketRegister Ticket { get; set; } = default!;
        public List<SelectListItem> Buildings { get; set; } = new();
        public List<SelectListItem> TicketTypes { get; set; } = new();
        public List<SelectListItem> Priorities { get; set; } = new();

        [BindProperty]
        public TicketEditInput Input { get; set; } = new();

        public bool CanEditTicket { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            CanEditTicket = User.IsInRole(RoleTypes.Admin.ToString())
                || User.IsInRole(RoleTypes.Manager.ToString())
                || User.IsInRole(RoleTypes.Employee.ToString());

            if (!CanEditTicket)
            {
                return Forbid();
            }

            var ticketResult = await _ticketService.GetTicketById(id);
            if (!ticketResult.Success || ticketResult.Data == null)
            {
                return NotFound();
            }

            Ticket = ticketResult.Data;
            Input = new TicketEditInput
            {
                TicketId = Ticket.TicketId,
                RequestedBy = Ticket.RequestedBy,
                LocationCode = Ticket.Location,
                Unit = Ticket.Unit,
                TicketTypeCode = Ticket.TicketTroubleType,
                TicketPriorityCode = Ticket.TicketPriority,
                Subject = Ticket.TicketSubject,
                Description = Ticket.TicketDescription
            };

            await LoadDropdownDataAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            CanEditTicket = User.IsInRole(RoleTypes.Admin.ToString())
                || User.IsInRole(RoleTypes.Manager.ToString())
                || User.IsInRole(RoleTypes.Employee.ToString());

            if (!CanEditTicket)
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                await LoadDropdownDataAsync();
                return Page();
            }

            var requestorId = LoggedInUser.UserId;
            if (string.IsNullOrWhiteSpace(requestorId))
            {
                return RedirectToPage("/Account/Login");
            }

            RoleTypes requestorRole;
            if (User.IsInRole(RoleTypes.Admin.ToString()))
            {
                requestorRole = RoleTypes.Admin;
            }
            else if (User.IsInRole(RoleTypes.Manager.ToString()))
            {
                requestorRole = RoleTypes.Manager;
            }
            else if (User.IsInRole(RoleTypes.Employee.ToString()))
            {
                requestorRole = RoleTypes.Employee;
            }
            else
            {
                return Forbid();
            }

            var updateDto = new TicketDataDto
            {
                TicketId = Input.TicketId,
                RequestedBy = Input.RequestedBy,
                Location = Input.LocationCode,
                Unit = Input.Unit,
                TicketTroubleType = Input.TicketTypeCode,
                TicketPriority = Input.TicketPriorityCode,
                TicketSubject = Input.Subject,
                TicketDescription = Input.Description
            };

            var updateResult = await _ticketService.UpdateTicket(requestorId, requestorRole, updateDto);
            if (!updateResult.Success)
            {
                ModelState.AddModelError(string.Empty, updateResult.Error ?? "Failed to update ticket.");
                var ticketResult = await _ticketService.GetTicketById(Input.TicketId);
                if (ticketResult.Success && ticketResult.Data != null)
                {
                    Ticket = ticketResult.Data;
                }
                await LoadDropdownDataAsync();
                return Page();
            }

            TempData["SuccessMessage"] = "Ticket updated successfully.";
            return RedirectToPage("./Details", new { id = Input.TicketId });
        }

        private async Task LoadDropdownDataAsync()
        {
            var buildingResult = await _ticketService.GetBuildings();
            Buildings = buildingResult.Success && buildingResult.Data != null
                ? buildingResult.Data
                    .OrderBy(b => b.LocationName)
                    .Select(b => new SelectListItem
                    {
                        Value = b.LocationCode.ToString(),
                        Text = $"{b.LocationName} (#{b.BuildingNumber})"
                    })
                    .ToList()
                : new List<SelectListItem>();

            var ticketTypeResult = await _ticketService.GetTicketTypes();
            TicketTypes = ticketTypeResult.Success && ticketTypeResult.Data != null
                ? ticketTypeResult.Data
                    .OrderBy(t => t.TypeName)
                    .Select(t => new SelectListItem
                    {
                        Value = t.Id.ToString(),
                        Text = t.TypeName
                    })
                    .ToList()
                : new List<SelectListItem>();

            var priorityResult = await _adminService.GetPriorityCodeList();
            Priorities = priorityResult.Success && priorityResult.Data != null
                ? priorityResult.Data
                    .OrderBy(p => p.PriorityCode)
                    .Select(p => new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.PriorityName ?? "Unknown"
                    })
                    .ToList()
                : new List<SelectListItem>();
        }

        public class TicketEditInput
        {
            [Required]
            public Guid TicketId { get; set; }

            [Required]
            public string RequestedBy { get; set; } = string.Empty;

            [Required]
            public int LocationCode { get; set; }

            [Required]
            public int Unit { get; set; }

            [Required]
            public int TicketTypeCode { get; set; }

            [Required]
            public int TicketPriorityCode { get; set; }

            [Required]
            public string Subject { get; set; } = string.Empty;

            [Required]
            public string Description { get; set; } = string.Empty;
        }
    }
}
