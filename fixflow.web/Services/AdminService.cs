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

<<<<<<< HEAD
        public async Task<ServiceResult<int>> AddPriorityCode(string _requestorId, RoleTypes _requestorRole, NewPriorityCodeDto _newPriorityData)
=======
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
>>>>>>> origin/ZZ-development/dashboard-refresh
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
<<<<<<< HEAD
                    LocationName = _newBuildingData.LocationName,
                    ComplexName = _newBuildingData.ComplexName,
                    BuildingNumber = _newBuildingData.BuildingNumber,
                    NumUnits = (int)_newBuildingData.NumUnits,
                    LocationLat = (decimal)_newBuildingData.LocationLat,
                    LocationLon = (decimal)_newBuildingData.LocationLon
=======
                    LocationName = newBuildingData.LocationName,
                    ComplexName = newBuildingData.ComplexName,
                    BuildingNumber = newBuildingData.BuildingNumber,
                    NumUnits = newBuildingData.NumUnits,
                    LocationLat = newBuildingData.LocationLat,
                    LocationLon = newBuildingData.LocationLon
>>>>>>> origin/ZZ-development/dashboard-refresh
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

<<<<<<< HEAD
        public async Task<ServiceResult<int>> UpdatePriorityCode(string _requestorId, RoleTypes _requestorRole, UpdatePriorityCodeDto _updatedPriorityData)
=======
        public async Task<ServiceResult<int>> UpdatePriorityCode(string requestorId, RoleTypes requestorRole, UpdatePriorityCodeDto updatePriorityData)
>>>>>>> origin/ZZ-development/dashboard-refresh
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

<<<<<<< HEAD
                if (_requestorRole != RoleTypes.Admin)
                {
                    return ServiceResult<int>.Fail("Insufficient privileges.");
                }

                if ((_updatedPriorityData.PriorityCode != null) && (_updatedPriorityData.PriorityCode < 0))  // If not null, new value is negative and not valid
                {
                    return ServiceResult<int>.Fail("Invalid priority code.");
                }


                var existingRecord = await _db.FfPriorityCodess.FindAsync(_updatedPriorityData.Id);
                if (existingRecord != null)
                {
                    if (_updatedPriorityData.PriorityCode != null)        // New value is not null
                    {
                        existingRecord.PriorityCode = (int)_updatedPriorityData.PriorityCode;
                    }

                    if (_updatedPriorityData.PriorityName != null)
                    {
                        existingRecord.PriorityName = _updatedPriorityData.PriorityName;
                    }
                    await _db.SaveChangesAsync();

                    return ServiceResult<int>.Ok(existingRecord.Id);
                }
                else 
                {
                    return ServiceResult<int>.Fail("Priority ID not found.");
                }
=======
                existingPriorityCode.PriorityName = updatePriorityData.PriorityName;
                await _db.SaveChangesAsync();

                return ServiceResult<int>.Ok(existingPriorityCode.Code);
>>>>>>> origin/ZZ-development/dashboard-refresh
            }
            catch (Exception ex)
            {
                return ServiceResult<int>.Fail(ex.Message);
            }
        }

<<<<<<< HEAD
        public async Task<ServiceResult<int>> UpdateStatusCode(string _requestorId, RoleTypes _requestorRole, UpdateStatusCodeDto _updatedStatusData)
=======
        public async Task<ServiceResult<int>> UpdateStatusCode(string requestorId, RoleTypes requestorRole, UpdateStatusCodeDto updateStatusData)
>>>>>>> origin/ZZ-development/dashboard-refresh
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

<<<<<<< HEAD
                if (_requestorRole != RoleTypes.Admin)
                {
                    return ServiceResult<int>.Fail("Insufficient privileges.");
                }

                if ((_updatedStatusData.StatusCode != null) && (_updatedStatusData.StatusCode < 0))  // If not null, new value is negative and not valid
                {
                    return ServiceResult<int>.Fail("Invalid status code.");
                }

                var existingRecord = await _db.FfStatusCodes.FindAsync(_updatedStatusData.Id);
                if (existingRecord != null)
                {
                    if (_updatedStatusData.StatusCode != null)        // New value is not null
                    {
                        existingRecord.StatusCode = (int)_updatedStatusData.StatusCode;
                    }

                    if (_updatedStatusData.StatusName != null)
                    {
                        existingRecord.StatusName = _updatedStatusData.StatusName;
                    }
                    await _db.SaveChangesAsync();

                    return ServiceResult<int>.Ok(existingRecord.Id);
                }
                else
                {
                    return ServiceResult<int>.Fail("Status ID not found.");
                }
