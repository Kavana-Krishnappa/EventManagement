using EventManagement.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventManagement.Services
{
    public interface IParticipantService
    {
        Task<ServiceResponse<IEnumerable<ParticipantDTO>>> GetAllParticipantsAsync();
        Task<ServiceResponse<ParticipantDTO>> GetParticipantByIdAsync(int id);
        Task<ServiceResponse<ParticipantDTO>> CreateParticipantAsync(ParticipantDTO participantDTO);
        Task<ServiceResponse<IEnumerable<EventDTO>>> GetUpcomingEventsForParticipantAsync(int participantId);
        Task<ServiceResponse<IEnumerable<EventDTO>>> GetPreviousEventsForParticipantAsync(int participantId);
        Task<ServiceResponse<ParticipantLoginResponseDTO>> LoginAsync(ParticipantLoginDTO loginDTO);
    }
}