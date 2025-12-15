using backend.Models;

namespace backend.Data
{
    public static class DbSeederTest
    {
        public static void Seed(AppDbContext context)
        {
            context.Database.EnsureCreated();

            //// Alleen seeden als je testdata wilt
            //if (!context.Users.Any(u => u.RoleId == 4)) // bv. patient-role
            //{
            //    context.Users.AddRange(
            //        new User { Id = 100, FirstName = "Test", LastName = "Patient", Email = "test@patient.com", PasswordHash = "hashed", StreetName = "TestStreet", HouseNumber = "1", PostalCode = "1234AB", RoleId = 4, DoctorId = 2 }
            //    );

            //    context.SaveChanges();
            //}

            context.SaveChanges();
        }
    }
}
