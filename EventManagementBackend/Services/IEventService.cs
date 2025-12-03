using EventManagement.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;

namespace EventManagement.Services
{
    public interface IEventService
    {
        Task<ServiceResponse<IEnumerable<EventDTO>>> GetAllEventsAsync();
        Task<ServiceResponse<EventDTO>> GetEventByIdAsync(int id);
        Task<ServiceResponse<EventDTO>> CreateEventAsync(EventDTO eventDTO);
        Task<ServiceResponse<bool>> UpdateEventAsync(int id, JsonPatchDocument<EventDTO> patchDocument);
        Task<ServiceResponse<EventCapacityDTO>> GetEventCapacityAsync(int id);
        Task<ServiceResponse<bool>> DeleteEventAsync(int id);
    }
}