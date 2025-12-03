using AutoMapper;
using BCrypt.Net;
using EventManagement.DTOs;
using EventManagement.Models;
using EventManagement.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace EventManagement.Services
{
    public class AdminService : IAdminService
    {
        private readonly IEventManagementRepository<Admin> _adminRepository;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly ILogger<AdminService> _logger;

        public AdminService(
            IEventManagementRepository<Admin> adminRepository,
            ITokenService tokenService,
            IMapper mapper,
            ILogger<AdminService> logger)
        {
            _adminRepository = adminRepository;
            _tokenService = tokenService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResponse<string>> RegisterAsync(AdminCreateDTO adminCreateDTO)
        {
            try
            {
                var existingAdmin = await _adminRepository
                    .GetFirstOrDefaultAsync(a => a.Email.ToLower() == adminCreateDTO.Email.ToLower());

                if (existingAdmin != null)
                    return ServiceResponse<string>.FailureResponse("Email already in use.");

                var admin = new Admin
                {
                    FullName = adminCreateDTO.FullName,
                    Email = adminCreateDTO.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminCreateDTO.Password),
                    Role = adminCreateDTO.Role,
                    CreatedAt = DateTime.UtcNow
                };

                await _adminRepository.AddAsync(admin);

                _logger.LogInformation("Admin registered: {Email}", admin.Email);

                return ServiceResponse<string>.SuccessResponse("Admin registered successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering admin");
                return ServiceResponse<string>.FailureResponse("An error occurred during registration.");
            }
        }

        public async Task<ServiceResponse<AdminLoginResponseDTO>> LoginAsync(AdminLoginDTO adminLoginDTO)
        {
            try
            {
                var admin = await _adminRepository
                    .GetFirstOrDefaultAsync(a => a.Email == adminLoginDTO.Email);

                if (admin == null || !BCrypt.Net.BCrypt.Verify(adminLoginDTO.Password, admin.PasswordHash))
                    return ServiceResponse<AdminLoginResponseDTO>.FailureResponse("Invalid email or password.");

                var token = _tokenService.GenerateToken(admin);
                var adminDTO = _mapper.Map<AdminDTO>(admin);

                var response = new AdminLoginResponseDTO
                {
                    Token = token,
                    Admin = adminDTO
                };

                _logger.LogInformation("Admin logged in: {Email}", admin.Email);

                return ServiceResponse<AdminLoginResponseDTO>.SuccessResponse(response, "Login successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during admin login");
                return ServiceResponse<AdminLoginResponseDTO>.FailureResponse("An error occurred during login.");
            }
        }
    }
}