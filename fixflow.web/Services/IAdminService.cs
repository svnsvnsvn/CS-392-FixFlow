using fixflow.web.Domain.Enums;
using fixflow.web.Dto;


namespace fixflow.web.Services;

public interface IAdminService
{
    Task<ServiceResult<int>> AddPriorityCode(string _requestorId, RoleTypes _requestorRole, NewPriorityCodeDto _newPriorityData);
    Task<ServiceResult<int>> AddStatusCode(string _requestorId, RoleTypes _requestorRole, NewStatusCodeDto _newStatusData);
    Task<ServiceResult<int>> AddTicketType(string _requestorId, RoleTypes _requestorRole, NewTicketTypeDto _newTicketData);
    Task<ServiceResult<int>> AddBuilding(string _requestorId, RoleTypes _requestorRole, NewBuildingDto _newBuildingData);

    Task<ServiceResult<int>> UpdatePriorityCode(string _requestorId, RoleTypes _requestorRole, UpdatePriorityCodeDto _NewPriorityData);
    Task<ServiceResult<int>> UpdateStatusCode(string _requestorId, RoleTypes _requestorRole, UpdateStatusCodeDto _NewStatusData);
    Task<ServiceResult<int>> UpdateTicketType(string _requestorId, RoleTypes _requestorRole, TicketTypeDto _NewTicketData);
    Task<ServiceResult<int>> UpdateBuilding(string _requestorId, RoleTypes _requestorRole, UpdateBuildingDto _newBuildingData);

    Task<ServiceResult<bool>> ChangeUserRole(string _requestorId, RoleTypes _requestorRole, string _targetId, RoleTypes _targetUserNewRole);
}