using EventManagement.DTOs;
using System.Threading.Tasks;

namespace EventManagement.Services
{
    public interface IAdminService
    {
        Task<ServiceResponse<string>> RegisterAsync(AdminCreateDTO adminCreateDTO);
        Task<ServiceResponse<AdminLoginResponseDTO>> LoginAsync(AdminLoginDTO adminLoginDTO);
    }
}