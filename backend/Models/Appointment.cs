using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    [PrimaryKey(nameof(SpecialistId), nameof(AppointmentDateTime))]
    public class Appointment
    {
        [Required]
        public int SpecialistId { get; set; }

        [Required]
        public DateTime AppointmentDateTime { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        public int TreatmentId { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        public Department Department { get; set; } = null!; // Navigation property
        public Treatment Treatment { get; set; } = null!; // Navigation property
        public User Specialist { get; set; } = null!; // Navigation property
        public User Patient { get; set; } = null!; // Navigation property
    }
}
