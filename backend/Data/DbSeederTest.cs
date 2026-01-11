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
            var adminRole = context.Roles.First(r => r.RoleName == "Admin");
            var doctorRole = context.Roles.First(r => r.RoleName == "Huisarts");

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

            // 🔹 Doctor users seeden
            if (!context.Users.Any(u => u.Email == "doctor1@example.com"))
            {
                var doctor = new User
                {
                    FirstName = "Huisarts",
                    LastName = "Een",
                    Email = "doctor1@example.com",
                    StreetName = "Doctorstraat",
                    HouseNumber = "1",
                    PostalCode = "1234AB",
                    PhoneNumber = "0612345678",
                    DateOfBirth = DateTime.Now,
                    Gender = "Man",
                    CitizenServiceNumber = "987654321",
                    CreatedAt = DateTime.UtcNow,
                    RoleId = doctorRole.Id
                };

                doctor.PasswordHash = passwordHasher.HashPassword(doctor, "Huisarts123");

                context.Users.Add(doctor);
                context.SaveChanges();
            }

            // 🔹 Doctor users seeden
            if (!context.Users.Any(u => u.Email == "doctor2@example.com"))
            {
                var doctor = new User
                {
                    FirstName = "Huisarts",
                    LastName = "Twee",
                    Email = "doctor2@example.com",
                    StreetName = "Doctorstraat",
                    HouseNumber = "1",
                    PostalCode = "1234AB",
                    PhoneNumber = "0612345678",
                    DateOfBirth = DateTime.Now,
                    Gender = "Man",
                    CitizenServiceNumber = "987654322",
                    CreatedAt = DateTime.UtcNow,
                    RoleId = doctorRole.Id
                };

                doctor.PasswordHash = passwordHasher.HashPassword(doctor, "Huisarts123");

                context.Users.Add(doctor);
                context.SaveChanges();
            }
        }
    }
}
