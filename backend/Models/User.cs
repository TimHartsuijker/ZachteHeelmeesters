using System.ComponentModel.DataAnnotations;

// NP = Navigation Property

namespace backend.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string FirstName { get; set; } = null!;

        [Required, MaxLength(100)]
        public string LastName { get; set; } = null!;

        [Required, MaxLength(100)]
        public string Email { get; set; } = null!;

        [Required, MaxLength(100)]
        public string PasswordHash { get; set; } = null!;

        [Required, MaxLength(100)]
        public string StreetName { get; set; } = null!;

        [Required, MaxLength(100)]
        public string HouseNumber { get; set; } = null!;

        [Required, StringLength(6)]
        public string PostalCode { get; set; } = null!;
        
        [Required, StringLength(9), RegularExpression(@"^\d{9}$")]
        public string CitizenServiceNumber { get; set; } = null!;

        [Required]
        public DateTime DateOfBirth { get; set; }

        [AllowedValues("Man", "Vrouw", "Ander", "Zeg ik liever niet")]
        public string Gender { get; set; } = null!;

        [MaxLength(15)]
        public string? PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int RoleId { get; set; }
        public Role Role { get; set; } = null!; // NP

        // For specialists and patients
        public ICollection<Referral> ReferralsAsPatient { get; set; } = [];
        public ICollection<Referral> ReferralsAsDoctor { get; set; } = [];
        public ICollection<Appointment> SpecialistAppointments { get; set; } = [];
        public ICollection<Appointment> PatientAppointments { get; set; } = [];
        public ICollection<MedicalRecordAccess> MedicalRecordAccessAsSpecialist { get; set; } = [];
        public ICollection<MedicalRecordAccess> MedicalRecordAccessAsPatient { get; set; } = [];

        // For patients
        public int? DoctorId { get; set; }
        public User? Doctor { get; set; } // NP for patient's doctor
        public ICollection<MedicalRecordEntry> MedicalRecordEntries { get; set; } = []; // NP for patient's medical records

        // For doctors
        public string? PracticeName { get; set; }
        public ICollection<User> Patients { get; set; } = []; // NP for doctor's patients

        // For specialists
        public int? DepartmentId { get; set; }
        public Department? Department { get; set; } // NP for specialist's department
        public ICollection<Specialism> Specialisms { get; set; } = []; // NP for specialist's treatments
        public ICollection<MedicalRecordEntry> CreatedMedicalRecordEntries { get; set; } = []; // NP for medical records created by specialist
    }
}
