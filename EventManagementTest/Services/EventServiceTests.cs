using AutoMapper;
using EventManagement.DTOs;
using EventManagement.Models;
using EventManagement.Repositories;
using EventManagement.Services;
using EventManagementTests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace EventManagement.Tests.Services
{
    public class EventServiceTests
    {
        private readonly Mock<IEventManagementRepository<Event>> _mockEventRepository;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly Mock<ILogger<EventService>> _mockLogger;
        private readonly EventService _eventService;

        public EventServiceTests()
        {
            _mockEventRepository = new Mock<IEventManagementRepository<Event>>();
            _context = TestHelper.GetInMemoryDbContext();
            _mapper = TestHelper.GetMapper();
            _mockLogger = TestHelper.GetMockLogger<EventService>();

            _eventService = new EventService(
                _mockEventRepository.Object,
                _context,
                _mapper,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task GetAllEventsAsync_ReturnsAllEvents()
        {
            // Arrange
            var events = new List<Event>
            {
                TestHelper.CreateTestEvent(1),
                TestHelper.CreateTestEvent(2),
                TestHelper.CreateTestEvent(3)
            };

            _mockEventRepository
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(events);

            // Act
            var result = await _eventService.GetAllEventsAsync();

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().HaveCount(3);
        }

        [Fact]
        public async Task GetEventByIdAsync_WithValidId_ReturnsEvent()
        {
            // Arrange
            var eventItem = TestHelper.CreateTestEvent(1);

            _mockEventRepository
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(eventItem);

            // Act
            var result = await _eventService.GetEventByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.EventId.Should().Be(1);
        }

        [Fact]
        public async Task GetEventByIdAsync_WithInvalidId_ReturnsFailure()
        {
            // Arrange
            _mockEventRepository
                .Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((Event)null);

            // Act
            var result = await _eventService.GetEventByIdAsync(999);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("not found");
        }

        [Fact]
        public async Task CreateEventAsync_WithValidData_ReturnsSuccess()
        {
            // Arrange
            var eventDto = new EventDTO
            {
                EventName = "New Event",
                EventDate = DateTime.UtcNow.AddDays(30),
                Location = "Test Location",
                Description = "Test Description",
                MaxCapacity = 100,
                CreatedByAdminId = 1
            };

            _mockEventRepository
                .Setup(r => r.AddAsync(It.IsAny<Event>()))
                .ReturnsAsync((Event e) =>
                {
                    e.EventId = 1;
                    return e;
                });

            // Act
            var result = await _eventService.CreateEventAsync(eventDto);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.EventId.Should().Be(1);
        }

        [Fact]
        public async Task CreateEventAsync_WithNullData_ReturnsFailure()
        {
            // Act
            var result = await _eventService.CreateEventAsync(null);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("Invalid");
        }

        [Fact]
        public async Task UpdateEventAsync_WithValidData_ReturnsSuccess()
        {
            // Arrange
            var existingEvent = TestHelper.CreateTestEvent(1);
            var patchDoc = new JsonPatchDocument<EventDTO>();
            patchDoc.Replace(e => e.EventName, "Updated Event Name");

            _mockEventRepository
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(existingEvent);

            _mockEventRepository
                .Setup(r => r.UpdateAsync(It.IsAny<Event>()))
                .ReturnsAsync(true);

            // Act
            var result = await _eventService.UpdateEventAsync(1, patchDoc);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task GetEventCapacityAsync_ReturnsCapacityInfo()
        {
            // Arrange
            var eventItem = TestHelper.CreateTestEvent(1, 1, 100);

            _mockEventRepository
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(eventItem);

            _context.Registrations.Add(new Registration
            {
                RegistrationId = 1,
                EventId = 1,
                ParticipantId = 1,
                Status = "Confirmed",
                RegisteredAt = DateTime.UtcNow
            });
            _context.SaveChanges();

            // Act
            var result = await _eventService.GetEventCapacityAsync(1);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.MaxCapacity.Should().Be(100);
            result.Data.CurrentRegistrations.Should().Be(1);
        }

        [Fact]
        public async Task DeleteEventAsync_WithValidId_ReturnsSuccess()
        {
            // Arrange
            _mockEventRepository
                .Setup(r => r.DeleteAsync(1))
                .ReturnsAsync(true);

            // Act
            var result = await _eventService.DeleteEventAsync(1);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
        }
    }
}