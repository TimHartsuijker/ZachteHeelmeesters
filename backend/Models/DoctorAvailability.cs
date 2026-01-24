using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace backend.Models
{
    public class DoctorAvailability
    {
        [Key]
        public int AvailabilityId { get; set; }
        [Required]
        public int DoctorId { get; set; }
        public virtual User? Doctor { get; set; }
        [Required]
        public DateTime DateTime { get; set; }
        [Required]
        public bool IsAvailable { get; set; } = true;
        [AllowNull]
        public string? Reason { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
