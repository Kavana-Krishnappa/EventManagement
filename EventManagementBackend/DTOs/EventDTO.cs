using System;
using System.ComponentModel.DataAnnotations;

namespace EventManagement.DTOs
{
    public class EventDTO
    {
        public int EventId { get; set; }

        [Required]
        [MaxLength(200)]
        public required string EventName { get; set; }

        [Required]
        public required DateTime EventDate { get; set; }

        [Required]
        [MaxLength(255)]
        public required string Location { get; set; }

        public string? Description { get; set; }

        [Required]
        public int MaxCapacity { get; set; }

        [Required]
        public int CreatedByAdminId { get; set; }
    }

    public class EventCapacityDTO
    {
        public int EventId { get; set; }
        public int MaxCapacity { get; set; }
        public int CurrentRegistrations { get; set; }
        public int AvailableSpots { get; set; }
        public bool IsFull { get; set; }
    }
}