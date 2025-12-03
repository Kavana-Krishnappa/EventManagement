using AutoMapper;
using BCrypt.Net;
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
    public class ParticipantService : IParticipantService
    {
        private readonly IEventManagementRepository<Participant> _participantRepository;
        private readonly ApplicationDbContext _dbContext;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly ILogger<ParticipantService> _logger;

        public ParticipantService(
            IEventManagementRepository<Participant> participantRepository,
            ApplicationDbContext dbContext,
            ITokenService tokenService,
            IMapper mapper,
            ILogger<ParticipantService> logger)
        {
            _participantRepository = participantRepository;
            _dbContext = dbContext;
            _tokenService = tokenService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResponse<IEnumerable<ParticipantDTO>>> GetAllParticipantsAsync()
        {
            try
            {
                var participants = await _participantRepository.GetAllAsync();
                var participantDTOs = _mapper.Map<List<ParticipantDTO>>(participants);

                _logger.LogInformation("Retrieved {Count} participants", participantDTOs.Count);

                return ServiceResponse<IEnumerable<ParticipantDTO>>.SuccessResponse(participantDTOs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving participants");
                return ServiceResponse<IEnumerable<ParticipantDTO>>.FailureResponse("An error occurred while retrieving participants.");
            }
        }

        public async Task<ServiceResponse<ParticipantDTO>> GetParticipantByIdAsync(int id)
        {
            try
            {
                var participant = await _participantRepository.GetByIdAsync(id);

                if (participant == null)
                    return ServiceResponse<ParticipantDTO>.FailureResponse("Participant not found");

                var participantDTO = _mapper.Map<ParticipantDTO>(participant);

                return ServiceResponse<ParticipantDTO>.SuccessResponse(participantDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving participant {ParticipantId}", id);
                return ServiceResponse<ParticipantDTO>.FailureResponse("An error occurred while retrieving the participant.");
            }
        }

        public async Task<ServiceResponse<ParticipantDTO>> CreateParticipantAsync(ParticipantDTO participantDTO)
        {
            try
            {
                var isDuplicate = await _participantRepository
                    .ExistsAsync(p => p.Email == participantDTO.Email);

                if (isDuplicate)
                    return ServiceResponse<ParticipantDTO>.FailureResponse("A participant with this email already exists.");

                var participant = _mapper.Map<Participant>(participantDTO);
                participant.Password = BCrypt.Net.BCrypt.HashPassword(participantDTO.Password);

                await _participantRepository.AddAsync(participant);

                participantDTO.ParticipantId = participant.ParticipantId;

                _logger.LogInformation("Participant created: {Email}", participant.Email);

                return ServiceResponse<ParticipantDTO>.SuccessResponse(participantDTO, "Participant created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating participant");
                return ServiceResponse<ParticipantDTO>.FailureResponse("An error occurred while creating the participant.");
            }
        }

        public async Task<ServiceResponse<IEnumerable<EventDTO>>> GetUpcomingEventsForParticipantAsync(int participantId)
        {
            try
            {
                var participant = await _participantRepository.GetByIdAsync(participantId);

                if (participant == null)
                    return ServiceResponse<IEnumerable<EventDTO>>.FailureResponse("Participant not found");

                var upcomingEvents = await _dbContext.Registrations
                    .Where(r => r.ParticipantId == participantId)
                    .Include(r => r.Event)
                    .Where(r => r.Event.EventDate >= DateTime.UtcNow)
                    .Select(r => new EventDTO
                    {
                        EventId = r.Event.EventId,
                        EventName = r.Event.EventName,
                        EventDate = r.Event.EventDate,
                        Description = r.Event.Description,
                        Location = r.Event.Location,
                        MaxCapacity = r.Event.MaxCapacity,
                        CreatedByAdminId = r.Event.CreatedByAdminId
                    })
                    .ToListAsync();

                return ServiceResponse<IEnumerable<EventDTO>>.SuccessResponse(upcomingEvents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving upcoming events for participant {ParticipantId}", participantId);
                return ServiceResponse<IEnumerable<EventDTO>>.FailureResponse("An error occurred while retrieving upcoming events.");
            }
        }

        public async Task<ServiceResponse<IEnumerable<EventDTO>>> GetPreviousEventsForParticipantAsync(int participantId)
        {
            try
            {
                var participant = await _participantRepository.GetByIdAsync(participantId);

                if (participant == null)
                    return ServiceResponse<IEnumerable<EventDTO>>.FailureResponse("Participant not found");

                var previousEvents = await _dbContext.Registrations
                    .Where(r => r.ParticipantId == participantId)
                    .Include(r => r.Event)
                    .Where(r => r.Event.EventDate < DateTime.UtcNow)
                    .Select(r => new EventDTO
                    {
                        EventId = r.Event.EventId,
                        EventName = r.Event.EventName,
                        EventDate = r.Event.EventDate,
                        Description = r.Event.Description,
                        Location = r.Event.Location,
                        MaxCapacity = r.Event.MaxCapacity,
                        CreatedByAdminId = r.Event.CreatedByAdminId
                    })
                    .ToListAsync();

                return ServiceResponse<IEnumerable<EventDTO>>.SuccessResponse(previousEvents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving previous events for participant {ParticipantId}", participantId);
                return ServiceResponse<IEnumerable<EventDTO>>.FailureResponse("An error occurred while retrieving previous events.");
            }
        }

        public async Task<ServiceResponse<ParticipantLoginResponseDTO>> LoginAsync(ParticipantLoginDTO loginDTO)
        {
            try
            {
                var participant = await _participantRepository
                    .GetFirstOrDefaultAsync(p => p.Email == loginDTO.Email);

                if (participant == null || !BCrypt.Net.BCrypt.Verify(loginDTO.Password, participant.Password))
                    return ServiceResponse<ParticipantLoginResponseDTO>.FailureResponse("Invalid email or password.");

                var token = _tokenService.GenerateToken(participant);
                var participantDTO = _mapper.Map<ParticipantDTO>(participant);

                var response = new ParticipantLoginResponseDTO
                {
                    Token = token,
                    Participant = participantDTO
                };

                _logger.LogInformation("Participant logged in: {Email}", participant.Email);

                return ServiceResponse<ParticipantLoginResponseDTO>.SuccessResponse(response, "Login successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during participant login");
                return ServiceResponse<ParticipantLoginResponseDTO>.FailureResponse("An error occurred during login.");
            }
        }
    }
}
