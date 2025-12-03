using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventManagement.Models
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }

        [Required]
        [MaxLength(200)]
        public string EventName { get; set; }

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        [MaxLength(255)]
        public string Location { get; set; }

        public string Description { get; set; }

        [Required]
        public int MaxCapacity { get; set; }

        [ForeignKey("Admin")]
        public int CreatedByAdminId { get; set; }

        // Navigation Properties
        public Admin Admin { get; set; }
        public ICollection<Registration>? Registrations { get; set; }
    }
}
