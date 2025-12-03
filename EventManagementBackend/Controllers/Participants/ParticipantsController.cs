using AutoMapper;
using EventManagement.DTOs;
using EventManagement.Models;
using EventManagement.Repositories;
using EventManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;




namespace EventManagement.Controllers.Participants
{

    [Route("api/[controller]")]
    [ApiController]
    public class ParticipantsController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IEventManagementRepository<Participant> _participantRepository;
        private readonly ApplicationDbContext _dbContext;
        private readonly ITokenService _tokenService;
        public ParticipantsController(IConfiguration configuration, ILogger<ParticipantsController> logger, IEventManagementRepository<Participant> participantRepository, IMapper mapper, ApplicationDbContext dbContext, ITokenService tokenService
            )
        {
            _configuration = configuration;
            _logger = logger;
            _mapper = mapper;
            _participantRepository = participantRepository;
            _dbContext = dbContext;
            _tokenService = tokenService;

        }

        [Authorize (Roles ="Admin")]

        [HttpGet]
        [Route("All", Name = "GetAllParticipants")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ParticipantDTO>>> GetAllParticipants()
        {
            _logger.LogInformation("Getting all participants");

            var participants = await _participantRepository.GetAllAsync();
            var participantDTOdata = _mapper.Map<List<ParticipantDTO>>(participants);


            return Ok(participantDTOdata);
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet]
        [Route("id", Name = "GetParticipantById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ParticipantDTO>> GetParticipantById(int id)
        {
            _logger.LogInformation("Getting participant by Id");
            var ev = await _participantRepository.GetByIdAsync(id);
            if (ev == null)
                return NotFound();
            return Ok(ev);
        }

        [HttpPost]
        [Route("SignUp")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<ParticipantDTO>> CreateParticipant([FromBody] ParticipantDTO participantCreateDTO)
        {
            _logger.LogInformation("Creating a new participant");
            
                
                var isDuplicate = await _participantRepository.ExistsAsync(p => p.Email == participantCreateDTO.Email);
                if (isDuplicate)
                {
                    return Conflict(new { message = "A participant with this email already exists." });
                }
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(participantCreateDTO.Password);

            var participant = _mapper.Map<Participant>(participantCreateDTO);
            participant.Password = hashedPassword;
            await _participantRepository.AddAsync(participant);

                return CreatedAtAction(nameof(GetParticipantById), new { id = participant.ParticipantId }, participant);
            }


        [Authorize(Roles = "Admin, User")]
        [HttpGet]
        [Route("{participantId}/upcoming-events", Name = "getupcomingeventsforparticipant")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<EventDTO>>> GetUpcomingEventsForParticipant(int participantId)
        {
            _logger.LogInformation("Fetching upcoming events for participant {ParticipantId}", participantId);

            var participant = await _dbContext.Participants.FindAsync(participantId);
            if (participant == null)
                return NotFound("Participant not found");

            var upcomingEvents = await _dbContext.Registrations
                .Where(r => r.ParticipantId == participantId)
                .Include(r => r.Event)
                .Where(r => r.Event.EventDate >= DateTime.UtcNow)  // only future events
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

            return Ok(upcomingEvents);
        }
        [Authorize(Roles = "Admin, User")]
        [HttpGet]
        [Route("{participantId}/previous-events", Name = "getpreviouseventsforparticipant")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<EventDTO>>> GetPreviousEventsForParticipant(int participantId)
        {
            _logger.LogInformation("Fetching preivous events for participant {ParticipantId}", participantId);

            var participant = await _dbContext.Participants.FindAsync(participantId);
            if (participant == null)
                return NotFound("Participant not found");

            var previousEvents = await _dbContext.Registrations
                .Where(r => r.ParticipantId == participantId)
                .Include(r => r.Event)
                .Where(r => r.Event.EventDate < DateTime.UtcNow)  // only past events
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

            return Ok(previousEvents);
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] ParticipantLoginDTO participantLoginDTO)
        {
            // Find participant by email
            var participant = await _dbContext.Participants
                .FirstOrDefaultAsync(p => p.Email == participantLoginDTO.Email);

            if (participant == null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

           
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(participantLoginDTO.Password, participant.Password);

            if (!isPasswordValid)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }


            var token = _tokenService.GenerateToken(participant);

            return Ok(new
            {
                token,
                participant = new
                {
                    participant.ParticipantId,
                    participant.FullName,
                    participant.Email,
                    participant.role
                }
            });
        }









        
    }
}
