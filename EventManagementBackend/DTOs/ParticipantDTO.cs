using System.ComponentModel.DataAnnotations;

namespace EventManagement.DTOs
{
    public class ParticipantDTO


    {

        
        public int ParticipantId { get; set; }

        [Required]

        public required string FullName { get; set; }
        [Required]
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public string Password { get; set; }

        public string Role { get; set; }
    }

    public class ParticipantLoginDTO
    {
        public required string Email { get; set; }

        public string Password { get; set; }

        public string Role { get; set; }



    }
}
