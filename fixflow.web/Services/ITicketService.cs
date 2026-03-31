using fixflow.web.Domain.Enums;
using fixflow.web.Dto;
using fixflow.web.Data;

namespace fixflow.web.Services

{
    // TODO (Adam): Add read/query APIs for ticket lists (dashboard, /Tickets/List, details) so page models
    // do not inject FfDbContext. Today those pages query FfTicketRegisters, FfTicketFlows, FfUserProfiles, etc. directly.
    public interface ITicketService
    {
        Task<ServiceResult<Guid>> AddNewTicket(string _requestorId, RoleTypes _requestorRole, NewTicketDto _newTicketData);
        Task<ServiceResult<bool>> UpdateTicket(string _requestorId, RoleTypes _requestorRole, TicketDataDto _updateTicketData);
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
        Task<ServiceResult<List<TicketDataDto>>> GetTicketsByRequestor(string _RequestorId);
        Task<ServiceResult<List<TicketDataDto>>> GetTicketsByAssignee(string _AssigneeId);
        Task<ServiceResult<List<TicketHistoryItemDto>>> GetTicketHistory(Guid _TicketId);
        Task<ServiceResult<FfTicketRegister>> GetTicketById(Guid ticketId);
        Task<ServiceResult<FfTicketRegister>> GetTicketByIdentifier(string ticketIdOrCode);
        Task<ServiceResult<List<FfTicketFlow>>> GetTicketFlows(Guid ticketId);
        Task<ServiceResult<Dictionary<int, string>>> GetStatusCodeNameMap();
        Task<ServiceResult<List<FfExternalNotes>>> GetExternalNotes(Guid ticketId);
        Task<ServiceResult<List<FfInternalNotes>>> GetInternalNotes(Guid ticketId);
        Task<ServiceResult<TicketQueryBundleDto>> GetTicketListBundle();
        Task<ServiceResult<TicketQueryBundleDto>> GetDashboardBundle(string requestorId, RoleTypes requestorRole);


    }
}
