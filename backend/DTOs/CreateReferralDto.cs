using System.ComponentModel.DataAnnotations;

namespace backend.DTOs
{
    public class CreateReferralDto
    {
        [Required]
        public string Code { get; set; } = null!;

        [Required]
        public int TreatmentId { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
