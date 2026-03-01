using fixflow.web.Domain.Enums;
using fixflow.web.Dto;

namespace fixflow.web.Services

{
    public interface ITicketService
    {
        Task<ServiceResult<Guid>> AddNewTicket(string _requestorId, RoleTypes _requestorRole, NewTicketDto _newTicketData);
        Task<ServiceResult<bool>> UpdateTicket(string _requestorId, RoleTypes _requestorRole, UpdateTicketDto _updateTicketData);
        Task<ServiceResult<long>> ReassignTicket(string _requestorId, RoleTypes _requestorRole, Guid _ticketIdToUpdate, string _newAssigneeId, int _newStatus);
        Task<ServiceResult<List<TicketTypeDto>>> GetTicketTypes();

    }
}
