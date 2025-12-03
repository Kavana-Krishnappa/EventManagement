using EventManagement.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventManagement.Services
{
    public interface IRegistrationService
    {
        Task<ServiceResponse<IEnumerable<RegistrationDTO>>> GetRegistrationsForEventAsync(int eventId);
        Task<ServiceResponse<RegistrationDTO>> RegisterParticipantAsync(int eventId, RegistrationCreateDTO registrationDTO);
        Task<ServiceResponse<bool>> DeleteRegistrationAsync(int id);
    }
}