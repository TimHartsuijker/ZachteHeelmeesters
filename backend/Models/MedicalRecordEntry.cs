using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    [PrimaryKey(nameof(Id), nameof(PatientId))]
    public class MedicalRecordEntry
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }
        public User Patient { get; set; } = null!;

        public int TreatmentId { get; set; }
        public Treatment Treatment { get; set; } = null!;

        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
