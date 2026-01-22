using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class MedicalRecordEntry
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }
        public User Patient { get; set; } = null!;

        public int? AppointmentId { get; set; }
        public Appointment? Appointment { get; set; }

        [Required]
        public int CreatedById { get; set; }
        public User CreatedBy { get; set; } = null!;

        [Required]
        public DateTime CreatedAt { get; set; }

        // Entry details
        [MaxLength(200)]
        public string? Title { get; set; }

        public string? Notes { get; set; }

        [MaxLength(100)]
        public string? Category { get; set; }

        public ICollection<MedicalRecordFile> Files { get; set; } = [];
    }
}
