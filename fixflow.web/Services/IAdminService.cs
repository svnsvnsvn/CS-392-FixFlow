using fixflow.web.Domain.Enums;
using fixflow.web.Dto;


namespace fixflow.web.Services;

public interface IAdminService
{
    Task<ServiceResult<int>> AddPriorityCode(string requestorId, RoleTypes requestorRole, NewPriorityCodeDto newPriorityData);
    Task<ServiceResult<int>> AddStatusCode(string requestorId, RoleTypes requestorRole, NewStatusCodeDto newStatusData);
    Task<ServiceResult<int>> AddTicketType(string requestorId, RoleTypes requestorRole, NewTicketTypeDto newTicketData);
    Task<ServiceResult<int>> AddBuilding(string requestorId, RoleTypes requestorRole, NewBuildingDto newBuildingData);
    
    Task<ServiceResult<int>> UpdatePriorityCode(string requestorId, RoleTypes requestorRole, UpdatePriorityCodeDto updatePriorityData);
    Task<ServiceResult<int>> UpdateStatusCode(string requestorId, RoleTypes requestorRole, UpdateStatusCodeDto updateStatusData);
    Task<ServiceResult<int>> UpdateTicketType(string requestorId, RoleTypes requestorRole, UpdateTicketTypeDto updateTicketData);
    Task<ServiceResult<int>> UpdateBuilding(string requestorId, RoleTypes requestorRole, UpdateBuildingDto updateBuildingData);
}
