<<<<<<< HEAD
﻿using fixflow.web.Domain.Enums;
=======
﻿using fixflow.web.Data;
>>>>>>> origin/ZZ-development/dashboard-refresh
using fixflow.web.Dto;

namespace fixflow.web.Services
{
    public interface ITicketService
    {
<<<<<<< HEAD
        Task<ServiceResult<Guid>> AddNewTicket(string _requestorId, RoleTypes _requestorRole, NewTicketDto _newTicketData);
        Task<ServiceResult<bool>> UpdateTicket(string _requestorId, RoleTypes _requestorRole, UpdateTicketDto _updateTicketData);
        Task<ServiceResult<long>> ReassignTicket(string _requestorId, RoleTypes _requestorRole, Guid _ticketIdToUpdate, string _newAssigneeId, int _newStatus);
        
=======
        Task<List<FfTicketRegister>> GetTicketsForListAsync();
        Task<Dictionary<Guid, string>> GetLatestAssigneesAsync(IReadOnlyCollection<Guid> ticketIds);
        Task<ServiceResult<TicketCreateResult>> CreateTicketAsync(string userId, bool isStaff, TicketCreateRequest request);
        Task<ServiceResult<bool>> AssignTicketAsync(Guid ticketId, string assigneeId);
>>>>>>> origin/ZZ-development/dashboard-refresh
    }
}
