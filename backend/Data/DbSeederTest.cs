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

            // 🔹 Roles seeden
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

            // 🔹 Doctor seeden
            User? doctor = context.Users.FirstOrDefault(u => u.Email == "dokter@example.com");
            if (doctor == null)
            {
                doctor = new User
                {
                    FirstName = "Test",
                    LastName = "Dokter",
                    Email = "dokter@example.com",
                    StreetName = "Dokterstraat",
                    HouseNumber = "10",
                    PostalCode = "5678CD",
                    CitizenServiceNumber = "987654321",
                    DateOfBirth = new DateTime(1985, 5, 5),
                    Gender = "Man",
                    PhoneNumber = "0611111111",
                    PracticeName = "Test Praktijk",
                    CreatedAt = DateTime.UtcNow,
                    RoleId = doctorRole.Id
                };

                doctor.PasswordHash = passwordHasher.HashPassword(doctor, "Wachtwoord123");

                context.Users.Add(doctor);
                context.SaveChanges();
            }

            // 🔹 Patient seeden
            User? patient = context.Users.FirstOrDefault(u => u.Email == "gebruiker@example.com");
            if (patient == null)
            {
                patient = new User
                {
                    FirstName = "Test",
                    LastName = "Gebruiker",
                    Email = "gebruiker@example.com",
                    StreetName = "Teststraat",
                    HouseNumber = "1",
                    PostalCode = "1234AB",
                    CitizenServiceNumber = "123456789",
                    DateOfBirth = new DateTime(2000, 1, 1),
                    Gender = "Man",
                    PhoneNumber = "0612345678",
                    CreatedAt = DateTime.UtcNow,
                    RoleId = patientRole.Id,
                    DoctorId = doctor.Id
                };

                patient.PasswordHash = passwordHasher.HashPassword(patient, "Wachtwoord123");

                context.Users.Add(patient);
                context.SaveChanges();
            }
            else if (patient.DoctorId == null)
            {
                patient.DoctorId = doctor.Id;
                context.SaveChanges();
            }

            // Ensure doctor has patient in navigation collection
            if (doctor.Patients.All(p => p.Id != patient.Id))
            {
                doctor.Patients.Add(patient);
                context.SaveChanges();
            }

            // 🔹 Empty Dossier Patient seeden (for testing empty state)
            User? emptyPatient = context.Users.FirstOrDefault(u => u.Email == "leegdossier@example.com");
            if (emptyPatient == null)
            {
                emptyPatient = new User
                {
                    FirstName = "Leeg",
                    LastName = "Dossier",
                    Email = "leegdossier@example.com",
                    StreetName = "Lege Straat",
                    HouseNumber = "99",
                    PostalCode = "0000AA",
                    CitizenServiceNumber = "999999999",
                    DateOfBirth = new DateTime(1995, 12, 31),
                    Gender = "Vrouw",
                    PhoneNumber = "0699999999",
                    CreatedAt = DateTime.UtcNow,
                    RoleId = patientRole.Id,
                    DoctorId = doctor.Id
                };

                emptyPatient.PasswordHash = passwordHasher.HashPassword(emptyPatient, "Wachtwoord123");

                context.Users.Add(emptyPatient);
                context.SaveChanges();
            }
        }
    }
}
