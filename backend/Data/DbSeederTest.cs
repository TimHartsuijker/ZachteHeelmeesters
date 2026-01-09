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

            // 🔹 Role seeden - ensure they exist first
            var patientRole = context.Roles.FirstOrDefault(r => r.RoleName == "Patient");
            if (patientRole == null)
            {
                patientRole = new Role { RoleName = "Patient" };
                context.Roles.Add(patientRole);
                context.SaveChanges();
                Console.WriteLine("[DbSeederTest] Created Patient role");
            }

            var doctorRole = context.Roles.FirstOrDefault(r => r.RoleName == "Doctor");
            if (doctorRole == null)
            {
                doctorRole = new Role { RoleName = "Doctor" };
                context.Roles.Add(doctorRole);
                context.SaveChanges();
                Console.WriteLine("[DbSeederTest] Created Doctor role");
            }

            // 🔹 Patient user seeden
            var patientUser = context.Users.FirstOrDefault(u => u.Email == "gebruiker@example.com");
            if (patientUser == null)
            {
                patientUser = new User
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

                patientUser.PasswordHash = passwordHasher.HashPassword(patientUser, "Wachtwoord123");
                context.Users.Add(patientUser);
                context.SaveChanges();
                Console.WriteLine($"[DbSeederTest] Created Patient user with ID: {patientUser.Id}");
            }
            else
            {
                Console.WriteLine($"[DbSeederTest] Patient user already exists with ID: {patientUser.Id}");
            }

            // 🔹 Doctor user seeden
            var doctorUser = context.Users.FirstOrDefault(u => u.Email == "testdoctor@example.com");
            if (doctorUser == null)
            {
                doctorUser = new User
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

                doctorUser.PasswordHash = passwordHasher.HashPassword(doctorUser, "password");
                context.Users.Add(doctorUser);
                context.SaveChanges();
                Console.WriteLine($"[DbSeederTest] Created Doctor user with ID: {doctorUser.Id}");
            }
            else
            {
                Console.WriteLine($"[DbSeederTest] Doctor user already exists with ID: {doctorUser.Id}");
            }

            // Verify both users exist
            var allUsers = context.Users.ToList();
            Console.WriteLine($"[DbSeederTest] Total users in database: {allUsers.Count}");
            foreach (var user in allUsers)
            {
                Console.WriteLine($"[DbSeederTest]   - User ID {user.Id}: {user.Email}");
            }
        }
    }
}
