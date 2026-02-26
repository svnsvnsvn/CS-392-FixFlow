using fixflow.web.Domain.Enums;
using fixflow.web.Dto;


namespace fixflow.web.Services;

public interface IAdminService
{
    Task<ServiceResult<int>> AddPriorityCode(string requestorId, RoleTypes requestorRole, NewPriorityCodeDto newPriorityData);
    Task<ServiceResult<int>> AddStatusCode(string requestorId, RoleTypes requestorRole, NewStatusCodeDto newStatusData);
    Task<ServiceResult<int>> AddTicketType(string requestorId, RoleTypes requestorRole, NewTicketTypeDto newTicketData);
    Task<ServiceResult<int>> AddBuilding(string requestorId, RoleTypes requestorRole, NewBuildingDto newBuildingData);
    
<<<<<<< HEAD
    Task<ServiceResult<int>> UpdatePriorityCode(string _requestorId, RoleTypes _requestorRole, UpdatePriorityCodeDto _NewPriorityData);
    Task<ServiceResult<int>> UpdateStatusCode(string _requestorId, RoleTypes _requestorRole, UpdateStatusCodeDto _NewStatusData);
    Task<ServiceResult<int>> UpdateTicketType(string _requestorId, RoleTypes _requestorRole, UpdateTicketTypeDto _NewTicketData);
    Task<ServiceResult<int>> UpdateBuilding(string _requestorId, RoleTypes _requestorRole, UpdateBuildingDto _newBuildingData);

    Task<ServiceResult<bool>> ChangeUserRole (string _requestorId, RoleTypes _requestorRole, string _targetId, RoleTypes _targetUserNewRole);
=======
    Task<ServiceResult<int>> UpdatePriorityCode(string requestorId, RoleTypes requestorRole, UpdatePriorityCodeDto updatePriorityData);
    Task<ServiceResult<int>> UpdateStatusCode(string requestorId, RoleTypes requestorRole, UpdateStatusCodeDto updateStatusData);
    Task<ServiceResult<int>> UpdateTicketType(string requestorId, RoleTypes requestorRole, UpdateTicketTypeDto updateTicketData);
    Task<ServiceResult<int>> UpdateBuilding(string requestorId, RoleTypes requestorRole, UpdateBuildingDto updateBuildingData);
>>>>>>> origin/ZZ-development/dashboard-refresh
}
