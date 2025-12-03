using EventManagement.DTOs;
using EventManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventManagement.Controllers.Registrations
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationsController : ControllerBase
    {
        private readonly IRegistrationService _registrationService;

        public RegistrationsController(IRegistrationService registrationService)
        {
            _registrationService = registrationService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("event/{id:int}/registrations")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<RegistrationDTO>>> GetRegistrationsForEvent(int id)
        {
            var result = await _registrationService.GetRegistrationsForEventAsync(id);

            if (!result.Success)
                return NotFound(new { message = result.Message });

            return Ok(result.Data);
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPost("event/{id:int}/register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RegistrationDTO>> RegisterParticipant(int id, [FromBody] RegistrationCreateDTO registrationDTO)
        {
            var result = await _registrationService.RegisterParticipantAsync(id, registrationDTO);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return CreatedAtAction(nameof(GetRegistrationsForEvent), new { id = result.Data.EventId }, result.Data);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("registration/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteRegistration(int id)
        {
            var result = await _registrationService.DeleteRegistrationAsync(id);

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