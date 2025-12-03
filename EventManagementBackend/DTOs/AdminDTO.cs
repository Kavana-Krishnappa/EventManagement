using System.ComponentModel.DataAnnotations;

namespace EventManagement.DTOs
{
    public class AdminDTO
    {
       public int AdminId { get; set; }
        public required string FullName { get; set; }

        public required string Email { get; set; }
        public required string Role { get; set; }
        public  required string CreatedAt { get; set; }



    }

    public class AdminCreateDTO
    {
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string Role { get; set; }
    }

    public class AdminLoginDTO
    {
        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public required string Email { get; set; }

        [Required]
        [MaxLength(255)]
        public required string Password { get; set; }
    }
}
