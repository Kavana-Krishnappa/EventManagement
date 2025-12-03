using System;
using System.ComponentModel.DataAnnotations;

namespace EventManagement.DTOs
{
    public class RegistrationDTO
    {
        public int RegistrationId { get; set; }

        [Required]
        public int EventId { get; set; }

        [Required]
        public int ParticipantId { get; set; }

        public DateTime RegisteredAt { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty;
    }

    public class RegistrationCreateDTO
    {
        [Required]
        public int EventId { get; set; }

        [Required]
        public int ParticipantId { get; set; }

        [Required]
        [MaxLength(50)]
        public required string Status { get; set; }
    }
}