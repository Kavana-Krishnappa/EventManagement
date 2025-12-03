using AutoMapper;
using EventManagement.DTOs;
using EventManagement.Models;
using EventManagement.Repositories;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventManagement.Services
{
    public class EventService : IEventService
    {
        private readonly IEventManagementRepository<Event> _eventRepository;
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<EventService> _logger;

        public EventService(
            IEventManagementRepository<Event> eventRepository,
            ApplicationDbContext dbContext,
            IMapper mapper,
            ILogger<EventService> logger)
        {
            _eventRepository = eventRepository;
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResponse<IEnumerable<EventDTO>>> GetAllEventsAsync()
        {
            try
            {
                var events = await _eventRepository.GetAllAsync();
                var eventDTOs = _mapper.Map<List<EventDTO>>(events);

                _logger.LogInformation("Retrieved {Count} events", eventDTOs.Count);

                return ServiceResponse<IEnumerable<EventDTO>>.SuccessResponse(eventDTOs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving events");
                return ServiceResponse<IEnumerable<EventDTO>>.FailureResponse("An error occurred while retrieving events.");
            }
        }

        public async Task<ServiceResponse<EventDTO>> GetEventByIdAsync(int id)
        {
            try
            {
                var eventItem = await _eventRepository.GetByIdAsync(id);

                if (eventItem == null)
                {
                    _logger.LogWarning("Event with ID {EventId} not found", id);
                    return ServiceResponse<EventDTO>.FailureResponse("Event not found");
                }

                var eventDTO = _mapper.Map<EventDTO>(eventItem);

                return ServiceResponse<EventDTO>.SuccessResponse(eventDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving event {EventId}", id);
                return ServiceResponse<EventDTO>.FailureResponse("An error occurred while retrieving the event.");
            }
        }

        public async Task<ServiceResponse<EventDTO>> CreateEventAsync(EventDTO eventDTO)
        {
            try
            {
                if (eventDTO == null)
                    return ServiceResponse<EventDTO>.FailureResponse("Invalid event data");

                var eventItem = _mapper.Map<Event>(eventDTO);
                await _eventRepository.AddAsync(eventItem);

                eventDTO.EventId = eventItem.EventId;

                _logger.LogInformation("Event created: {EventId}", eventItem.EventId);

                return ServiceResponse<EventDTO>.SuccessResponse(eventDTO, "Event created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating event");
                return ServiceResponse<EventDTO>.FailureResponse("An error occurred while creating the event.");
            }
        }

        public async Task<ServiceResponse<bool>> UpdateEventAsync(int id, JsonPatchDocument<EventDTO> patchDocument)
        {
            try
            {
                if (patchDocument == null || id <= 0)
                    return ServiceResponse<bool>.FailureResponse("Invalid update data");

                var existingEvent = await _eventRepository.GetByIdAsync(id);

                if (existingEvent == null)
                    return ServiceResponse<bool>.FailureResponse("Event not found");

                var eventDTO = _mapper.Map<EventDTO>(existingEvent);
                patchDocument.ApplyTo(eventDTO);

                _mapper.Map(eventDTO, existingEvent);
                await _eventRepository.UpdateAsync(existingEvent);

                _logger.LogInformation("Event updated: {EventId}", id);

                return ServiceResponse<bool>.SuccessResponse(true, "Event updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating event {EventId}", id);
                return ServiceResponse<bool>.FailureResponse("An error occurred while updating the event.");
            }
        }

        public async Task<ServiceResponse<EventCapacityDTO>> GetEventCapacityAsync(int id)
        {
            try
            {
                var eventItem = await _eventRepository.GetByIdAsync(id);

                if (eventItem == null)
                    return ServiceResponse<EventCapacityDTO>.FailureResponse("Event not found");

                var registrationCount = _dbContext.Registrations
                    .Count(r => r.EventId == id && r.Status == "Confirmed");

                var capacityDTO = new EventCapacityDTO
                {
                    EventId = id,
                    MaxCapacity = eventItem.MaxCapacity,
                    CurrentRegistrations = registrationCount,
                    AvailableSpots = eventItem.MaxCapacity - registrationCount,
                    IsFull = registrationCount >= eventItem.MaxCapacity
                };

                return ServiceResponse<EventCapacityDTO>.SuccessResponse(capacityDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving event capacity {EventId}", id);
                return ServiceResponse<EventCapacityDTO>.FailureResponse("An error occurred while retrieving event capacity.");
            }
        }

        public async Task<ServiceResponse<bool>> DeleteEventAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return ServiceResponse<bool>.FailureResponse("Invalid event ID");

                var deleted = await _eventRepository.DeleteAsync(id);

                if (!deleted)
                    return ServiceResponse<bool>.FailureResponse("Event not found");

                _logger.LogInformation("Event deleted: {EventId}", id);

                return ServiceResponse<bool>.SuccessResponse(true, "Event deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting event {EventId}", id);
                return ServiceResponse<bool>.FailureResponse("An error occurred while deleting the event.");
            }
        }
    }
}
