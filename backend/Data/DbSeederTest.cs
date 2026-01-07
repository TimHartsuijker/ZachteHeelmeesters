using backend.Models;
using Microsoft.AspNetCore.Identity;

namespace backend.Data
{
    public static class DbSeederTest
    {
        public static void Seed(AppDbContext context)
        {
            context.Database.EnsureCreated();

            var passwordHasher = new PasswordHasher<User>();

            // 🔹 Role seeden
            if (!context.Roles.Any(r => r.RoleName == "Patient"))
            {
                context.Roles.Add(new Role { RoleName = "Patient" });
                context.SaveChanges();
            }

            if (!context.Roles.Any(r => r.RoleName == "Doctor"))
            {
                context.Roles.Add(new Role { RoleName = "Doctor" });
                context.SaveChanges();
            }

            var patientRole = context.Roles.First(r => r.RoleName == "Patient");
            var doctorRole = context.Roles.First(r => r.RoleName == "Doctor");

            // 🔹 Patient user seeden
            if (!context.Users.Any(u => u.Email == "gebruiker@example.com"))
            {
                var user = new User
                {
                    FirstName = "Test",
                    LastName = "Gebruiker",
                    Email = "gebruiker@example.com",
                    StreetName = "Teststraat",
                    HouseNumber = "1",
                    PostalCode = "1234AB",
                    PhoneNumber = "0612345678",
                    CreatedAt = DateTime.UtcNow,
                    RoleId = patientRole.Id
                };

                user.PasswordHash = passwordHasher.HashPassword(user, "Wachtwoord123");

                context.Users.Add(user);
                context.SaveChanges();
            }

            // 🔹 Doctor user seeden
            if (!context.Users.Any(u => u.Email == "testdoctor@example.com"))
            {
                var doctor = new User
                {
                    FirstName = "Test",
                    LastName = "Doctor",
                    Email = "testdoctor@example.com",
                    StreetName = "Teststraat",
                    HouseNumber = "1A",
                    PostalCode = "1234AB",
                    PhoneNumber = "0631234567",
                    CreatedAt = DateTime.UtcNow,
                    RoleId = doctorRole.Id
                };

                doctor.PasswordHash = passwordHasher.HashPassword(doctor, "password");

                context.Users.Add(doctor);
                context.SaveChanges();
            }
        }
    }
}
