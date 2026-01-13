namespace backend.Models
{
    public class DoctorAvailability
    {
        public int AvailabilityId { get; set; }
        public int DoctorId { get; set; }
        public DateTime DateTime { get; set; }
        public bool IsAvailable { get; set; } = true;
        public string? Reason { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
