using AutoMapper;
using EventManagement.DTOs;
using EventManagement.Models;
using EventManagement.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventManagement.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IEventManagementRepository<Registration> _registrationRepository;
        private readonly IEventManagementRepository<Event> _eventRepository;
        private readonly IEventManagementRepository<Participant> _participantRepository;
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<RegistrationService> _logger;

        public RegistrationService(
            IEventManagementRepository<Registration> registrationRepository,
            IEventManagementRepository<Event> eventRepository,
            IEventManagementRepository<Participant> participantRepository,
            ApplicationDbContext dbContext,
            IMapper mapper,
            ILogger<RegistrationService> logger)
        {
            _registrationRepository = registrationRepository;
            _eventRepository = eventRepository;
            _participantRepository = participantRepository;
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResponse<IEnumerable<RegistrationDTO>>> GetRegistrationsForEventAsync(int eventId)
        {
            try
            {
                var eventItem = await _eventRepository.GetByIdAsync(eventId);

                if (eventItem == null)
                    return ServiceResponse<IEnumerable<RegistrationDTO>>.FailureResponse("Event not found");

                var registrations = await _dbContext.Registrations
                    .Where(r => r.EventId == eventId)
                    .Include(r => r.Participant)
                    .Select(r => new RegistrationDTO
                    {
                        RegistrationId = r.RegistrationId,
                        EventId = r.EventId,
                        ParticipantId = r.ParticipantId,
                        Status = r.Status,
                        RegisteredAt = r.RegisteredAt
                    })
                    .ToListAsync();

                _logger.LogInformation("Retrieved {Count} registrations for event {EventId}", registrations.Count, eventId);

                return ServiceResponse<IEnumerable<RegistrationDTO>>.SuccessResponse(registrations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving registrations for event {EventId}", eventId);
                return ServiceResponse<IEnumerable<RegistrationDTO>>.FailureResponse("An error occurred while retrieving registrations.");
            }
        }

        public async Task<ServiceResponse<RegistrationDTO>> RegisterParticipantAsync(int eventId, RegistrationCreateDTO registrationDTO)
        {
            try
            {
                var eventItem = await _eventRepository.GetByIdAsync(eventId);
                if (eventItem == null)
                    return ServiceResponse<RegistrationDTO>.FailureResponse("Event not found");

                var participant = await _participantRepository.GetByIdAsync(registrationDTO.ParticipantId);
                if (participant == null)
                    return ServiceResponse<RegistrationDTO>.FailureResponse("Participant not found");

                var existingRegistration = await _registrationRepository
                    .GetFirstOrDefaultAsync(r => r.EventId == eventId && r.ParticipantId == registrationDTO.ParticipantId);

                if (existingRegistration != null)
                    return ServiceResponse<RegistrationDTO>.FailureResponse("Participant is already registered for this event");

                var currentRegistrations = _dbContext.Registrations
                    .Count(r => r.EventId == eventId && r.Status == "Confirmed");

                if (currentRegistrations >= eventItem.MaxCapacity)
                    return ServiceResponse<RegistrationDTO>.FailureResponse("Event has reached maximum capacity");

                var registration = new Registration
                {
                    EventId = eventId,
                    ParticipantId = registrationDTO.ParticipantId,
                    Status = registrationDTO.Status,
                    RegisteredAt = DateTime.UtcNow
                };

                await _registrationRepository.AddAsync(registration);

                var responseDTO = _mapper.Map<RegistrationDTO>(registration);

                _logger.LogInformation("Registration created: Participant {ParticipantId} registered for Event {EventId}",
                    registrationDTO.ParticipantId, eventId);

                return ServiceResponse<RegistrationDTO>.SuccessResponse(responseDTO, "Registration successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering participant for event {EventId}", eventId);
                return ServiceResponse<RegistrationDTO>.FailureResponse("An error occurred while registering the participant.");
            }
        }

        public async Task<ServiceResponse<bool>> DeleteRegistrationAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return ServiceResponse<bool>.FailureResponse("Invalid registration ID");

                var registration = await _registrationRepository.GetByIdAsync(id);

                if (registration == null)
                    return ServiceResponse<bool>.FailureResponse("Registration not found");

                var deleted = await _registrationRepository.DeleteAsync(id);

                if (!deleted)
                    return ServiceResponse<bool>.FailureResponse("Failed to delete registration");

                _logger.LogInformation("Registration deleted: {RegistrationId}", id);

                return ServiceResponse<bool>.SuccessResponse(true, "Registration deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting registration {RegistrationId}", id);
                return ServiceResponse<bool>.FailureResponse("An error occurred while deleting the registration.");
            }
        }
    }
}