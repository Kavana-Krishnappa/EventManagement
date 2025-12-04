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
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace EventManagement.Tests.Services
{
    public class ParticipantServiceTests
    {
        private readonly Mock<IEventManagementRepository<Participant>> _mockParticipantRepository;
        private readonly ApplicationDbContext _context;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly IMapper _mapper;
        private readonly Mock<ILogger<ParticipantService>> _mockLogger;
        private readonly ParticipantService _participantService;

        public ParticipantServiceTests()
        {
            _mockParticipantRepository = new Mock<IEventManagementRepository<Participant>>();
            _context = TestHelper.GetInMemoryDbContext();
            _mockTokenService = new Mock<ITokenService>();
            _mapper = TestHelper.GetMapper();
            _mockLogger = TestHelper.GetMockLogger<ParticipantService>();

            _participantService = new ParticipantService(
                _mockParticipantRepository.Object,
                _context,
                _mockTokenService.Object,
                _mapper,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task GetAllParticipantsAsync_ReturnsAllParticipants()
        {
            // Arrange
            var participants = new List<Participant>
            {
                TestHelper.CreateTestParticipant(1, "p1@test.com"),
                TestHelper.CreateTestParticipant(2, "p2@test.com")
            };

            _mockParticipantRepository
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(participants);

            // Act
            var result = await _participantService.GetAllParticipantsAsync();

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetParticipantByIdAsync_WithValidId_ReturnsParticipant()
        {
            // Arrange
            var participant = TestHelper.CreateTestParticipant(1);

            _mockParticipantRepository
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(participant);

            // Act
            var result = await _participantService.GetParticipantByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.ParticipantId.Should().Be(1);
        }

        [Fact]
        public async Task CreateParticipantAsync_WithValidData_ReturnsSuccess()
        {
            // Arrange
            var participantDto = new ParticipantDTO
            {
                FullName = "New Participant",
                Email = "new@test.com",
                PhoneNumber = "1234567890",
                Password = "Password@123",
                Role = "User"
            };

            _mockParticipantRepository
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Participant, bool>>>()))
                .ReturnsAsync(false);

            _mockParticipantRepository
                .Setup(r => r.AddAsync(It.IsAny<Participant>()))
                .ReturnsAsync((Participant p) =>
                {
                    p.ParticipantId = 1;
                    return p;
                });

            // Act
            var result = await _participantService.CreateParticipantAsync(participantDto);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.ParticipantId.Should().Be(1);
        }

        [Fact]
        public async Task CreateParticipantAsync_WithDuplicateEmail_ReturnsFailure()
        {
            // Arrange
            var participantDto = new ParticipantDTO
            {
                FullName = "Test",
                Email = "existing@test.com",
                PhoneNumber = "1234567890",
                Password = "Password@123",
                Role = "User"
            };

            _mockParticipantRepository
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Participant, bool>>>()))
                .ReturnsAsync(true);

            // Act
            var result = await _participantService.CreateParticipantAsync(participantDto);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("already exists");
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ReturnsSuccess()
        {
            // Arrange
            var participant = TestHelper.CreateTestParticipant(1, "participant@test.com");
            var loginDto = new ParticipantLoginDTO
            {
                Email = "participant@test.com",
                Password = "Test@123"
            };

            _mockParticipantRepository
                .Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Participant, bool>>>()))
                .ReturnsAsync(participant);

            _mockTokenService
                .Setup(t => t.GenerateToken(It.IsAny<Participant>()))
                .Returns("test-token");

            // Act
            var result = await _participantService.LoginAsync(loginDto);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Token.Should().Be("test-token");
        }
    }
}