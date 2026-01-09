using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime AppointmentDateTime { get; set; } = DateTime.UtcNow;

        [Required]
        public int ReferralId { get; set; }
        public Referral Referral { get; set; } = null!; // Navigation property

        [Required]
        public int SpecialistId { get; set; }
        public User Specialist { get; set; } = null!; // Navigation property

        [Required]
        public int PatientId { get; set; }
        public User Patient { get; set; } = null!; // Navigation property

        [Required]
        public int TreatmentId { get; set; }
        public Treatment Treatment { get; set; } = null!; // Navigation property

        [Required]
        public int DepartmentId { get; set; }
        public Department Department { get; set; } = null!; // Navigation property
    }
}
