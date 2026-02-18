using fixflow.web.Data;
using fixflow.web.Dto;

namespace fixflow.web.Services
{
    public interface ITicketService
    {
        Task<List<FfTicketRegister>> GetTicketsForListAsync();
        Task<Dictionary<Guid, string>> GetLatestAssigneesAsync(IReadOnlyCollection<Guid> ticketIds);
        Task<ServiceResult<TicketCreateResult>> CreateTicketAsync(string userId, bool isStaff, TicketCreateRequest request);
        Task<ServiceResult<bool>> AssignTicketAsync(Guid ticketId, string assigneeId);
    }
}
