using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using EventManagement.Models;       
using EventManagement.DTOs;            
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using AutoMapper;
using EventManagement.Repositories;
using Microsoft.AspNetCore.Authorization;
using System.Data.Entity;
namespace EventManagement.Controllers.Events
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
       
        private readonly ILogger<EventsController> _logger;
        private readonly IMapper _mapper;
        private readonly IEventManagementRepository<Event> _eventRepository;
        private readonly ApplicationDbContext _dbContext;
        public EventsController(IEventManagementRepository<Event> eventManagementRepository, ILogger<EventsController> logger, IMapper mapper, ApplicationDbContext dbContext)
        {
            _eventRepository = (IEventManagementRepository<Event>)eventManagementRepository;
            _logger = logger;
            _mapper = mapper;
            _dbContext = dbContext;
        }

  
        [Authorize(Roles = "Admin, User")]
        [HttpGet]
        [Route("All", Name = "GetAllEvents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<EventDTO>>> GetAllEventsAsync()
        {
            _logger.LogInformation("Getting all upcoming events");
            var events = await _eventRepository.GetAllAsync();
            var eventDTOData = _mapper.Map<List<EventDTO>>(events);
            return Ok(eventDTOData);
        }

        
        [Authorize(Roles = "Admin, User")]
        [HttpGet]
        [Route("{id:int}", Name = "GetEventById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<EventDTO>>> GetEventById(int id)
        {
            _logger.LogInformation($"Getting event with ID: {id}");

            var ev = await _eventRepository.GetByIdAsync(id);
            if (ev == null)
            {
                _logger.LogWarning($"Event with ID {id} not found.");
                return NotFound(new { message = "Event not found" });
            }

            var eventDTO = _mapper.Map<EventDTO>(ev);

            //var eventDto = new EventDTO
            //{
            //    EventId = ev.EventId,
            //    EventName = ev.EventName,
            //    EventDate = ev.EventDate.ToString("yyyy-MM-dd HH:mm"),
            //    Location = ev.Location,
            //    Description = ev.Description,
            //    MaxCapacity = ev.MaxCapacity,
            //    CreatedByAdminId = ev.CreatedByAdminId
            //};

            return Ok(eventDTO);
        }

        [Authorize(Roles ="Admin")] 
        [HttpPost]
        [Route("create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EventDTO>> CreateEvent([FromBody] EventDTO newEvent)
        {
            _logger.LogInformation("Creating a new event");

            if (newEvent == null)
                return BadRequest(new { message = "Invalid event data" });

             var ev = _mapper.Map<Event>(newEvent);
            await _eventRepository.AddAsync(ev);

            //var ev = new Event
            //{
            //    EventName = newEvent.EventName,
            //    EventDate = DateTime.Parse(newEvent.EventDate),
            //    Location = newEvent.Location,
            //    Description = newEvent.Description,
            //    MaxCapacity = newEvent.MaxCapacity,
            //    CreatedByAdminId = newEvent.CreatedByAdminId
            //};
            newEvent.EventId = ev.EventId;
            return CreatedAtRoute("GetEventById", new { id = ev.EventId }, newEvent);
        }

      
        [Authorize (Roles ="Admin")]
        [HttpPatch]
        [Route("{id:int}", Name = "UpdateEvent")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateEvent(int id, JsonPatchDocument<EventDTO> patchDocument)
        {
            _logger.LogInformation($"Updating event with ID: {id}");

            if (patchDocument == null || id <= 0)
                return BadRequest();

            var existingEvent = await _eventRepository.GetByIdAsync(id);
           

            if (existingEvent == null)
                return NotFound();

            //var eventDTO = new EventDTO
            //{
            //    EventId = existingEvent.EventId,
            //    EventName = existingEvent.EventName,
            //    EventDate = existingEvent.EventDate,
            //    Location = existingEvent.Location,
            //    MaxCapacity = existingEvent.MaxCapacity,
            //    Description = existingEvent.Description

            //};

            var eventDTO = _mapper.Map<EventDTO>(existingEvent);
            patchDocument.ApplyTo(eventDTO, ModelState);


            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _mapper.Map(eventDTO, existingEvent);

            await _eventRepository.UpdateAsync(existingEvent);

            //existingEvent.EventId = eventDTO.EventId;
            //existingEvent.EventName = eventDTO.EventName;
            //existingEvent.EventDate = Convert.ToDateTime(eventDTO.EventDate);
            //existingEvent.Location = eventDTO.Location;
            //existingEvent.MaxCapacity = eventDTO.MaxCapacity;
            //existingEvent.Description = eventDTO.Description;
            //_dbContext.SaveChanges();

            return NoContent();

        }

        

        [HttpGet]
        [Route("{id:int}/capacity", Name = "GetEventCapacity")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetEventCapacity(int id)
        {
            _logger.LogInformation($"Getting capacity info for event {id}");

            var ev = await _eventRepository.GetByIdAsync(id);
            if (ev == null)
            {
                return NotFound(new { message = "Event not found" });
            }

            // Count confirmed registrations
            var registrationCount = _dbContext.Registrations
                .Count(r => r.EventId == id && r.Status == "Confirmed");

            return Ok(new
            {
                eventId = id,
                maxCapacity = ev.MaxCapacity,
                currentRegistrations = registrationCount,
                availableSpots = ev.MaxCapacity - registrationCount,
                isFull = registrationCount >= ev.MaxCapacity
            });
        }


        [Authorize (Roles ="Admin")]
        [HttpDelete]
        [Route("{id:int}", Name = "DeleteEvent")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            _logger.LogInformation($"Deleting event with ID: {id}");

            if (id <= 0)
            {
                return BadRequest(); 
            }

            var deleted = await _eventRepository.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
