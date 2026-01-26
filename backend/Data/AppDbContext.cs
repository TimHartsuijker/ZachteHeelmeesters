using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        // DbSets voor al je entiteiten
        public DbSet<Treatment> Treatments { get; set; } = null!;
        public DbSet<Department> Departments { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Referral> Referrals { get; set; } = null!;
        public DbSet<MedicalRecordEntry> MedicalRecordEntries { get; set; } = null!;
        public DbSet<MedicalRecordFile> MedicalRecordFiles { get; set; } = null!;
        public DbSet<Appointment> Appointments { get; set; } = null!;
        public DbSet<Specialism> Specialisms { get; set; } = null!;
        public DbSet<MedicalRecordAccess> MedicalRecordAccesses { get; set; } = null!;
        public DbSet<DoctorAvailability> DoctorAvailabilities { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Composite keys
            modelBuilder.Entity<MedicalRecordAccess>()
                .HasKey(mra => new { mra.SpecialistId, mra.PatientId });


            // Treatment Relationships
            modelBuilder.Entity<Treatment>()
                .HasOne(t => t.Specialism)
                .WithMany(s => s.Treatments)
                .HasForeignKey(t => t.SpecialismId)
                .OnDelete(DeleteBehavior.Cascade);

            // Department Relationships
            modelBuilder.Entity<Department>()
                .HasMany(d => d.Specialists)
                .WithOne(u => u.Department)
                .HasForeignKey(u => u.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);

            // Roles Relationships
            modelBuilder.Entity<Role>()
                .HasMany(r => r.Users)
                .WithOne(u => u.Role)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Appointments Relationships
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Referral)
                .WithMany(r => r.Appointments)
                .HasForeignKey(a => a.ReferralId)
                .OnDelete(DeleteBehavior.Restrict);


            // Users Relationships

            // Referrals
            modelBuilder.Entity<User>()
                .HasMany(u => u.ReferralsAsPatient)
                .WithOne(r => r.Patient)
                .HasForeignKey(r => r.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.ReferralsAsDoctor)
                .WithOne(r => r.Doctor)
                .HasForeignKey(r => r.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Specialisms (Many-to-Many)
            modelBuilder.Entity<User>()
                .HasMany(u => u.Specialisms)
                .WithMany(t => t.Specialists)
                .UsingEntity(j => j.ToTable("SpecialismSpecialists"));

            // Patients <-> Doctors
            modelBuilder.Entity<User>()
                .HasMany(u => u.Patients)
                .WithOne(d => d.Doctor)
                .HasForeignKey(p => p.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Doctor)
                .WithMany(d => d.Patients)
                .HasForeignKey(u => u.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Appointments
            modelBuilder.Entity<User>()
                .HasMany(u => u.SpecialistAppointments)
                .WithOne(a => a.Specialist)
                .HasForeignKey(a => a.SpecialistId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.PatientAppointments)
                .WithOne(a => a.Patient)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            // MedicalRecordAccesses
            modelBuilder.Entity<User>()
                .HasMany(u => u.MedicalRecordAccessAsSpecialist)
                .WithOne(mra => mra.Specialist)
                .HasForeignKey(mra => mra.SpecialistId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.MedicalRecordAccessAsPatient)
                .WithOne(mra => mra.Patient)
                .HasForeignKey(mra => mra.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            // MedicalRecordEntries
            modelBuilder.Entity<User>()
                .HasMany(u => u.MedicalRecordEntries)
                .WithOne(mre => mre.Patient)
                .HasForeignKey(mre => mre.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.CreatedMedicalRecordEntries)
                .WithOne(mre => mre.CreatedBy)
                .HasForeignKey(mre => mre.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            // MedicalRecord Files
            modelBuilder.Entity<MedicalRecordEntry>()
                .HasMany(mre => mre.Files)
                .WithOne(mrf => mrf.MedicalRecordEntry)
                .HasForeignKey(mrf => mrf.MedicalRecordEntryId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique Constraints
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.CitizenServiceNumber)
                .IsUnique();

            modelBuilder.Entity<Role>()
                .HasIndex(r => r.RoleName)
                .IsUnique();

            modelBuilder.Entity<Specialism>()
                .HasIndex(s => s.Name)
                .IsUnique();

            modelBuilder.Entity<Referral>()
                .HasIndex(d => d.Code)
                .IsUnique();

            modelBuilder.Entity<Treatment>()
                .Property(t => t.Cost)
                .HasPrecision(18, 2);

            modelBuilder.Entity<DoctorAvailability>()
                .HasIndex(da => new { da.DoctorId, da.DateTime })
                .IsUnique();
        }
    }
}
