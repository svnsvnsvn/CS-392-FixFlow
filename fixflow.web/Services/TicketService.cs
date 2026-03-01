using fixflow.web.Data;
using fixflow.web.Domain.Enums;
using fixflow.web.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace fixflow.web.Services
{
    public class TicketService : ITicketService
    {
        private readonly FfDbContext _db;

        public TicketService(FfDbContext db)
        {
            _db = db;
        }

        public async Task<ServiceResult<Guid>> AddNewTicket(string _requestorId, RoleTypes _requestorRole, NewTicketDto _newTicketData)
        {
            try
            {
                // Validate requestor inputs
                bool validRequestor = await _db.FfUserProfiles.AnyAsync(u => u.FfUserId == _requestorId);

                if ((_requestorId == null) || (validRequestor == false))
                {
                    return ServiceResult<Guid>.Fail("Invalid requestor Id.");
                }

                if (!Enum.IsDefined(typeof(RoleTypes), _requestorRole))
                {
                    return ServiceResult<Guid>.Fail("Invalid role.");
                }

                if (_requestorRole == RoleTypes.Pending)
                {
                    return ServiceResult<Guid>.Fail("Insufficient privileges.");
                }

                // Validate new ticket info and create record
                var newTicket = new FfTicketRegister();
                newTicket.TicketId = Guid.NewGuid();

                var result = await GetNextShortCode();
                if ((result == null) || (!result.Success))
                {
                    return ServiceResult<Guid>.Fail("Could not create new short code");
                }
                newTicket.TicketShortCode = result.Data;
                   

                // Store requestor who called this as creator of ticket
                newTicket.EnteredBy = _requestorId;

                // Store requestor as the id provided, if null use the submitter, of ID not found fail
                if (_newTicketData.RequestedBy == null)
                {
                    newTicket.RequestedBy = _requestorId;
                }
                else
                {
                    bool exists = await _db.FfUserProfiles.AnyAsync(u => u.FfUserId == _newTicketData.RequestedBy);
                    if (exists)
                    {
                        newTicket.RequestedBy = _newTicketData.RequestedBy;
                    }
                    else
                    {
                        return ServiceResult<Guid>.Fail("RequestedBy does not exist");
                    }
                }

                // Validate location and store code in ticket
                var validLocation = await _db.FfBuildingDirectorys.FindAsync(_newTicketData.Location);
                if (validLocation == null)
                {
                    return ServiceResult<Guid>.Fail("Location does not exist");
                }
                newTicket.Location = _newTicketData.Location;

                // Validate locations unit number
                if ((_newTicketData.Unit > validLocation.NumUnits) || (_newTicketData.Unit < 0))
                {
                    return ServiceResult<Guid>.Fail("Invalid unit number");
                }
                newTicket.Unit = _newTicketData.Unit;

                // Validate trouble ticket type
                bool validTicketType = await _db.FfTicketTypess.AnyAsync(u => u.Id == _newTicketData.TicketTroubleType);
                if (!validTicketType)
                {
                    return ServiceResult<Guid>.Fail("Invalid ticket type");
                }
                newTicket.TicketTroubleType = _newTicketData.TicketTroubleType;

                // Validate priority code
                bool validPrioirtyCode = await _db.FfStatusCodes.AnyAsync(u => u.StatusCode == _newTicketData.TicketPriority);
                if (!validPrioirtyCode)
                {
                    return ServiceResult<Guid>.Fail("Invalid priority code");
                }
                newTicket.TicketPriority = _newTicketData.TicketPriority;

                // Validate status code
                bool validStatusCode = await _db.FfPriorityCodess.AnyAsync(u => u.PriorityCode == _newTicketData.TicketPriority);
                if (!validStatusCode)
                {
                    return ServiceResult<Guid>.Fail("Invalid status code");
                }
                newTicket.TicketStatus = _newTicketData.TicketStatus;

                // Validate ticket subject
                if (_newTicketData.TicketSubject == null)
                {
                    return ServiceResult<Guid>.Fail("Missing ticket subject");
                }
                newTicket.TicketSubject = _newTicketData.TicketSubject;

                // Validate ticket description
                if (_newTicketData.TicketDescription == null)
                {
                    return ServiceResult<Guid>.Fail("Missing ticket description");
                }
                newTicket.TicketDescription = _newTicketData.TicketDescription;

                // Create initial TicketFlow entry for this ticket
                var newTicketFlow = new FfTicketFlow();

                newTicketFlow.TicketId = newTicket.TicketId;
                newTicketFlow.NewTicketStatus = newTicket.TicketStatus;
                newTicketFlow.NewAssignee = _requestorId;
                newTicketFlow.TimeStamp = DateTime.UtcNow;


                // Create transaction object and write ticket to DB's
                using var userCreationTransaction = await _db.Database.BeginTransactionAsync();
                try
                {
                    _db.FfTicketRegisters.Add(newTicket);
                    _db.FfTicketFlows.Add(newTicketFlow);

                    await _db.SaveChangesAsync();

                    await userCreationTransaction.CommitAsync();
                }
                catch
                {
                    await userCreationTransaction.RollbackAsync();          // Stop db writes if something failed. Prevent half transactions.
                    return ServiceResult<Guid>.Fail("Database write failed");
                }

                return ServiceResult<Guid>.Ok(newTicket.TicketId);
            }
            catch (Exception ex)
            {
                return ServiceResult<Guid>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<bool>> UpdateTicket(string _requestorId, RoleTypes _requestorRole, UpdateTicketDto _updatedTicketData)
        {
            try
            {
                // Validate requestor inputs
                bool validRequestor = await _db.FfUserProfiles.AnyAsync(u => u.FfUserId == _requestorId);

                if ((_requestorId == null) || (validRequestor == false))
                {
                    return ServiceResult<bool>.Fail("Invalid requestor Id.");
                }

                // Validate ticket Id exists
                bool validTicket = await _db.FfTicketRegisters.AnyAsync(u => u.TicketId == _updatedTicketData.TicketId);
                if (!validTicket)
                {
                    return ServiceResult<bool>.Fail("Invalid TIcketId provided.");
                }

                if (!Enum.IsDefined(typeof(RoleTypes), _requestorRole))
                {
                    return ServiceResult<bool>.Fail("Invalid role.");
                }

                if (_requestorRole == RoleTypes.Pending)
                {
                    return ServiceResult<bool>.Fail("Insufficient privileges.");
                }

                var ticketToUpdate = await _db.FfTicketRegisters.FindAsync(_updatedTicketData.TicketId);
                if (ticketToUpdate == null)
                {
                    return ServiceResult<bool>.Fail("Ticket to update not found.");
                }

                ticketToUpdate.RequestedBy = _updatedTicketData.RequestedBy;
                ticketToUpdate.Location = _updatedTicketData.Location;
                ticketToUpdate.Unit = _updatedTicketData.Unit;
                ticketToUpdate.TicketTroubleType = _updatedTicketData.TicketTroubleType;
                ticketToUpdate.TicketPriority = _updatedTicketData.TicketPriority;
                ticketToUpdate.TicketSubject = _updatedTicketData.TicketSubject;
                ticketToUpdate.TicketDescription = _updatedTicketData.TicketDescription;

                await _db.SaveChangesAsync();

                return ServiceResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<long>> ReassignTicket(string _requestorId, RoleTypes _requestorRole, Guid _ticketIdToUpdate, string _newAssigneeId, int _newStatus)
        {
            try
            {
                // Validate requestor inputs
                bool validRequestor = await _db.FfUserProfiles.AnyAsync(u => u.FfUserId == _requestorId);

                if ((_requestorId == null) || (validRequestor == false))
                {
                    return ServiceResult<long>.Fail("Invalid requestor Id.");
                }

                // Validate new asignee inputs
                bool validAssignee = await _db.FfUserProfiles.AnyAsync(u => u.FfUserId == _newAssigneeId);

                if ((_newAssigneeId == null) || (validAssignee == false))
                {
                    return ServiceResult<long>.Fail("Invalid assignee Id.");
                }

                // Validate ticket Id exists
                bool validTicket = await _db.FfTicketRegisters.AnyAsync(u => u.TicketId == _ticketIdToUpdate);
                if (!validTicket)
                {
                    return ServiceResult<long>.Fail("Invalid TicketId provided.");
                }

                // Validate new status code exists
                bool validStatus = await _db.FfStatusCodes.AnyAsync(u => u.StatusCode == _newStatus);
                if (!validStatus)
                {
                    return ServiceResult<long>.Fail("invalid status code provided.");
                }

                if (!Enum.IsDefined(typeof(RoleTypes), _requestorRole))
                {
                    return ServiceResult<long>.Fail("Invalid role.");
                }

                if (_requestorRole == RoleTypes.Pending)
                {
                    return ServiceResult<long>.Fail("Insufficient privileges.");
                }

                var ticketFlowUpdate = new FfTicketFlow()
                {
                    TicketId = _ticketIdToUpdate,
                    NewTicketStatus = _newStatus,
                    NewAssignee = _newAssigneeId,
                    TimeStamp = DateTime.UtcNow
                };

                try
                {
                    await _db.FfTicketFlows.AddAsync(ticketFlowUpdate);
                    await _db.SaveChangesAsync();
                    return ServiceResult<long>.Ok(ticketFlowUpdate.ActionId);
                }
                catch (Exception ex)
                {
                    return ServiceResult<long>.Fail("Database write failed.");
                }
            }
            catch (Exception ex)
            {
                return ServiceResult<long>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<List<TicketTypeDto>>> GetTicketTypes()
        {
            try
            {
                var ticketTypes = await _db.FfTicketTypess
                    .OrderBy(a => a.Id)
                    .Select(a => new TicketTypeDto
                    {
                        Id = a.Id,
                        TypeName = a.TypeName
                    })
                    .ToListAsync();

                if (ticketTypes == null || ticketTypes.Count == 0)
                {
                    return ServiceResult<List<TicketTypeDto>>.Fail("No Ticket Types Defined");
                }
                
                return ServiceResult<List<TicketTypeDto>>.Ok(ticketTypes);

            }
            catch (Exception ex)
            {
                return ServiceResult<List<TicketTypeDto>>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<List<BuildingDto>>> GetBuildings()
        {
            try
            {
                var buildingOptions = await _db.FfBuildingDirectorys
                    .Where(a => a.LocationName != "Unassigned")
                    .OrderBy(a => a.LocationCode)
                    .Select(a => new BuildingDto
                    {
                        LocationCode = a.LocationCode,
                        LocationName = a.LocationName,
                        ComplexName = a.ComplexName,
                        BuildingNumber = a.BuildingNumber,
                        NumUnits = a.NumUnits,
                        LocationLat = a.LocationLat,
                        LocationLon = a.LocationLon
                    })
                    .ToListAsync();

                if (buildingOptions == null || buildingOptions.Count == 0)
                {
                    return ServiceResult<List<BuildingDto>>.Fail("No Buildings Defined");
                }
                
                return ServiceResult<List<BuildingDto>>.Ok(buildingOptions);

            }
            catch (Exception ex)
            {
                return ServiceResult<List<BuildingDto>>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<List<StatusCodeDto>>> GetStatusCodeList()
        {
            try
            {
                var statusCodeOptions = await _db.FfStatusCodes
                    .OrderBy(a => a.StatusCode)
                    .Select(a => new StatusCodeDto
                    {
                        StatusCode = a.StatusCode,
                        StatusName = a.StatusName
                    })
                    .ToListAsync();

                if (statusCodeOptions == null || statusCodeOptions.Count == 0)
                {
                    return ServiceResult<List<StatusCodeDto>>.Fail("No Status Codes Defined");
                }

                return ServiceResult<List<StatusCodeDto>>.Ok(statusCodeOptions);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<StatusCodeDto>>.Fail(ex.Message);
            }
        }

        public async Task<ServiceResult<int>> GetStatusCode(string _StatusName)
        {
            try
            {
                var result = await _db.FfStatusCodes.SingleAsync(a => a.StatusName == _StatusName);
                return ServiceResult<int>.Ok(result.StatusCode);
            }
            catch (Exception ex)
            {
                return ServiceResult<int>.Fail(ex.Message);
            }
        }
        
        public async Task<ServiceResult<string>> GetStatusCode(int _StatusCode)
        {
            try
            {
                var result = await _db.FfStatusCodes.SingleAsync(a => a.StatusCode == _StatusCode);
                return ServiceResult<string>.Ok(result.StatusName);
            }
            catch (Exception ex)
            {
                return ServiceResult<string>.Fail(ex.Message);
            }

        }

        public async Task<ServiceResult<string>> GetNextShortCode()
        {
            try
            {
                var ticketSeriesData = await _db.FfTicketConstructoror.SingleAsync(a => a.SeriesIsActive == true);

                string newShortCode = ticketSeriesData.TicketPrefix +
                    "-" + ticketSeriesData.TicketSeries.ToString() +
                    "-" + (ticketSeriesData.LastTicketUsed + 1).ToString();

                ticketSeriesData.LastTicketUsed++;

                await _db.SaveChangesAsync();

                return ServiceResult<string>.Ok(newShortCode);


            }
            catch (Exception ex)
            {
                return ServiceResult<string>.Fail(ex.Message);
            }
        }
    }
}