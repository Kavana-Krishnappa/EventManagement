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
    public class RegistrationServiceTests
    {
        private readonly Mock<IEventManagementRepository<Registration>> _mockRegistrationRepository;
        private readonly Mock<IEventManagementRepository<Event>> _mockEventRepository;
        private readonly Mock<IEventManagementRepository<Participant>> _mockParticipantRepository;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly Mock<ILogger<RegistrationService>> _mockLogger;
        private readonly RegistrationService _registrationService;

        public RegistrationServiceTests()
        {
            _mockRegistrationRepository = new Mock<IEventManagementRepository<Registration>>();
            _mockEventRepository = new Mock<IEventManagementRepository<Event>>();
            _mockParticipantRepository = new Mock<IEventManagementRepository<Participant>>();
            _context = TestHelper.GetInMemoryDbContext();
            _mapper = TestHelper.GetMapper();
            _mockLogger = TestHelper.GetMockLogger<RegistrationService>();

            _registrationService = new RegistrationService(
                _mockRegistrationRepository.Object,
                _mockEventRepository.Object,
                _mockParticipantRepository.Object,
                _context,
                _mapper,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task GetRegistrationsForEventAsync_WithValidEventId_ReturnsRegistrations()
        {
            // Arrange
            var eventItem = TestHelper.CreateTestEvent(1);

            _mockEventRepository
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(eventItem);

            _context.Registrations.Add(TestHelper.CreateTestRegistration(1, 1, 1));
            _context.SaveChanges();

            // Act
            var result = await _registrationService.GetRegistrationsForEventAsync(1);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeEmpty();
        }

        [Fact]
        public async Task RegisterParticipantAsync_WithValidData_ReturnsSuccess()
        {
            // Arrange
            var eventItem = TestHelper.CreateTestEvent(1, 1, 100);
            var participant = TestHelper.CreateTestParticipant(1);
            var registrationDto = new RegistrationCreateDTO
            {
                EventId = 1,
                ParticipantId = 1,
                Status = "Confirmed"
            };

            _mockEventRepository
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(eventItem);

            _mockParticipantRepository
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(participant);

            _mockRegistrationRepository
                .Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Registration, bool>>>()))
                .ReturnsAsync((Registration)null);

            _mockRegistrationRepository
                .Setup(r => r.AddAsync(It.IsAny<Registration>()))
                .ReturnsAsync((Registration reg) =>
                {
                    reg.RegistrationId = 1;
                    return reg;
                });

            // Act
            var result = await _registrationService.RegisterParticipantAsync(1, registrationDto);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
        }

        
        [Fact]
        public async Task DeleteRegistrationAsync_WithValidId_ReturnsSuccess()
        {
            // Arrange
            var registration = TestHelper.CreateTestRegistration(1);

            _mockRegistrationRepository
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(registration);

            _mockRegistrationRepository
                .Setup(r => r.DeleteAsync(1))
                .ReturnsAsync(true);

            // Act
            var result = await _registrationService.DeleteRegistrationAsync(1);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
        }
    }
}