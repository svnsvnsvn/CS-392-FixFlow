using fixflow.web.Domain.Enums;
using fixflow.web.Dto;

namespace fixflow.web.Services

{
    // TODO (Adam): Add read/query APIs for ticket lists (dashboard, /Tickets/List, details) so page models
    // do not inject FfDbContext. Today those pages query FfTicketRegisters, FfTicketFlows, FfUserProfiles, etc. directly.
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
        Task<ServiceResult<StatusCodeDto>> GetStatusCodeFromId(int _Id);
        Task<ServiceResult<int>> GetPriorityCode(string _StatusName);
        Task<ServiceResult<string>> GetPriorityCode(int _StatusCode);
        Task<ServiceResult<PriorityCodeDto>> GetPriorityCodeFromId(int _Id);


        
        // Stuff I need to write.
        //Task<ServiceResult<TicketDTO>> GetTicketsByRequestor(string _RequestorId);
        //Task<ServiceResult<TicketDTO>> GetTicketsByAssignee(string _AssigneeId);
        //Task<ServiceResult<TicketDTO>> GetTicketHistory(Guid _TicketId);


    }
}
