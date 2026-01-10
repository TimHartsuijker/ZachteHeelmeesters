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

            var patientRole = context.Roles.First(r => r.RoleName == "Patiënt");

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
                    DateOfBirth = DateTime.MinValue,
                    Gender = "Vrouw",
                    CitizenServiceNumber = "123456789",
                    CreatedAt = DateTime.UtcNow,
                    RoleId = patientRole.Id
                };

                user.PasswordHash = passwordHasher.HashPassword(user, "Wachtwoord123");

                context.Users.Add(user);
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
                    DateOfBirth = DateTime.Now,
                    Gender = "Man",
                    CitizenServiceNumber = "012345678",
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
