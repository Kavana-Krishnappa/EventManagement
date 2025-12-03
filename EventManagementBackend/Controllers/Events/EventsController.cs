using Microsoft.AspNetCore.Mvc;
using EventManagement.DTOs;
using EventManagement.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Authorization;

namespace EventManagement.Controllers.Events
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet("All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<EventDTO>>> GetAllEvents()
        {
            var result = await _eventService.GetAllEventsAsync();

            if (!result.Success)
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = result.Message });

            return Ok(result.Data);
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet("{id:int}", Name = "GetEventById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EventDTO>> GetEventById(int id)
        {
            var result = await _eventService.GetEventByIdAsync(id);

            if (!result.Success)
                return NotFound(new { message = result.Message });

            return Ok(result.Data);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EventDTO>> CreateEvent([FromBody] EventDTO newEvent)
        {
            var result = await _eventService.CreateEventAsync(newEvent);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return CreatedAtRoute("GetEventById", new { id = result.Data.EventId }, result.Data);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateEvent(int id, JsonPatchDocument<EventDTO> patchDocument)
        {
            var result = await _eventService.UpdateEventAsync(id, patchDocument);

            if (!result.Success)
            {
                if (result.Message.Contains("not found"))
                    return NotFound(new { message = result.Message });

                return BadRequest(new { message = result.Message });
            }

            return NoContent();
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet("{id:int}/capacity")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EventCapacityDTO>> GetEventCapacity(int id)
        {
            var result = await _eventService.GetEventCapacityAsync(id);

            if (!result.Success)
                return NotFound(new { message = result.Message });

            return Ok(result.Data);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var result = await _eventService.DeleteEventAsync(id);

            if (!result.Success)
            {
                if (result.Message.Contains("not found"))
                    return NotFound(new { message = result.Message });

                return BadRequest(new { message = result.Message });
            }

            return NoContent();
        }
    }
}