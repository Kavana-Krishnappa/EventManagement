using EventManagement.Controllers.Events;
using EventManagement.DTOs;
using EventManagement.Services;
using FluentAssertions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace EventManagement.Tests.Controllers
{
    public class EventsControllerTests
    {
        private readonly Mock<IEventService> _mockEventService;
        private readonly EventsController _controller;

        public EventsControllerTests()
        {
            _mockEventService = new Mock<IEventService>();
            _controller = new EventsController(_mockEventService.Object);
        }

        [Fact]
        public async Task GetAllEvents_ReturnsOkWithEvents()
        {
            // Arrange
            var events = new List<EventDTO>
            {
                new EventDTO
                {
                    EventId = 1,
                    EventName = "Event 1",
                    EventDate = DateTime.UtcNow.AddDays(30),
                    Location = "Location 1",
                    Description = "Description 1",
                    MaxCapacity = 100,
                    CreatedByAdminId = 1
                }
            };

            _mockEventService
                .Setup(s => s.GetAllEventsAsync())
                .ReturnsAsync(ServiceResponse<IEnumerable<EventDTO>>.SuccessResponse(events));

            // Act
            var result = await _controller.GetAllEvents();

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(events);
        }

        [Fact]
        public async Task GetEventById_WithValidId_ReturnsOk()
        {
            // Arrange
            var eventDto = new EventDTO
            {
                EventId = 1,
                EventName = "Test Event",
                EventDate = DateTime.UtcNow.AddDays(30),
                Location = "Test Location",
                MaxCapacity = 100,
                CreatedByAdminId = 1
            };

            _mockEventService
                .Setup(s => s.GetEventByIdAsync(1))
                .ReturnsAsync(ServiceResponse<EventDTO>.SuccessResponse(eventDto));

            // Act
            var result = await _controller.GetEventById(1);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetEventById_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            _mockEventService
                .Setup(s => s.GetEventByIdAsync(999))
                .ReturnsAsync(ServiceResponse<EventDTO>.FailureResponse("Event not found"));

            // Act
            var result = await _controller.GetEventById(999);

            // Assert
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task CreateEvent_WithValidData_ReturnsCreated()
        {
            // Arrange
            var eventDto = new EventDTO
            {
                EventId = 1,
                EventName = "New Event",
                EventDate = DateTime.UtcNow.AddDays(30),
                Location = "Test Location",
                MaxCapacity = 100,
                CreatedByAdminId = 1
            };

            _mockEventService
                .Setup(s => s.CreateEventAsync(eventDto))
                .ReturnsAsync(ServiceResponse<EventDTO>.SuccessResponse(eventDto));

            // Act
            var result = await _controller.CreateEvent(eventDto);

            // Assert
            result.Result.Should().BeOfType<CreatedAtRouteResult>();
        }

        [Fact]
        public async Task UpdateEvent_WithValidData_ReturnsNoContent()
        {
            // Arrange
            var patchDoc = new JsonPatchDocument<EventDTO>();
            patchDoc.Replace(e => e.EventName, "Updated Event");

            _mockEventService
                .Setup(s => s.UpdateEventAsync(1, patchDoc))
                .ReturnsAsync(ServiceResponse<bool>.SuccessResponse(true));

            // Act
            var result = await _controller.UpdateEvent(1, patchDoc);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeleteEvent_WithValidId_ReturnsNoContent()
        {
            // Arrange
            _mockEventService
                .Setup(s => s.DeleteEventAsync(1))
                .ReturnsAsync(ServiceResponse<bool>.SuccessResponse(true));

            // Act
            var result = await _controller.DeleteEvent(1);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task GetEventCapacity_WithValidId_ReturnsOk()
        {
            // Arrange
            var capacityDto = new EventCapacityDTO
            {
                EventId = 1,
                MaxCapacity = 100,
                CurrentRegistrations = 50,
                AvailableSpots = 50,
                IsFull = false
            };

            _mockEventService
                .Setup(s => s.GetEventCapacityAsync(1))
                .ReturnsAsync(ServiceResponse<EventCapacityDTO>.SuccessResponse(capacityDto));

            // Act
            var result = await _controller.GetEventCapacity(1);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
        }
    }
}
