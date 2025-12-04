using EventManagement.Controllers.Admin;
using EventManagement.DTOs;
using EventManagement.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace EventManagement.Tests.Controllers
{
    public class AdminControllerTests
    {
        private readonly Mock<IAdminService> _mockAdminService;
        private readonly AdminController _controller;

        public AdminControllerTests()
        {
            _mockAdminService = new Mock<IAdminService>();
            _controller = new AdminController(_mockAdminService.Object);
        }

        [Fact]
        public async Task Register_WithValidData_ReturnsOk()
        {
            // Arrange
            var adminDto = new AdminCreateDTO
            {
                FullName = "Test Admin",
                Email = "admin@test.com",
                Password = "Password@123",
                Role = "Admin"
            };

            _mockAdminService
                .Setup(s => s.RegisterAsync(adminDto))
                .ReturnsAsync(ServiceResponse<string>.SuccessResponse("Admin registered successfully."));

            // Act
            var result = await _controller.Register(adminDto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Register_WithDuplicateEmail_ReturnsBadRequest()
        {
            // Arrange
            var adminDto = new AdminCreateDTO
            {
                FullName = "Test Admin",
                Email = "existing@test.com",
                Password = "Password@123",
                Role = "Admin"
            };

            _mockAdminService
                .Setup(s => s.RegisterAsync(adminDto))
                .ReturnsAsync(ServiceResponse<string>.FailureResponse("Email already in use."));

            // Act
            var result = await _controller.Register(adminDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOk()
        {
            // Arrange
            var loginDto = new AdminLoginDTO
            {
                Email = "admin@test.com",
                Password = "Password@123"
            };

            var responseDto = new AdminLoginResponseDTO
            {
                Token = "test-token",
                Admin = new AdminDTO
                {
                    AdminId = 1,
                    FullName = "Test Admin",
                    Email = "admin@test.com",
                    Role = "Admin",
                    CreatedAt = "2024-01-01"
                }
            };

            _mockAdminService
                .Setup(s => s.LoginAsync(loginDto))
                .ReturnsAsync(ServiceResponse<AdminLoginResponseDTO>.SuccessResponse(responseDto));

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(responseDto);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var loginDto = new AdminLoginDTO
            {
                Email = "admin@test.com",
                Password = "WrongPassword"
            };

            _mockAdminService
                .Setup(s => s.LoginAsync(loginDto))
                .ReturnsAsync(ServiceResponse<AdminLoginResponseDTO>.FailureResponse("Invalid email or password."));

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>();
        }
    }
}