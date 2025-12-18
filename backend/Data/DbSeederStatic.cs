using backend.Models;

namespace backend.Data
{
    public static class DbSeederStatic
    {
        public static void Seed(AppDbContext context)
        {
            context.Database.EnsureCreated();

            //// Rollen
            //if (!context.Roles.Any())
            //{
            //    context.Roles.AddRange(
            //        new Role { Id = 1, RoleName = "Patient" },
            //    );
            //}

            //// Departments
            //if (!context.Departments.Any())
            //{
            //    context.Departments.AddRange(
            //        new Department { Id = 1, Name = "Orthopedics" },
            //    );
            //}

            //// Specialisms
            //if (!context.Specialisms.Any())
            //{
            //    context.Specialisms.AddRange(
            //        new Specialism { Id = 1, Name = "Knee Surgery" }
            //    );
            //}

            context.SaveChanges();
        }
    }
}
