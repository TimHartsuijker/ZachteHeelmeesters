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
                    PhoneNumber = "0612345678",
                    DateOfBirth = DateTime.MinValue,
                    Gender = "Vrouw",
                    CitizenServiceNumber = "123456789",
                    CreatedAt = DateTime.UtcNow,
                    RoleId = patientRole.Id,
                    //CitizenServiceNumber = "233456789",
                    //Gender = "Ander"

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

            // Doctor user seeden
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
                    DateOfBirth = DateTime.MinValue,
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
                    RoleId = generalPracticioner.Id
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
                    RoleId = generalPracticioner.Id
                };

                doctor.PasswordHash = passwordHasher.HashPassword(doctor, "Huisarts123");

                context.Users.Add(doctor);
                context.SaveChanges();

                // Verify all users exist
                var allUsers = context.Users.ToList();
                Console.WriteLine($"[DbSeederTest] Total users in database: {allUsers.Count}");
                foreach (var user in allUsers)
                {
                    Console.WriteLine($"[DbSeederTest]   - User ID {user.Id}: {user.Email}");
                }
            }


            // =====================
            // Additional SQL-based seed (converted to EF Core)
            // =====================

            // ---------------------
            // Seed Roles
            // ---------------------
            if (!context.Roles.Any(r => r.RoleName == "GeneralPractitioner"))
            {
                context.Roles.Add(new Role { RoleName = "GeneralPractitioner" });
            }

            if (!context.Roles.Any(r => r.RoleName == "Doctor"))
            {
                context.Roles.Add(new Role { RoleName = "Doctor" });
            }

            context.SaveChanges();

            var gpRole = context.Roles.First(r => r.RoleName == "GeneralPractitioner");

            // ---------------------
            // Seed General Practitioner user
            // ---------------------
            if (!context.Users.Any(u => u.Email == "gp.jansen@example.nl"))
            {
                var gpUser = new User
                {
                    FirstName = "Jan",
                    LastName = "Jansen",
                    Email = "gp.jansen@example.nl",
                    StreetName = "Village Street",
                    HouseNumber = "12A",
                    PostalCode = "1234AB",
                    PhoneNumber = "0612345678",
                    CreatedAt = DateTime.UtcNow,
                    RoleId = gpRole.Id,
                    PracticeName = "Jansen General Practice",
                    //CitizenServiceNumber = "123456789",
                    //Gender = "Man"
                };

                gpUser.PasswordHash = passwordHasher.HashPassword(gpUser, "Wachtwoord123");

                context.Users.Add(gpUser);
                context.SaveChanges();
            }

            // ---------------------
            // Seed Patient user (Emma de Vries)
            // ---------------------
            if (!context.Users.Any(u => u.Email == "devries@example.com"))
            {
                var patientUser = new User
                {
                    FirstName = "Emma",
                    LastName = "de Vries",
                    Email = "devries@example.com",
                    StreetName = "Lime Tree Avenue",
                    HouseNumber = "45",
                    PostalCode = "5678CD",
                    PhoneNumber = "0687654321",
                    CreatedAt = DateTime.UtcNow,
                    RoleId = patientRole.Id,
                    CitizenServiceNumber = "223456789",
                    Gender = "Vrouw",
                    DateOfBirth = new DateTime(1990, 1, 12)
                };

                patientUser.PasswordHash = passwordHasher.HashPassword(patientUser, "Wachtwoord123");

                context.Users.Add(patientUser);
                context.SaveChanges();
            }

            // ---------------------
            // Link General Practitioner to Patient
            // (replacement for huisarts_patient table)
            // ---------------------
            var gp = context.Users.First(u => u.Email == "gp.jansen@example.nl");
            var patient = context.Users.First(u => u.Email == "devries@example.com");

            if (patient.DoctorId == null)
            {
                patient.DoctorId = gp.Id;
                context.SaveChanges();
            }
        }
    }
}
