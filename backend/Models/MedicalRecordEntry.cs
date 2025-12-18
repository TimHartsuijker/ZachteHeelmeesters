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

        [Required]
        public int AppointmentId { get; set; }
        public Appointment Appointment { get; set; } = null!;

        [Required]
        public int CreatedById { get; set; }
        public User CreatedBy { get; set; } = null!;

        [Required]
        public DateTime CreatedAt { get; set; }

        public ICollection<MedicalRecordFile> Files { get; set; } = [];
    }
}
