using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<DoctorAvailability> DoctorAvailabilities => Set<DoctorAvailability>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DoctorAvailability>(entity =>
            {
                entity.ToTable("doctor_availability");

                entity.HasKey(e => e.AvailabilityId);

                entity.Property(e => e.AvailabilityId).HasColumnName("availability_id");
                entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
                entity.Property(e => e.DateTime).HasColumnName("date_time");
                entity.Property(e => e.IsAvailable).HasColumnName("is_available");
                entity.Property(e => e.Reason).HasColumnName("reason");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.HasIndex(e => new { e.DoctorId, e.DateTime }).IsUnique();
            });
        }
    }
}