=======
                existingStatusCode.StatusName = updateStatusData.StatusName;
                await _db.SaveChangesAsync();

                return ServiceResult<int>.Ok(existingStatusCode.Code);
>>>>>>> origin/ZZ-development/dashboard-refresh
            }
            catch (Exception ex)
            {
                return ServiceResult<int>.Fail(ex.Message);
            }
        }

<<<<<<< HEAD
        public async Task<ServiceResult<int>> UpdateTicketType(string _requestorId, RoleTypes _requestorRole, UpdateTicketTypeDto _updatedTicketData)
=======
        public async Task<ServiceResult<int>> UpdateTicketType(string requestorId, RoleTypes requestorRole, UpdateTicketTypeDto updateTicketData)
>>>>>>> origin/ZZ-development/dashboard-refresh
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

<<<<<<< HEAD
                if (_requestorRole != RoleTypes.Admin)
                {
                    return ServiceResult<int>.Fail("Insufficient privileges.");
                }

                var existingRecord = await _db.FfTicketTypess.FindAsync(_updatedTicketData.Id);
                if (existingRecord != null)
                {
                    if (_updatedTicketData.TypeName != null)        // New value is not null
                    {
                        existingRecord.TypeName = _updatedTicketData.TypeName;
                    }
                    else   // Typename provided is null, delete record
                    {
                        _db.FfTicketTypess.Remove(existingRecord);
                    }
                    
                    await _db.SaveChangesAsync();

                    return ServiceResult<int>.Ok(1);
                }
                else
                {
                    return ServiceResult<int>.Fail("Status ID not found.");
                }
=======
                existingTicketType.TypeName = updateTicketData.TypeName;
                await _db.SaveChangesAsync();

                return ServiceResult<int>.Ok(existingTicketType.Code);
>>>>>>> origin/ZZ-development/dashboard-refresh
            }
            catch (Exception ex)
            {
                return ServiceResult<int>.Fail(ex.Message);
            }
        }

