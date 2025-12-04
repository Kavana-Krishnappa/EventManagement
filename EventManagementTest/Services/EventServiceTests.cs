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