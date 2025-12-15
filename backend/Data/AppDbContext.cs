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
        public DbSet<MedicalRecordEntry> MedicalRecordEntries { get; set; } = null!;
        public DbSet<Appointment> Appointments { get; set; } = null!;
        public DbSet<Specialism> Specialisms { get; set; } = null!;
        public DbSet<MedicalRecordAccess> MedicalRecordAccesses { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Composite keys
            modelBuilder.Entity<Appointment>()
                .HasKey(a => new { a.SpecialistId, a.AppointmentDateTime });

            modelBuilder.Entity<MedicalRecordAccess>()
                .HasKey(mra => new { mra.SpecialistId, mra.PatientId });

            // Relaties User ↔ Doctor ↔ Patients
            modelBuilder.Entity<User>()
                .HasOne(u => u.Doctor)
                .WithMany(d => d.Patients)
                .HasForeignKey(u => u.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Optioneel: unieke constraints
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Role>()
                .HasIndex(r => r.RoleName)
                .IsUnique();

            modelBuilder.Entity<Specialism>()
                .HasIndex(s => s.Name)
                .IsUnique();


            modelBuilder.Entity<Treatment>()
                .Property(t => t.Cost)
                .HasPrecision(18, 2);

            // User -> Doctor (self reference)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Doctor)
                .WithMany(d => d.Patients)
                .HasForeignKey(u => u.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Specialism -> Treatment
            modelBuilder.Entity<Specialism>()
                .HasMany(s => s.Treatments)
                .WithOne(t => t.Specialism)
                .HasForeignKey(t => t.SpecialismId)
                .OnDelete(DeleteBehavior.Cascade);

            // Appointment -> Specialist (User)
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Specialist)
                .WithMany(u => u.SpecialistAppointments)
                .HasForeignKey(a => a.SpecialistId)
                .OnDelete(DeleteBehavior.Restrict);

            // Appointment -> Patient (User)
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(u => u.PatientAppointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Specialist navigatie
            modelBuilder.Entity<MedicalRecordAccess>()
                .HasOne(mra => mra.Specialist)
                .WithMany(u => u.MedicalRecordAccessAsSpecialist)
                .HasForeignKey(mra => mra.SpecialistId)
                .OnDelete(DeleteBehavior.Restrict);

            // Patient navigatie
            modelBuilder.Entity<MedicalRecordAccess>()
                .HasOne(mra => mra.Patient)
                .WithMany(u => u.MedicalRecordAccessAsPatient)
                .HasForeignKey(mra => mra.PatientId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
