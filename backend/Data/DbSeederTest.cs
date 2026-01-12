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
                    CreatedAt = DateTime.UtcNow,
                    RoleId = patientRole.Id,
                    //CitizenServiceNumber = "233456789",
                    //Gender = "Ander"

                };

                user.PasswordHash = passwordHasher.HashPassword(user, "Wachtwoord123");

                context.Users.Add(user);
                context.SaveChanges();
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
