using fixflow.web.Domain.Enums;
using fixflow.web.Dto;

namespace fixflow.web.Services

{
    public interface ITicketService
    {
        Task<ServiceResult<Guid>> AddNewTicket(string _requestorId, RoleTypes _requestorRole, NewTicketDto _newTicketData);
    }
}
