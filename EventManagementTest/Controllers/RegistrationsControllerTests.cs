using EventManagement.Controllers.Registrations;
using EventManagement.DTOs;
using EventManagement.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace EventManagement.Tests.Controllers
{
    public class RegistrationsControllerTests
    {
        private readonly Mock<IRegistrationService> _mockRegistrationService;
        private readonly RegistrationsController _controller;

        public RegistrationsControllerTests()
        {
            _mockRegistrationService = new Mock<IRegistrationService>();
            _controller = new RegistrationsController(_mockRegistrationService.Object);
        }

        [Fact]
        public async Task GetRegistrationsForEvent_WithValidId_ReturnsOk()
        {
            // Arrange
            var registrations = new List<RegistrationDTO>
            {
                new RegistrationDTO
                {
                    RegistrationId = 1,
                    EventId = 1,
                    ParticipantId = 1,
                    Status = "Confirmed",
                    RegisteredAt = DateTime.UtcNow
                }
            };

            _mockRegistrationService
                .Setup(s => s.GetRegistrationsForEventAsync(1))
                .ReturnsAsync(ServiceResponse<IEnumerable<RegistrationDTO>>.SuccessResponse(registrations));

            // Act
            var result = await _controller.GetRegistrationsForEvent(1);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task RegisterParticipant_WithValidData_ReturnsCreated()
        {
            // Arrange
            var registrationDto = new RegistrationCreateDTO
            {
                EventId = 1,
                ParticipantId = 1,
                Status = "Confirmed"
            };

            var responseDto = new RegistrationDTO
            {
                RegistrationId = 1,
                EventId = 1,
                ParticipantId = 1,
                Status = "Confirmed",
                RegisteredAt = DateTime.UtcNow
            };

            _mockRegistrationService
                .Setup(s => s.RegisterParticipantAsync(1, registrationDto))
                .ReturnsAsync(ServiceResponse<RegistrationDTO>.SuccessResponse(responseDto));

            // Act
            var result = await _controller.RegisterParticipant(1, registrationDto);

            // Assert
            result.Result.Should().BeOfType<CreatedAtActionResult>();
        }

    }
}