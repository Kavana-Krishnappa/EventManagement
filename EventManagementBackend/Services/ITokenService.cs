using EventManagement.Models;

namespace EventManagement.Services
{
    public interface ITokenService
    {
        string GenerateToken(Admin admin);
        string GenerateToken(Participant participant);
    }
}