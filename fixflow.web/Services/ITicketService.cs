using fixflow.web.Domain.Enums;
using fixflow.web.Dto;

namespace fixflow.web.Services

{
    public interface ITicketService
    {
        Task<ServiceResult<Guid>> AddNewTicket(string _requestorId, RoleTypes _requestorRole, NewTicketDto _newTicketData);
        Task<ServiceResult<bool>> UpdateTicket(string _requestorId, RoleTypes _requestorRole, UpdateTicketDto _updateTicketData);
        Task<ServiceResult<long>> ReassignTicket(string _requestorId, RoleTypes _requestorRole, Guid _ticketIdToUpdate, string _newAssigneeId, int _newStatus);
        Task<ServiceResult<string>> GetNextShortCode();
        Task<ServiceResult<List<TicketTypeDto>>> GetTicketTypes();
        Task<ServiceResult<List<BuildingDto>>> GetBuildings();
        Task<ServiceResult<List<StatusCodeDto>>> GetStatusCodeList();
        Task<ServiceResult<int>> GetStatusCode(string _StatusName);
        Task<ServiceResult<string>> GetStatusCode(int _StatusCode);
    }
}