<<<<<<< HEAD
        public async Task<ServiceResult<int>> UpdateBuilding(string _requestorId, RoleTypes _requestorRole, UpdateBuildingDto _updatedBuildingData)
        {
            try
            {
                // Prevent Location name of "Unassigned" this is a protected location name sued as a default.
                if (_updatedBuildingData.LocationName == "Unassigned")
=======
        public async Task<ServiceResult<int>> UpdateBuilding(string requestorId, RoleTypes requestorRole, UpdateBuildingDto updateBuildingData)
        {
            try
            {
                // Prevent Location name of "Unassigned" this is a protected location name used as a default.
                if (updateBuildingData.LocationName == "Unassigned")
>>>>>>> origin/ZZ-development/dashboard-refresh
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

<<<<<<< HEAD
                var existingRecord = await _db.FfBuildingDirectorys.FindAsync(_updatedBuildingData.LocationCode);
                if (existingRecord != null)
                {
                    existingRecord.LocationName = _updatedBuildingData.LocationName;
                    existingRecord.ComplexName = _updatedBuildingData.ComplexName;
                    
                    if (_updatedBuildingData.BuildingNumber != null)
                    {
                        if (_updatedBuildingData.BuildingNumber >= 0)
                        {
                            existingRecord.BuildingNumber = (int)_updatedBuildingData.BuildingNumber;
                        }
                        else  // Negative Number. No no.
                        {
                            return ServiceResult<int>.Fail("Invalid building number.");
                        }
                    }
                    else  // Null = 0
                    {
                        existingRecord.BuildingNumber = (int)0;
                    }

                    if (_updatedBuildingData.NumUnits != null)
                    {
                        if (_updatedBuildingData.NumUnits >= 0)
                        {
                            existingRecord.NumUnits = (int)_updatedBuildingData.NumUnits;
                        }
                        else  // Negative Number. No no.
                        {
                            return ServiceResult<int>.Fail("Invalid number of units.");
                        }
                    }
                    else  // Null = 0
                    {
                        existingRecord.NumUnits = (int)0;
                    }

                    if (_updatedBuildingData.BuildingNumber != null)
                    {
                        if (_updatedBuildingData.BuildingNumber >= 0)
                        {
                            existingRecord.BuildingNumber = (int)_updatedBuildingData.BuildingNumber;
                        }
                        else  // Negative Number. No no.
                        {
                            return ServiceResult<int>.Fail("Invalid building number.");
                        }
                    }
                    else  // Null = 0
                    {
                        existingRecord.BuildingNumber = (int)0;
                    }

                    if (_updatedBuildingData.LocationLat != null)
                    {
                        if ((_updatedBuildingData.LocationLat >= -90) && (_updatedBuildingData.LocationLat <= 90))
                        {
                            existingRecord.LocationLat = (decimal)_updatedBuildingData.LocationLat;
                        }
                        else  // Out of range. No no.
                        {
                            return ServiceResult<int>.Fail("Invalid Latitude.");
                        }
                    }
                    else  // Null = 0
                    {
                        existingRecord.LocationLat = (decimal)0;
                    }

                    if (_updatedBuildingData.LocationLon != null)
                    {
                        if ((_updatedBuildingData.LocationLon >= -180) && (_updatedBuildingData.LocationLon <= 180))
                        {
                            existingRecord.LocationLon = (decimal)_updatedBuildingData.LocationLon;
                        }
                        else  // Out of range. No no.
                        {
                            return ServiceResult<int>.Fail("Invalid Longitude.");
                        }
                    }
                    else  // Null = 0
                    {
                        existingRecord.LocationLon = (decimal)0;
                    }

                    await _db.SaveChangesAsync();

                    return ServiceResult<int>.Ok(existingRecord.LocationCode);
                }
                else
                {
                    return ServiceResult<int>.Fail("Location code not found.");
                }
=======
                await _db.SaveChangesAsync();

                return ServiceResult<int>.Ok(existingBuilding.LocationCode);
>>>>>>> origin/ZZ-development/dashboard-refresh
            }
            catch (Exception ex)
            {
                return ServiceResult<int>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<bool>> ChangeUserRole(string _requestorId, RoleTypes _requestorRole, string _targetId, RoleTypes _targetUserNewRole)
        {
            try
            {
                // Validate requestor inputs
                if (_requestorId == null)
                {
                    return ServiceResult<bool>.Fail("Invalid requestor Id.");
                }

                if (!Enum.IsDefined(typeof(RoleTypes), _requestorRole))
                {
                    return ServiceResult<bool>.Fail("Invalid requestor role.");
                }

                if (!Enum.IsDefined(typeof(RoleTypes), _targetUserNewRole))
                {
                    return ServiceResult<bool>.Fail("Invalid target role.");
                }

                if (_requestorRole != RoleTypes.Admin)
                {
                    return ServiceResult<bool>.Fail("Insufficient privileges.");
                }

                // Find target employee
                var targetUser = await _userManager.FindByIdAsync(_targetId);
                if (targetUser == null)
                {
                    return ServiceResult<bool>.Fail("Employee Id not found.");
                }

                // Get current toles
                var roles = await _userManager.GetRolesAsync(targetUser);

                // Remove all roles if any
                if (roles.Any())
                {
                    var roleResult = await _userManager.RemoveFromRolesAsync(targetUser, roles);
                    if (!roleResult.Succeeded)
                    {
                        return ServiceResult<bool>.Fail("Unable to remove existing role(s).");
                    }
                }

                // Set new role as provided
                var result = await _userManager.AddToRoleAsync(targetUser, _targetUserNewRole.ToString());
                if (!result.Succeeded)
                {
                    return ServiceResult<bool>.Fail("Failed to add role.");
                }

                return ServiceResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Fail(ex.Message);
            }
        }
    }
}
