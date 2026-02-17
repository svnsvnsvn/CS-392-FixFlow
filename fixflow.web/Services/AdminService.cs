using fixflow.web.Data;
using fixflow.web.Dto;
using fixflow.web.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace fixflow.web.Services
{
    public class AdminService : IAdminService
    {
        private readonly FfDbContext _db;
        private readonly UserManager<AppUser> _userManager;

        public AdminService(FfDbContext db, UserManager<AppUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }


        public async Task<ServiceResult<int>> AddPriorityCode(string _requestorId, RoleTypes _requestorRole, NewPriorityCodeDto _newPriorityData)
        {
            try
            {
                // Validate requestor inputs
                if (_requestorId == null)
                {
                    return ServiceResult<int>.Fail("Invalid requestor Id.");
                }

                if (!Enum.IsDefined(typeof(RoleTypes), _requestorRole))
                {
                    return ServiceResult<int>.Fail("Invalid role.");
                }

                if (_requestorRole != RoleTypes.Admin)
                {
                    return ServiceResult<int>.Fail("Insufficient privileges.");
                }

                var newPriorityCode = new FfPriorityCodes
                {
                    PriorityName = _newPriorityData.PriorityName,
                    Code = _newPriorityData.PriorityCode
                };

                _db.FfPriorityCodess.Add(newPriorityCode);
                await _db.SaveChangesAsync();

                return ServiceResult<int>.Ok(newPriorityCode.Code);
            }
            catch (Exception ex)
            {
                return ServiceResult<int>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<int>> AddStatusCode(string _requestorId, RoleTypes _requestorRole, NewStatusCodeDto _newStatusData)
        {
            try
            {
                // Validate requestor inputs
                if (_requestorId == null)
                {
                    return ServiceResult<int>.Fail("Invalid requestor Id.");
                }

                if (!Enum.IsDefined(typeof(RoleTypes), _requestorRole))
                {
                    return ServiceResult<int>.Fail("Invalid role.");
                }

                if (_requestorRole != RoleTypes.Admin)
                {
                    return ServiceResult<int>.Fail("Insufficient privileges.");
                }

                var newStatusCode = new FfStatusCodes
                {
                    StatusName = _newStatusData.StatusName,
                    Code = _newStatusData.StatusCode
                };

                _db.FfStatusCodes.Add(newStatusCode);
                await _db.SaveChangesAsync();

                return ServiceResult<int>.Ok(newStatusCode.Code);
            }
            catch (Exception ex)
            {
                return ServiceResult<int>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<int>> AddTicketType(string _requestorId, RoleTypes _requestorRole, NewTicketTypeDto _newTicketData)
        {
            try
            {
                // Validate requestor inputs
                if (_requestorId == null)
                {
                    return ServiceResult<int>.Fail("Invalid requestor Id.");
                }

                if (!Enum.IsDefined(typeof(RoleTypes), _requestorRole))
                {
                    return ServiceResult<int>.Fail("Invalid role.");
                }

                if (_requestorRole != RoleTypes.Admin)
                {
                    return ServiceResult<int>.Fail("Insufficient privileges.");
                }

                var newTicketType = new FfTicketTypes
                {
                    TypeName = _newTicketData.TypeName,
                };

                _db.FfTicketTypess.Add(newTicketType);
                await _db.SaveChangesAsync();

                return ServiceResult<int>.Ok(newTicketType.Code);
            }
            catch (Exception ex)
            {
                return ServiceResult<int>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<int>> AddBuilding(string _requestorId, RoleTypes _requestorRole, NewBuildingDto _newBuildingData)
        {
            try
            {
                // Prevent Location name of "Unassigned" this is a protected location name sued as a default.
                if (_newBuildingData.LocationName == "Unassigned")
                {
                    return ServiceResult<int>.Fail("'Unassigned' is a protected building name and not for use.");
                }
                
                // Validate requestor inputs
                if (_requestorId == null)
                {
                    return ServiceResult<int>.Fail("Invalid requestor Id.");
                }

                if (!Enum.IsDefined(typeof(RoleTypes), _requestorRole))
                {
                    return ServiceResult<int>.Fail("Invalid role.");
                }

                if (_requestorRole != RoleTypes.Admin)
                {
                    return ServiceResult<int>.Fail("Insufficient privileges.");
                }

                var newBuilding = new FfBuildingDirectory
                {
                    LocationName = _newBuildingData.LocationName,
                    ComplexName = _newBuildingData.ComplexName,
                    BuildingNumber = _newBuildingData.BuildingNumber,
                    NumUnits = _newBuildingData.NumUnits,
                    LocationLat = _newBuildingData.LocationLat,
                    LocationLon = _newBuildingData.LocationLon
                };

                _db.FfBuildingDirectorys.Add(newBuilding);
                await _db.SaveChangesAsync();

                return ServiceResult<int>.Ok(newBuilding.LocationCode);
            }
            catch (Exception ex)
            {
                return ServiceResult<int>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<int>> UpdatePriorityCode(string _requestorId, RoleTypes _requestorRole, UpdatePriorityCodeDto _newPriorityData)
        {
            try
            {
                // Validate requestor inputs
                if (_requestorId == null)
                {
                    return ServiceResult<int>.Fail("Invalid requestor Id.");
                }

                if (!Enum.IsDefined(typeof(RoleTypes), _requestorRole))
                {
                    return ServiceResult<int>.Fail("Invalid role.");
                }

                if (_requestorRole != RoleTypes.Admin)
                {
                    return ServiceResult<int>.Fail("Insufficient privileges.");
                }

                var newPriorityCode = new FfPriorityCodes
                {
                    PriorityName = _newPriorityData.PriorityName,
                    Code = _newPriorityData.PriorityCode
                };

                _db.FfPriorityCodess.Add(newPriorityCode);
                await _db.SaveChangesAsync();

                return ServiceResult<int>.Ok(newPriorityCode.Code);
            }
            catch (Exception ex)
            {
                return ServiceResult<int>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<int>> UpdateStatusCode(string _requestorId, RoleTypes _requestorRole, UpdateStatusCodeDto _newStatusData)
        {
            try
            {
                // Validate requestor inputs
                if (_requestorId == null)
                {
                    return ServiceResult<int>.Fail("Invalid requestor Id.");
                }

                if (!Enum.IsDefined(typeof(RoleTypes), _requestorRole))
                {
                    return ServiceResult<int>.Fail("Invalid role.");
                }

                if (_requestorRole != RoleTypes.Admin)
                {
                    return ServiceResult<int>.Fail("Insufficient privileges.");
                }

                var newStatusCode = new FfStatusCodes
                {
                    StatusName = _newStatusData.StatusName,
                    Code = _newStatusData.StatusCode
                };

                _db.FfStatusCodes.Add(newStatusCode);
                await _db.SaveChangesAsync();

                return ServiceResult<int>.Ok(newStatusCode.Code);
            }
            catch (Exception ex)
            {
                return ServiceResult<int>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<int>> UpdateTicketType(string _requestorId, RoleTypes _requestorRole, UpdateTicketTypeDto _newTicketData)
        {
            try
            {
                // Validate requestor inputs
                if (_requestorId == null)
                {
                    return ServiceResult<int>.Fail("Invalid requestor Id.");
                }

                if (!Enum.IsDefined(typeof(RoleTypes), _requestorRole))
                {
                    return ServiceResult<int>.Fail("Invalid role.");
                }

                if (_requestorRole != RoleTypes.Admin)
                {
                    return ServiceResult<int>.Fail("Insufficient privileges.");
                }

                var newTicketType = new FfTicketTypes
                {
                    TypeName = _newTicketData.TypeName,
                };

                _db.FfTicketTypess.Add(newTicketType);
                await _db.SaveChangesAsync();

                return ServiceResult<int>.Ok(newTicketType.Code);
            }
            catch (Exception ex)
            {
                return ServiceResult<int>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<int>> UpdateBuilding(string _requestorId, RoleTypes _requestorRole, UpdateBuildingDto _newBuildingData)
        {
            try
            {
                // Prevent Location name of "Unassigned" this is a protected location name sued as a default.
                if (_newBuildingData.LocationName == "Unassigned")
                {
                    return ServiceResult<int>.Fail("'Unassigned' is a protected building name and not for use.");
                }

                // Validate requestor inputs
                if (_requestorId == null)
                {
                    return ServiceResult<int>.Fail("Invalid requestor Id.");
                }

                if (!Enum.IsDefined(typeof(RoleTypes), _requestorRole))
                {
                    return ServiceResult<int>.Fail("Invalid role.");
                }

                if (_requestorRole != RoleTypes.Admin)
                {
                    return ServiceResult<int>.Fail("Insufficient privileges.");
                }

                var newBuilding = new FfBuildingDirectory
                {
                    LocationName = _newBuildingData.LocationName,
                    ComplexName = _newBuildingData.ComplexName,
                    BuildingNumber = _newBuildingData.BuildingNumber,
                    NumUnits = _newBuildingData.NumUnits,
                    LocationLat = _newBuildingData.LocationLat ?? 0m,
                    LocationLon = _newBuildingData.LocationLon ?? 0m
                };

                _db.FfBuildingDirectorys.Add(newBuilding);
                await _db.SaveChangesAsync();

                return ServiceResult<int>.Ok(newBuilding.LocationCode);
            }
            catch (Exception ex)
            {
                return ServiceResult<int>.Fail(ex.Message);
            }
        }
    }
}
