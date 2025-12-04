using EventManagement.Controllers.Participants;
using EventManagement.DTOs;
using EventManagement.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace EventManagement.Tests.Controllers
{
    public class ParticipantsControllerTests
    {
        private readonly Mock<IParticipantService> _mockParticipantService;
        private readonly ParticipantsController _controller;

        public ParticipantsControllerTests()
        {
            _mockParticipantService = new Mock<IParticipantService>();
            _controller = new ParticipantsController(_mockParticipantService.Object);
        }

        [Fact]
        public async Task GetAllParticipants_ReturnsOk()
        {
            // Arrange
            var participants = new List<ParticipantDTO>
            {
                new ParticipantDTO
                {
                    ParticipantId = 1,
                    FullName = "Test Participant",
                    Email = "participant@test.com",
                    PhoneNumber = "1234567890",
                    Role = "User"
                }
            };

            _mockParticipantService
                .Setup(s => s.GetAllParticipantsAsync())
                .ReturnsAsync(ServiceResponse<IEnumerable<ParticipantDTO>>.SuccessResponse(participants));

            // Act
            var result = await _controller.GetAllParticipants();

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task CreateParticipant_WithValidData_ReturnsCreated()
        {
            // Arrange
            var participantDto = new ParticipantDTO
            {
                ParticipantId = 1,
                FullName = "New Participant",
                Email = "new@test.com",
                PhoneNumber = "1234567890",
                Password = "Password@123",
                Role = "User"
            };

            _mockParticipantService
                .Setup(s => s.CreateParticipantAsync(participantDto))
                .ReturnsAsync(ServiceResponse<ParticipantDTO>.SuccessResponse(participantDto));

            // Act
            var result = await _controller.CreateParticipant(participantDto);

            // Assert
            result.Result.Should().BeOfType<CreatedAtRouteResult>();
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOk()
        {
            // Arrange
            var loginDto = new ParticipantLoginDTO
            {
                Email = "participant@test.com",
                Password = "Password@123"
            };

            var responseDto = new ParticipantLoginResponseDTO
            {
                Token = "test-token",
                Participant = new ParticipantDTO
                {
                    ParticipantId = 1,
                    FullName = "Test Participant",
                    Email = "participant@test.com",
                    PhoneNumber = "1234567890",
                    Role = "User"
                }
            };

            _mockParticipantService
                .Setup(s => s.LoginAsync(loginDto))
                .ReturnsAsync(ServiceResponse<ParticipantLoginResponseDTO>.SuccessResponse(responseDto));

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }
    }
}
