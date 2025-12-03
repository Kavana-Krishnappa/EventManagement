using AutoMapper;
using EventManagement.Controllers.Events;
using EventManagement.DTOs;
using EventManagement.Models;
using EventManagement.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventManagement.Controllers.Registrations
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class RegistrationsController : ControllerBase
    {
        private readonly ILogger _logger;
   
        private readonly IMapper _mapper;
        private readonly IEventManagementRepository<Registration> _registrationRepository;

       
        private readonly ApplicationDbContext _dbContext;

        public RegistrationsController(IEventManagementRepository<Registration> registrationRepository, ILogger<RegistrationsController> logger, IMapper mapper, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _mapper = mapper;
            _dbContext = dbContext;
            _registrationRepository = registrationRepository;
           

        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("event/{id}/registrations", Name = "getregistrations")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<RegistrationDTO>>> GetRegistrationsForEvent(int id)
        {
            _logger.LogInformation("Fetching registrations for event {EventId}", id);

          
            var ev = await _dbContext.Events.FindAsync(id);
            if (ev == null)
                return NotFound("Event not found");

           
            var registrations = await _dbContext.Registrations
                .Where(r => r.EventId == id)
                .Include(r => r.Participant)  
                .Select(r => new RegistrationDTO
                {
                    RegistrationId = r.RegistrationId,
                    EventId = r.EventId,
                    ParticipantId = r.ParticipantId,
                    Status = r.Status,
                    RegisteredAt = r.RegisteredAt,
                })
                .ToListAsync();

            return Ok(registrations);
        }

        [Authorize(Roles = "Admin, User")]


        [HttpPost]
        [Route("event/{id}/register", Name = "registerparticipant")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RegistrationDTO>> RegisterParticipant(int id, [FromBody] RegistrationCreateDTO registrationCreateDTO)
        {
            _logger.LogInformation("Registering participant for event");

           
            var ev = await _dbContext.Events.FindAsync(id);
            if (ev == null)
                return BadRequest("Event not found");

            var participant = await _dbContext.Participants.FindAsync(registrationCreateDTO.ParticipantId);
            if (participant == null)
                return BadRequest("Participant not found");

            var existingRegistration = _dbContext.Registrations
                .FirstOrDefault(r => r.EventId == id && r.ParticipantId == registrationCreateDTO.ParticipantId);
            if (existingRegistration != null)
                return BadRequest("Participant is already registered for this event");

            //CHECK MAX CAPACITY 
            var currentRegistrations = _dbContext.Registrations
                .Count(r => r.EventId == id && r.Status == "Confirmed");

            if (currentRegistrations >= ev.MaxCapacity)
                return BadRequest(new { message = "Event has reached maximum capacity" });


            var registration = new Registration
            {
                EventId = id,
                ParticipantId = registrationCreateDTO.ParticipantId,
                Status = registrationCreateDTO.Status,
                RegisteredAt = DateTime.UtcNow
            };

            _dbContext.Registrations.Add(registration);
            _dbContext.SaveChanges();

          
            var responseDto = new RegistrationDTO
            {
                RegistrationId = registration.RegistrationId,
                EventId = registration.EventId,
                ParticipantId = registration.ParticipantId,
                Status = registration.Status,
                RegisteredAt = registration.RegisteredAt
            };

            return CreatedAtRoute("getregistrations", new { id = registration.RegistrationId }, responseDto);
        }

        [Authorize(Roles = "Admin")]

        [HttpDelete]
        [Route("registration/{id}", Name = "deleteRegistration")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteRegistrationAsync(int id)
        {
            _logger.LogInformation("Deleting registration with ID {RegistrationId}", id);

            if (id <= 0)
                return BadRequest("Invalid registration ID");

   
            var registration = await _registrationRepository.GetByIdAsync(id);
            if (registration == null)
                return NotFound("Registration not found");

           
            var isDeleted = await _registrationRepository.DeleteAsync(id);
            if (!isDeleted)
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to delete registration");

            return NoContent();
        }



    }
}
