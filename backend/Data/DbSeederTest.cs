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

            var patientRole = context.Roles.First(r => r.RoleName == "Patient");

            // 🔹 User seeden
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
                    CitizenServiceNumber = "123456789", // uniek maken!
                    DateOfBirth = new DateTime(1990, 1, 1),
                    Gender = "Man",
                    CreatedAt = DateTime.UtcNow,
                    RoleId = patientRole.Id
                };

                user.PasswordHash = passwordHasher.HashPassword(user, "Wachtwoord123");

                context.Users.Add(user);
                context.SaveChanges();
            }

            // 🔹 Admin role seeden
            if (!context.Roles.Any(r => r.RoleName == "Admin"))
            {
                context.Roles.Add(new Role { RoleName = "Admin" });
                context.SaveChanges();
            }

            var adminRole = context.Roles.First(r => r.RoleName == "Admin");

            // 🔹 Admin user seeden
            if (!context.Users.Any(u => u.Email == "admin@example.com"))
            {
                var admin = new User
                {
                    FirstName = "System",
                    LastName = "Administrator",
                    Email = "admin@example.com",
                    StreetName = "Adminstraat",
                    HouseNumber = "99",
                    PostalCode = "9999AA",
                    PhoneNumber = "0600000000",
                    CitizenServiceNumber = "123466789", // uniek maken!
                    DateOfBirth = new DateTime(1980, 1, 1),
                    Gender = "Man",
                    CreatedAt = DateTime.UtcNow,
                    RoleId = adminRole.Id
                };

                admin.PasswordHash = passwordHasher.HashPassword(admin, "Admin123");

                context.Users.Add(admin);
                context.SaveChanges();
            }
        }
    }
}
