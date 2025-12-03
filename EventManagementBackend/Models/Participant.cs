using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EventManagement.Models
{
    public class Participant
    {
        [Key]
        public int ParticipantId { get; set; }

        [Required]
        [MaxLength(100)]
        public required string FullName { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public required string Email { get; set; }

        [MaxLength(20)]
        public required string PhoneNumber { get; set; }

        public required string role { get; set; }

        [Required]
        [MaxLength(255)]
        public string Password { get; set; }

        // Navigation Property
        public ICollection<Registration> Registrations { get; set; }
        
    }
}
