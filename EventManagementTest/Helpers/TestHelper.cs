using AutoMapper;
using EventManagement.Configurations;
using EventManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;

namespace EventManagementTests.Helpers
{
    public static class TestHelper
    {
        /// <summary>
        /// Creates an in-memory database context for testing
        /// </summary>
        public static ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        /// <summary>
        /// Creates a configured AutoMapper instance
        /// </summary>
        public static IMapper GetMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperConfig>();
            });

            return configuration.CreateMapper();
        }

        /// <summary>
        /// Creates a mock logger
        /// </summary>
        public static Mock<ILogger<T>> GetMockLogger<T>()
        {
            return new Mock<ILogger<T>>();
        }

        /// <summary>
        /// Creates a mock configuration with JWT settings
        /// </summary>
        public static IConfiguration GetMockConfiguration()
        {
            var inMemorySettings = new Dictionary<string, string>
            {
                {"Jwt:key", "ThisIsAVerySecretKeyForTestingPurposesOnly123456789"},
                {"Jwt:Issuer", "TestIssuer"}
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
        }

        /// <summary>
        /// Creates a test admin entity
        /// </summary>
        public static Admin CreateTestAdmin(int id = 1, string email = "test@test.com")
        {
            return new Admin
            {
                AdminId = id,
                FullName = "Test Admin",
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test@123"),
                Role = "Admin",
                CreatedAt = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Creates a test event entity
        /// </summary>
        public static Event CreateTestEvent(int id = 1, int adminId = 1, int maxCapacity = 100)
        {
            return new Event
            {
                EventId = id,
                EventName = $"Test Event {id}",
                EventDate = DateTime.UtcNow.AddDays(30),
                Location = "Test Location",
                Description = "Test Description",
                MaxCapacity = maxCapacity,
                CreatedByAdminId = adminId
            };
        }

        /// <summary>
        /// Creates a test participant entity
        /// </summary>
        public static Participant CreateTestParticipant(int id = 1, string email = "participant@test.com")
        {
            return new Participant
            {
                ParticipantId = id,
                FullName = "Test Participant",
                Email = email,
                PhoneNumber = "1234567890",
                Password = BCrypt.Net.BCrypt.HashPassword("Test@123"),
                role = "User"
            };
        }

        /// <summary>
        /// Creates a test registration entity
        /// </summary>
        public static Registration CreateTestRegistration(int id = 1, int eventId = 1, int participantId = 1)
        {
            return new Registration
            {
                RegistrationId = id,
                EventId = eventId,
                ParticipantId = participantId,
                Status = "Confirmed",
                RegisteredAt = DateTime.UtcNow
            };
        }
    }
}