using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventManagement.Models
{
    public class Registration
    {
        [Key]
        public int RegistrationId { get; set; }

        [ForeignKey("Event")]
        public int EventId { get; set; }

        [ForeignKey("Participant")]
        public int ParticipantId { get; set; }

        

        public DateTime RegisteredAt { get; set; } = DateTime.Now;

        [Required]
        [MaxLength(50)]
        public string Status { get; set; }

        // Navigation Properties
        public Event Event { get; set; }
        public Participant Participant { get; set; }
        
    }
}
