using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    [PrimaryKey(nameof(SpecialistId), nameof(PatientId))]
    public class MedicalRecordAccess
    {
        [Required]
        public int SpecialistId { get; set; }
        public User Specialist { get; set; } = null!; // NP

        [Required]
        public int PatientId { get; set; }
        public User Patient { get; set; } = null!; // NP

        [Required]
        public Boolean AccessGranted { get; set; } = false;
    }
}
