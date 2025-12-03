namespace EventManagement.DTOs
{
    public class RegistrationCreateDTO
    {
        public int EventId { get; set; }
        public int ParticipantId { get; set; }
        public required string Status { get; set; }
    }

    public class RegistrationDTO
    {
        public int RegistrationId { get; set; }
        public int EventId { get; set; }
        public int ParticipantId { get; set; }
        public DateTime RegisteredAt { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
