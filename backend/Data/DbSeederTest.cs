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
            var doctorRole = context.Roles.First(r => r.RoleName == "Specialist");
            var generalPracticioner = context.Roles.First(r => r.RoleName == "Huisarts");
            var adminRole = context.Roles.First(r => r.RoleName == "Admin");


            // Doctor user seeden (eerst zodat patient koppeling kan krijgen)
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
                    DateOfBirth = new DateTime(1990, 1, 1),
                    CitizenServiceNumber = "012948356",
                    Gender = "Man",
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

            // Patient user seeden
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
                    CitizenServiceNumber = "123456789",
                    DateOfBirth = new DateTime(2000, 1, 1),
                    Gender = "Vrouw",
                    PhoneNumber = "0612345678",
                    CreatedAt = DateTime.UtcNow,
                    RoleId = patientRole.Id,
                    DoctorId = doctorUser?.Id
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

            // Admin user seeden
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

            // Verify all users exist
            var allUsers = context.Users.ToList();
            Console.WriteLine($"[DbSeederTest] Total users in database: {allUsers.Count}");
            foreach (var user in allUsers)
            {
                Console.WriteLine($"[DbSeederTest]   - User ID {user.Id}: {user.Email}");
            }
        }
    }
}
