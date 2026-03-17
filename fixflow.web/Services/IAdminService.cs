using fixflow.web.Domain.Enums;
using fixflow.web.Dto;


namespace fixflow.web.Services;

public interface IAdminService
{
    Task<ServiceResult<int>> AddPriorityCode(string _requestorId, RoleTypes _requestorRole, NewPriorityCodeDto _newPriorityData);
    Task<ServiceResult<int>> IncrementPriorityCode(string _requestorId, RoleTypes _requestorRole, int _Id);
    Task<ServiceResult<int>> DecrementPriorityCode(string _requestorId, RoleTypes _requestorRole, int _Id);
    Task<ServiceResult<bool>> DeletePriorityCode(string _requestorId, RoleTypes _requestorRole, int _Id);

    Task<ServiceResult<int>> AddStatusCode(string _requestorId, RoleTypes _requestorRole, NewStatusCodeDto _newStatusData);
    Task<ServiceResult<int>> IncrementStatusCode(string _requestorId, RoleTypes _requestorRole, int _Id);
    Task<ServiceResult<int>> DecrementStatusCode(string _requestorId, RoleTypes _requestorRole, int _Id);
    Task<ServiceResult<bool>> DeleteStatusCode(string _requestorId, RoleTypes _requestorRole, int _Id);

    Task<ServiceResult<int>> AddTicketType(string _requestorId, RoleTypes _requestorRole, NewTicketTypeDto _newTicketData);
    Task<ServiceResult<int>> UpdateTicketType(string _requestorId, RoleTypes _requestorRole, TicketTypeDto _NewTicketData);
    Task<ServiceResult<int>> DeleteTicketType(string _requestorId, RoleTypes _requestorRole, int _Id);

    Task<ServiceResult<int>> AddBuilding(string _requestorId, RoleTypes _requestorRole, NewBuildingDto _newBuildingData);
    Task<ServiceResult<int>> UpdateBuilding(string _requestorId, RoleTypes _requestorRole, BuildingDto _newBuildingData);

    Task<ServiceResult<bool>> ChangeUserRole(string _requestorId, RoleTypes _requestorRole, string _targetId, RoleTypes _targetUserNewRole);
    Task<ServiceResult<List<UserListItemDto>>> SearchUsers(string _searchString);
    Task<ServiceResult<UserSettingsListItemDTO>> GetUserSettings(string _requestorId, RoleTypes _requestorRole, string _targetUser);
}