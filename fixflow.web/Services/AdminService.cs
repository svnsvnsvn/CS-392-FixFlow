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

        private ServiceResult<T> ValidateAdminAccess<T>(string requestorId, RoleTypes requestorRole)
        {
            if (requestorId == null)
            {
                return ServiceResult<T>.Fail("Invalid requestor Id.");
            }

            if (!Enum.IsDefined(typeof(RoleTypes), requestorRole))
            {
                return ServiceResult<T>.Fail("Invalid role.");
            }

            if (requestorRole != RoleTypes.Admin)
            {
                return ServiceResult<T>.Fail("Insufficient privileges.");
            }

            return null; // Validation passed
        }


        public async Task<ServiceResult<int>> AddPriorityCode(string requestorId, RoleTypes requestorRole, NewPriorityCodeDto newPriorityData)
        {
            try
            {
                var validationResult = ValidateAdminAccess<int>(requestorId, requestorRole);
                if (validationResult != null)
                {
                    return validationResult;
                }

                var newPriorityCode = new FfPriorityCodes
                {
                    PriorityName = newPriorityData.PriorityName,
                    Code = newPriorityData.PriorityCode
                };

                _db.FfPriorityCodes.Add(newPriorityCode);
                await _db.SaveChangesAsync();

                return ServiceResult<int>.Ok(newPriorityCode.Code);
            }
            catch (Exception ex)
            {
                return ServiceResult<int>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<int>> AddStatusCode(string requestorId, RoleTypes requestorRole, NewStatusCodeDto newStatusData)
        {
            try
            {
                var validationResult = ValidateAdminAccess<int>(requestorId, requestorRole);
                if (validationResult != null)
                {
                    return validationResult;
                }

                var newStatusCode = new FfStatusCodes
                {
                    StatusName = newStatusData.StatusName,
                    Code = newStatusData.StatusCode
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

        public async Task<ServiceResult<int>> AddTicketType(string requestorId, RoleTypes requestorRole, NewTicketTypeDto newTicketData)
        {
            try
            {
                var validationResult = ValidateAdminAccess<int>(requestorId, requestorRole);
                if (validationResult != null)
                {
                    return validationResult;
                }

                var newTicketType = new FfTicketTypes
                {
                    TypeName = newTicketData.TypeName,
                };

                _db.FfTicketTypes.Add(newTicketType);
                await _db.SaveChangesAsync();

                return ServiceResult<int>.Ok(newTicketType.Code);
            }
            catch (Exception ex)
            {
                return ServiceResult<int>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<int>> AddBuilding(string requestorId, RoleTypes requestorRole, NewBuildingDto newBuildingData)
        {
            try
            {
                // Prevent Location name of "Unassigned" this is a protected location name used as a default.
                if (newBuildingData.LocationName == "Unassigned")
                {
                    return ServiceResult<int>.Fail("'Unassigned' is a protected building name and not for use.");
                }
                
                var validationResult = ValidateAdminAccess<int>(requestorId, requestorRole);
                if (validationResult != null)
                {
                    return validationResult;
                }

                var newBuilding = new FfBuildingDirectory
                {
                    LocationName = newBuildingData.LocationName,
                    ComplexName = newBuildingData.ComplexName,
                    BuildingNumber = newBuildingData.BuildingNumber,
                    NumUnits = newBuildingData.NumUnits,
                    LocationLat = newBuildingData.LocationLat,
                    LocationLon = newBuildingData.LocationLon
                };

                _db.FfBuildingDirectories.Add(newBuilding);
                await _db.SaveChangesAsync();

                return ServiceResult<int>.Ok(newBuilding.LocationCode);
            }
            catch (Exception ex)
            {
                return ServiceResult<int>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<int>> UpdatePriorityCode(string requestorId, RoleTypes requestorRole, UpdatePriorityCodeDto updatePriorityData)
        {
            try
            {
                var validationResult = ValidateAdminAccess<int>(requestorId, requestorRole);
                if (validationResult != null)
                {
                    return validationResult;
                }

                var existingPriorityCode = await _db.FfPriorityCodes.FindAsync(updatePriorityData.PriorityCode);
                if (existingPriorityCode == null)
                {
                    return ServiceResult<int>.Fail($"Priority code {updatePriorityData.PriorityCode} not found.");
                }

                existingPriorityCode.PriorityName = updatePriorityData.PriorityName;
                await _db.SaveChangesAsync();

                return ServiceResult<int>.Ok(existingPriorityCode.Code);
            }
            catch (Exception ex)
            {
                return ServiceResult<int>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<int>> UpdateStatusCode(string requestorId, RoleTypes requestorRole, UpdateStatusCodeDto updateStatusData)
        {
            try
            {
                var validationResult = ValidateAdminAccess<int>(requestorId, requestorRole);
                if (validationResult != null)
                {
                    return validationResult;
                }

                var existingStatusCode = await _db.FfStatusCodes.FindAsync(updateStatusData.StatusCode);
                if (existingStatusCode == null)
                {
                    return ServiceResult<int>.Fail($"Status code {updateStatusData.StatusCode} not found.");
                }

                existingStatusCode.StatusName = updateStatusData.StatusName;
                await _db.SaveChangesAsync();

                return ServiceResult<int>.Ok(existingStatusCode.Code);
            }
            catch (Exception ex)
            {
                return ServiceResult<int>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<int>> UpdateTicketType(string requestorId, RoleTypes requestorRole, UpdateTicketTypeDto updateTicketData)
        {
            try
            {
                var validationResult = ValidateAdminAccess<int>(requestorId, requestorRole);
                if (validationResult != null)
                {
                    return validationResult;
                }

                var existingTicketType = await _db.FfTicketTypes.FindAsync(updateTicketData.Code);
                if (existingTicketType == null)
                {
                    return ServiceResult<int>.Fail($"Ticket type {updateTicketData.Code} not found.");
                }

                existingTicketType.TypeName = updateTicketData.TypeName;
                await _db.SaveChangesAsync();

                return ServiceResult<int>.Ok(existingTicketType.Code);
            }
            catch (Exception ex)
            {
                return ServiceResult<int>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<int>> UpdateBuilding(string requestorId, RoleTypes requestorRole, UpdateBuildingDto updateBuildingData)
        {
            try
            {
                // Prevent Location name of "Unassigned" this is a protected location name used as a default.
                if (updateBuildingData.LocationName == "Unassigned")
                {
                    return ServiceResult<int>.Fail("'Unassigned' is a protected building name and not for use.");
                }

                var validationResult = ValidateAdminAccess<int>(requestorId, requestorRole);
                if (validationResult != null)
                {
                    return validationResult;
                }

                var existingBuilding = await _db.FfBuildingDirectories.FindAsync(updateBuildingData.LocationCode);
                if (existingBuilding == null)
                {
                    return ServiceResult<int>.Fail($"Building {updateBuildingData.LocationCode} not found.");
                }

                existingBuilding.LocationName = updateBuildingData.LocationName;
                existingBuilding.ComplexName = updateBuildingData.ComplexName;
                existingBuilding.BuildingNumber = updateBuildingData.BuildingNumber;
                existingBuilding.NumUnits = updateBuildingData.NumUnits;
                existingBuilding.LocationLat = updateBuildingData.LocationLat ?? existingBuilding.LocationLat;
                existingBuilding.LocationLon = updateBuildingData.LocationLon ?? existingBuilding.LocationLon;

                await _db.SaveChangesAsync();

                return ServiceResult<int>.Ok(existingBuilding.LocationCode);
            }
            catch (Exception ex)
            {
                return ServiceResult<int>.Fail(ex.Message);
            }
        }
    }
}
