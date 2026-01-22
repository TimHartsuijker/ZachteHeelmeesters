using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Referral
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(10)]
        public string Code { get; set; } = null!;

        [Required]
        public int TreatmentId { get; set; }
        public Treatment Treatment { get; set; } = null!;

        [Required]
        public int PatientId { get; set; }
        public User Patient { get; set; } = null!;

        [Required]
        public int DoctorId { get; set; }
        public User Doctor { get; set; } = null!;

        [StringLength(1000)]
        public string Note { get; set; } = null!;

        [Required]
        public DateTime CreatedAt { get; set; }

        public ICollection<Appointment> Appointments { get; set; } = []; // NP for appointments under this referral
    }
}
