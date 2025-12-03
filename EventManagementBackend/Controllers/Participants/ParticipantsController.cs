using EventManagement.DTOs;
using EventManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventManagement.Controllers.Participants
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParticipantsController : ControllerBase
    {
        private readonly IParticipantService _participantService;

        public ParticipantsController(IParticipantService participantService)
        {
            _participantService = participantService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ParticipantDTO>>> GetAllParticipants()
        {
            var result = await _participantService.GetAllParticipantsAsync();

            if (!result.Success)
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = result.Message });

            return Ok(result.Data);
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet("{id:int}", Name = "GetParticipantById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ParticipantDTO>> GetParticipantById(int id)
        {
            var result = await _participantService.GetParticipantByIdAsync(id);

            if (!result.Success)
                return NotFound(new { message = result.Message });

            return Ok(result.Data);
        }

        [HttpPost("SignUp")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ParticipantDTO>> CreateParticipant([FromBody] ParticipantDTO participantDTO)
        {
            var result = await _participantService.CreateParticipantAsync(participantDTO);

            if (!result.Success)
            {
                if (result.Message.Contains("already exists"))
                    return Conflict(new { message = result.Message });

                return BadRequest(new { message = result.Message });
            }

            return CreatedAtRoute("GetParticipantById", new { id = result.Data.ParticipantId }, result.Data);
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet("{participantId:int}/upcoming-events")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<EventDTO>>> GetUpcomingEventsForParticipant(int participantId)
        {
            var result = await _participantService.GetUpcomingEventsForParticipantAsync(participantId);

            if (!result.Success)
                return NotFound(new { message = result.Message });

            return Ok(result.Data);
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet("{participantId:int}/previous-events")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<EventDTO>>> GetPreviousEventsForParticipant(int participantId)
        {
            var result = await _participantService.GetPreviousEventsForParticipantAsync(participantId);

            if (!result.Success)
                return NotFound(new { message = result.Message });

            return Ok(result.Data);
        }

        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] ParticipantLoginDTO loginDTO)
        {
            var result = await _participantService.LoginAsync(loginDTO);

            if (!result.Success)
                return Unauthorized(new { message = result.Message });

            return Ok(result.Data);
        }
    }
}