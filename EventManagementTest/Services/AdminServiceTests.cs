using AutoMapper;
using EventManagement.DTOs;
using EventManagement.Models;
using EventManagement.Repositories;
using EventManagement.Services;
using EventManagementTests.Helpers;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace EventManagement.Tests.Services
{
    public class AdminServiceTests
    {
        private readonly Mock<IEventManagementRepository<Admin>> _mockAdminRepository;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly IMapper _mapper;
        private readonly Mock<ILogger<AdminService>> _mockLogger;
        private readonly AdminService _adminService;

        public AdminServiceTests()
        {
            _mockAdminRepository = new Mock<IEventManagementRepository<Admin>>();
            _mockTokenService = new Mock<ITokenService>();
            _mapper = TestHelper.GetMapper();
            _mockLogger = TestHelper.GetMockLogger<AdminService>();

            _adminService = new AdminService(
                _mockAdminRepository.Object,
                _mockTokenService.Object,
                _mapper,
                _mockLogger.Object
            );
        }

      

        [Fact]
        public async Task RegisterAsync_WithDuplicateEmail_ReturnsFailure()
        {
            // Arrange
            var existingAdmin = TestHelper.CreateTestAdmin(1, "existing@test.com");
            var adminCreateDto = new AdminCreateDTO
            {
                FullName = "Jane Doe",
                Email = "existing@test.com",
                Password = "Password@123",
                Role = "Admin"
            };

            _mockAdminRepository
                .Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Admin, bool>>>()))
                .ReturnsAsync(existingAdmin);

            // Act
            var result = await _adminService.RegisterAsync(adminCreateDto);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("already in use");
            _mockAdminRepository.Verify(r => r.AddAsync(It.IsAny<Admin>()), Times.Never);
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ReturnsSuccess()
        {
            // Arrange
            var admin = TestHelper.CreateTestAdmin(1, "admin@test.com");
            var loginDto = new AdminLoginDTO
            {
                Email = "admin@test.com",
                Password = "Test@123"
            };

            _mockAdminRepository
                .Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Admin, bool>>>()))
                .ReturnsAsync(admin);

            _mockTokenService
                .Setup(t => t.GenerateToken(It.IsAny<Admin>()))
                .Returns("test-jwt-token");

            // Act
            var result = await _adminService.LoginAsync(loginDto);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Token.Should().Be("test-jwt-token");
            result.Data.Admin.Should().NotBeNull();
            result.Data.Admin.Email.Should().Be("admin@test.com");
        }

      

        [Fact]
        public async Task LoginAsync_WithInvalidPassword_ReturnsFailure()
        {
            // Arrange
            var admin = TestHelper.CreateTestAdmin(1, "admin@test.com");
            var loginDto = new AdminLoginDTO
            {
                Email = "admin@test.com",
                Password = "WrongPassword"
            };

            _mockAdminRepository
                .Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Admin, bool>>>()))
                .ReturnsAsync(admin);

            // Act
            var result = await _adminService.LoginAsync(loginDto);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("Invalid email or password");
        }
    }
}