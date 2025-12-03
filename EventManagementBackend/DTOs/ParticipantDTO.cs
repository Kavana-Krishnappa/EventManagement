using System.ComponentModel.DataAnnotations;

namespace EventManagement.DTOs
{
    public class ParticipantDTO
    {
        public int ParticipantId { get; set; }

        [Required]
        [MaxLength(100)]
        public required string FullName { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public required string Email { get; set; }

        [Required]
        [MaxLength(20)]
        public required string PhoneNumber { get; set; }

        [Required]
        [MaxLength(255)]
        public string Password { get; set; }

        [Required]
        [MaxLength(50)]
        public string Role { get; set; }
    }

    public class ParticipantLoginDTO
    {
        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public required string Email { get; set; }

        [Required]
        [MaxLength(255)]
        public string Password { get; set; }

        public string Role { get; set; }
    }

    public class ParticipantLoginResponseDTO
    {
        public string Token { get; set; }
        public ParticipantDTO Participant { get; set; }
    }
}
