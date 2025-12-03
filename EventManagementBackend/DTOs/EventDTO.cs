namespace EventManagement.DTOs
{
    public class EventDTO
    {
        public int EventId { get; set; }
        public required string EventName { get; set; }
        public required DateTime EventDate { get; set; }    
        public required string Location { get; set; }
        public string? Description { get; set; }
        public int MaxCapacity { get; set; }
        public int CreatedByAdminId { get; set; }

        
    }
}
