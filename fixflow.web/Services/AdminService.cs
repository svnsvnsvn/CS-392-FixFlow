using fixflow.web.Data;
using fixflow.web.Dto;
using fixflow.web.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

                int newPriorityCodeValue = await _db.FfPriorityCodess
                        .Select(p => (int?)p.PriorityCode)
                        .MaxAsync() ?? 0;
                newPriorityCodeValue++;

                var newPriorityCode = new FfPriorityCodes
                {
                    PriorityName = _newPriorityData.PriorityName,
                    PriorityCode = newPriorityCodeValue
                };
                

                _db.FfPriorityCodess.Add(newPriorityCode);
                await _db.SaveChangesAsync();

                return ServiceResult<int>.Ok(newPriorityCode.Id);
            }
            catch (Exception ex)
            {
                return ServiceResult<int>.Fail(ex.Message);
            }
        }
        public async Task<ServiceResult<int>> IncrementPriorityCode(string _requestorId, RoleTypes _requestorRole, int _Id)
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

                if (_Id < 0)  // If not null, new value is negative and not valid
                {
                    return ServiceResult<int>.Fail("Invalid priority code.");
                }

                
                // Begin transaction to wrap two possible priority code changes.
                using var userCreationTransaction = await _db.Database.BeginTransactionAsync();
                
                var existingRecord = await _db.FfPriorityCodess.FindAsync(_Id);
                if (existingRecord == null)
                {
                    return ServiceResult<int>.Fail("Priority ID not found.");
                }

                try
                    {

                    // See if a higher code exists, if so decrement it
                    bool exists = await _db.FfPriorityCodess.AnyAsync(p => p.PriorityCode == (existingRecord.PriorityCode + 1));
                    if (exists)
                    {
                        var existingRecordNext = await _db.FfPriorityCodess.FirstOrDefaultAsync(p => p.PriorityCode == (existingRecord.PriorityCode + 1));
                        existingRecordNext.PriorityCode--;
                    }
                    // Increment code
                    existingRecord.PriorityCode++;

                    // Flip polarity temporarily, to prevent circular reference in Db
                    existingRecord.PriorityCode *= -1;
                    
                    // Write records
                    await _db.SaveChangesAsync();
                    
                    // Flip back
                    existingRecord.PriorityCode *= -1;
                    await _db.SaveChangesAsync();
                    await userCreationTransaction.CommitAsync();
                }
                catch
                {
                    await userCreationTransaction.RollbackAsync();          // Stop db writes if something failed. Prevent half transactions.
                    throw;
                }

                return ServiceResult<int>.Ok(existingRecord.Id);

            }
            catch (Exception ex)
            {
                return ServiceResult<int>.Fail(ex.Message);
            }
        }
        public async Task<ServiceResult<int>> DecrementPriorityCode(string _requestorId, RoleTypes _requestorRole, int _Id)
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

                if (_Id < 0)  // If not null, new value is negative and not valid
                {
                    return ServiceResult<int>.Fail("Invalid priority code.");
                }


                // Begin transaction to wrap two possible priority code changes.
                using var userCreationTransaction = await _db.Database.BeginTransactionAsync();

                var existingRecord = await _db.FfPriorityCodess.FindAsync(_Id);
                if (existingRecord == null)
                {
                    return ServiceResult<int>.Fail("Priority ID not found.");
                }

                if (existingRecord.PriorityCode < 1)
                {
                    return ServiceResult<int>.Fail("Can not decrement Priority Code further.");
                }

                try
                {

                    // See if a lower code exists, if so increment it
                    bool exists = await _db.FfPriorityCodess.AnyAsync(p => p.PriorityCode == (existingRecord.PriorityCode - 1));
                    if (exists)
                    {
                        var existingRecordNext = await _db.FfPriorityCodess.FirstOrDefaultAsync(p => p.PriorityCode == (existingRecord.PriorityCode - 1));
                        existingRecordNext.PriorityCode++;
                    }
                    // Increment code
                    existingRecord.PriorityCode--;

                    // Flip polarity temporarily, to prevent circular reference in Db
                    existingRecord.PriorityCode *= -1;

                    // Write records
                    await _db.SaveChangesAsync();

                    // Flip back
                    existingRecord.PriorityCode *= -1;
                    await _db.SaveChangesAsync();

                    await userCreationTransaction.CommitAsync();
                }
                catch
                {
                    await userCreationTransaction.RollbackAsync();          // Stop db writes if something failed. Prevent half transactions.
                    throw;
                }

                return ServiceResult<int>.Ok(existingRecord.Id);

            }
            catch (Exception ex)
            {
                return ServiceResult<int>.Fail(ex.Message);
            }
        }
        public async Task<ServiceResult<bool>> DeletePriorityCode(string _requestorId, RoleTypes _requestorRole, int _Id)
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
                    return ServiceResult<bool>.Fail("Invalid role.");
                }

                if (_requestorRole != RoleTypes.Admin)
                {
                    return ServiceResult<bool>.Fail("Insufficient privileges.");
                }

                if (_Id < 0)  // If not null, new value is negative and not valid
                {
                    return ServiceResult<bool>.Fail("Invalid priority code.");
                }


                try
                {
                    var existingRecord = await _db.FfPriorityCodess.FindAsync(_Id);
                    if (existingRecord == null)
                    {
                        return ServiceResult<bool>.Fail("Priority ID not found.");
                    }

                    _db.FfPriorityCodess.Remove(existingRecord);
                    // Write records
                    await _db.SaveChangesAsync();
                }
                catch
                {
                    throw;
                }

                return ServiceResult<bool>.Ok(true);

            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Fail(ex.Message);
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

                int newStatusCodeValue = await _db.FfStatusCodes
                        .Select(p => (int?)p.StatusCode)
                        .MaxAsync() ?? 0;
                newStatusCodeValue++;

                var newStatusCode = new FfStatusCodes
                {
                    StatusName = _newStatusData.StatusName,
                    StatusCode = newStatusCodeValue
                };

                _db.FfStatusCodes.Add(newStatusCode);
                await _db.SaveChangesAsync();

                return ServiceResult<int>.Ok(newStatusCode.Id);
            }
            catch (Exception ex)
            {
                return ServiceResult<int>.Fail(ex.Message);
            }
        }
        public async Task<ServiceResult<int>> IncrementStatusCode(string _requestorId, RoleTypes _requestorRole, int _Id)
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

                if (_Id < 0)  // If not null, new value is negative and not valid
                {
                    return ServiceResult<int>.Fail("Invalid status code.");
                }


                // Begin transaction to wrap two possible priority code changes.
                using var userCreationTransaction = await _db.Database.BeginTransactionAsync();

                var existingRecord = await _db.FfStatusCodes.FindAsync(_Id);
                if (existingRecord == null)
                {
                    return ServiceResult<int>.Fail("Status ID not found.");
                }

                try
                {

                    // See if a higher code exists, if so decrement it
                    bool exists = await _db.FfStatusCodes.AnyAsync(p => p.StatusCode == (existingRecord.StatusCode + 1));
                    if (exists)
                    {
                        var existingRecordNext = await _db.FfStatusCodes.FirstOrDefaultAsync(p => p.StatusCode == (existingRecord.StatusCode + 1));
                        existingRecordNext.StatusCode--;
                    }
                    // Increment code
                    existingRecord.StatusCode++;

                    // Flip polarity temporarily, to prevent circular reference in Db
                    existingRecord.StatusCode *= -1;

                    // Write records
                    await _db.SaveChangesAsync();

                    // Flip back
                    existingRecord.StatusCode *= -1;
                    await _db.SaveChangesAsync();
                    await userCreationTransaction.CommitAsync();
                }
                catch
                {
                    await userCreationTransaction.RollbackAsync();          // Stop db writes if something failed. Prevent half transactions.
                    throw;
                }

                return ServiceResult<int>.Ok(existingRecord.Id);

            }
            catch (Exception ex)
            {
                return ServiceResult<int>.Fail(ex.Message);
            }
        }
        public async Task<ServiceResult<int>> DecrementStatusCode(string _requestorId, RoleTypes _requestorRole, int _Id)
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

                if (_Id < 0)  // If not null, new value is negative and not valid
                {
                    return ServiceResult<int>.Fail("Invalid status code.");
                }


                // Begin transaction to wrap two possible priority code changes.
                using var userCreationTransaction = await _db.Database.BeginTransactionAsync();

                var existingRecord = await _db.FfStatusCodes.FindAsync(_Id);
                if (existingRecord == null)
                {
                    return ServiceResult<int>.Fail("Status ID not found.");
                }

                if (existingRecord.StatusCode < 1)
                {
                    return ServiceResult<int>.Fail("Can not decrement Status Code further.");
                }

                try
                {

                    // See if a lower code exists, if so increment it
                    bool exists = await _db.FfStatusCodes.AnyAsync(p => p.StatusCode == (existingRecord.StatusCode - 1));
                    if (exists)
                    {
                        var existingRecordNext = await _db.FfStatusCodes.FirstOrDefaultAsync(p => p.StatusCode == (existingRecord.StatusCode - 1));
                        existingRecordNext.StatusCode++;
                    }
                    // Increment code
                    existingRecord.StatusCode--;

                    // Flip polarity temporarily, to prevent circular reference in Db
                    existingRecord.StatusCode *= -1;

                    // Write records
                    await _db.SaveChangesAsync();

                    // Flip back
                    existingRecord.StatusCode *= -1;
                    await _db.SaveChangesAsync();

                    await userCreationTransaction.CommitAsync();
                }
                catch
                {
                    await userCreationTransaction.RollbackAsync();          // Stop db writes if something failed. Prevent half transactions.
                    throw;
                }

                return ServiceResult<int>.Ok(existingRecord.Id);

            }
            catch (Exception ex)
            {
                return ServiceResult<int>.Fail(ex.Message);
            }
        }
        public async Task<ServiceResult<bool>> DeleteStatusCode(string _requestorId, RoleTypes _requestorRole, int _Id)
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
                    return ServiceResult<bool>.Fail("Invalid role.");
                }

                if (_requestorRole != RoleTypes.Admin)
                {
                    return ServiceResult<bool>.Fail("Insufficient privileges.");
                }

                if (_Id < 0)  // If not null, new value is negative and not valid
                {
                    return ServiceResult<bool>.Fail("Invalid status code.");
                }


                try
                {
                    var existingRecord = await _db.FfStatusCodes.FindAsync(_Id);
                    if (existingRecord == null)
                    {
                        return ServiceResult<bool>.Fail("Status ID not found.");
                    }

                    _db.FfStatusCodes.Remove(existingRecord);
                    // Write records
                    await _db.SaveChangesAsync();
                }
                catch
                {
                    throw;
                }

                return ServiceResult<bool>.Ok(true);

            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Fail(ex.Message);
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

                return ServiceResult<int>.Ok(newTicketType.Id);
            }
            catch (Exception ex)
            {
                return ServiceResult<int>.Fail(ex.Message);
            }
        }
        public async Task<ServiceResult<int>> UpdateTicketType(string _requestorId, RoleTypes _requestorRole, TicketTypeDto _updatedTicketData)
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
            }
            catch (Exception ex)
            {
                return ServiceResult<int>.Fail(ex.Message);
            }
        }
        public async Task<ServiceResult<int>> DeleteTicketType(string _requestorId, RoleTypes _requestorRole, int _Id)
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

                if (_Id < 0)  // If not null, new value is negative and not valid
                {
                    return ServiceResult<int>.Fail("Invalid ticket type.");
                }


                try
                {
                    var existingRecord = await _db.FfTicketTypess.FindAsync(_Id);
                    if (existingRecord == null)
                    {
                        return ServiceResult<int>.Fail("Ticket type ID not found.");
                    }

                    _db.FfTicketTypess.Remove(existingRecord);
                    // Write records
                    await _db.SaveChangesAsync();
                }
                catch
                {
                    throw;
                }

                return ServiceResult<int>.Ok(0);

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

                // Guard possible NULL values
                if (_newBuildingData.NumUnits == null)
                {
                    _newBuildingData.NumUnits = 0;
                }
                if (_newBuildingData.LocationLat == null)
                {
                    _newBuildingData.LocationLat = 0;
                }
                if (_newBuildingData.LocationLon == null)
                {
                    _newBuildingData.LocationLon = 0;
                }



                var newBuilding = new FfBuildingDirectory
                {
                    LocationName = _newBuildingData.LocationName,
                    ComplexName = _newBuildingData.ComplexName,
                    BuildingNumber = _newBuildingData.BuildingNumber,
                    NumUnits = (int)_newBuildingData.NumUnits,
                    LocationLat = (decimal)_newBuildingData.LocationLat,
                    LocationLon = (decimal)_newBuildingData.LocationLon
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

        public async Task<ServiceResult<int>> UpdateBuilding(string _requestorId, RoleTypes _requestorRole, BuildingDto _updatedBuildingData)
        {
            try
            {
                // Prevent Location name of "Unassigned" this is a protected location name sued as a default.
                if (_updatedBuildingData.LocationName == "Unassigned")
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


        public async Task<ServiceResult<List<UserListItemDto>>> SearchUsers(string _searchString)
        {
            try
            {
                if (_searchString.Length >= 3)
                {
                    _searchString = _searchString.ToLower();

                    var terms = _searchString.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                    var query = _db.FfUserProfiles.AsQueryable();

                    foreach (var term in terms)
                    {
                        query = query.Where(p =>
                            p.FName.ToLower().StartsWith(term) ||
                            p.LName.ToLower().StartsWith(term));
                    }

                    // Find user profiles that meet query (First or Last name, first characters)
                    var searchResults = await query
                        .Take(10)
                        .ToListAsync();

                    // Extract ids from searchResults
                    var searchIds = searchResults.Select(p => p.FfUserId).ToList();

                    // Get roles for those Ids
                    var roleByUserId = await (
                        from ur in _db.UserRoles
                        join r in _db.Roles on ur.RoleId equals r.Id
                        where searchIds.Contains(ur.UserId)
                        select new { ur.UserId, RoleName = r.Name }
                    ).ToDictionaryAsync(x => x.UserId, x => x.RoleName);


                    var finalResults = searchResults
                        .OrderBy(p => p.LName)
                        .ThenBy(p => p.FName)
                        .Select(p => new UserListItemDto
                        {
                            UserId = p.FfUserId,
                            FName = p.FName,
                            LName = p.LName,
                            Role = roleByUserId.TryGetValue(p.FfUserId, out var role) ? role : null
                        }).ToList();

                    return ServiceResult<List<UserListItemDto>>.Ok(finalResults);
                }
                return ServiceResult<List<UserListItemDto>>.Fail("Query String < 3 characters.");
            }
            catch (Exception ex)
            {
                return ServiceResult<List<UserListItemDto>>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<UserSettingsListItemDTO>> GetUserSettings(string _requestorId, RoleTypes _requestorRole, string _targetUser)
        {
            try
            {
                // Validate requestor inputs
                if (_requestorId == null)
                {
                    return ServiceResult<UserSettingsListItemDTO>.Fail("Invalid Requestor Id.");
                }

                if (!Enum.IsDefined(typeof(RoleTypes), _requestorRole))
                {
                    return ServiceResult<UserSettingsListItemDTO>.Fail("Invalid Role.");
                }

                // This function is for Admins only
                if (_requestorRole != RoleTypes.Admin)
                {
                    return ServiceResult<UserSettingsListItemDTO>.Fail("Insufficient Privileges.");
                }

                // Find target employee
                var targetUserSettings = await _userManager.FindByIdAsync(_targetUser);
                if (targetUserSettings == null)
                {
                    return ServiceResult<UserSettingsListItemDTO>.Fail("User Id not found.");
                }
                var targetUserName = await _db.FfUserProfiles.FindAsync(_targetUser);
                if (targetUserName == null)
                {
                    return ServiceResult<UserSettingsListItemDTO>.Fail("User Id not found.");
                }
                var targetUserRole = await _userManager.GetRolesAsync(targetUserSettings);
                if (targetUserRole == null)
                {
                    return ServiceResult<UserSettingsListItemDTO>.Fail("User Id not found.");
                }

                UserSettingsListItemDTO returnData = new UserSettingsListItemDTO();
                returnData.UserId = targetUserSettings.Id;
                returnData.FName = targetUserName.FName;
                returnData.LName = targetUserName.LName;
                returnData.Role = targetUserRole[0];
                returnData.ResetPassOnLogin = targetUserSettings.ResetPassOnLogin;

                return ServiceResult<UserSettingsListItemDTO>.Ok(returnData);
            }
            catch (Exception ex)
            {
                return ServiceResult<UserSettingsListItemDTO>.Fail("Employee Id not found.");
            }
        }


    }
}
