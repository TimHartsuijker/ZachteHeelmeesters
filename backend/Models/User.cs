using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

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
        [Required]
        //[Required, MaxLength(100)]
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
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int RoleId { get; set; }
        public Role Role { get; set; } = null!; // NP

        public ICollection<Referral> ReferralsAsPatient { get; set; } = new List<Referral>();
        public ICollection<Referral> ReferralsAsDoctor { get; set; } = new List<Referral>();
        public ICollection<Appointment> SpecialistAppointments { get; set; } = new List<Appointment>();
        public ICollection<Appointment> PatientAppointments { get; set; } = new List<Appointment>();
        public ICollection<MedicalRecordAccess> MedicalRecordAccessAsSpecialist { get; set; } = new List<MedicalRecordAccess>();
        public ICollection<MedicalRecordAccess> MedicalRecordAccessAsPatient { get; set; } = new List<MedicalRecordAccess>();

        public int? DoctorId { get; set; }
        public User? Doctor { get; set; }
        public ICollection<MedicalRecordEntry> MedicalRecordEntries { get; set; } = new List<MedicalRecordEntry>();

        public string? PracticeName { get; set; }
        public ICollection<User> Patients { get; set; } = new List<User>();

        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }
        public ICollection<Specialism> Specialisms { get; set; } = new List<Specialism>();
        public ICollection<MedicalRecordEntry> CreatedMedicalRecordEntries { get; set; } = new List<MedicalRecordEntry>();
    }
}
