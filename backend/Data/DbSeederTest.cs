using backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace backend.Data
{
    public static class DbSeederTest
    {
        public static void Seed(AppDbContext context)
        {
            context.Database.EnsureCreated();

            var passwordHasher = new PasswordHasher<User>();

            // Veilige manier om rollen op te halen of aan te maken
            Role GetOrCreateRole(string name)
            {
                var role = context.Roles.FirstOrDefault(r => r.RoleName == name);
                if (role == null)
                {
                    role = new Role { RoleName = name };
                    context.Roles.Add(role);
                    context.SaveChanges();
                }
                return role;
            }

            var patientRole = GetOrCreateRole("Patiënt");
            var specialistRole = GetOrCreateRole("Specialist");
            var gpRole = GetOrCreateRole("Huisarts");
            var adminRole = GetOrCreateRole("Admin");

            if (patientRole == null || specialistRole == null || gpRole == null || adminRole == null)
            {
                Console.WriteLine("[DbSeederTest] FOUT: Een of meer rollen ontbreken. Seeder afgebroken.");
                return;
            }

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
                    RoleId = specialistRole.Id
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

            // Tweede testpatiënt voor US5.5 (mag leeg dossier hebben)
            var patientUserB = context.Users.FirstOrDefault(u => u.Email == "patient2@example.com");
            if (patientUserB == null)
            {
                patientUserB = new User
                {
                    FirstName = "Test",
                    LastName = "PatiëntB",
                    Email = "patient2@example.com",
                    StreetName = "Teststraat",
                    HouseNumber = "2",
                    PostalCode = "1234AB",
                    CitizenServiceNumber = "987654321",
                    DateOfBirth = new DateTime(2001, 2, 2),
                    Gender = "Vrouw",
                    PhoneNumber = "0612345679",
                    CreatedAt = DateTime.UtcNow,
                    RoleId = patientRole.Id,
                    DoctorId = doctorUser?.Id
                };

                patientUserB.PasswordHash = passwordHasher.HashPassword(patientUserB, "Wachtwoord123");
                context.Users.Add(patientUserB);
                context.SaveChanges();
                Console.WriteLine($"[DbSeederTest] Created second Patient user with ID: {patientUserB.Id}");
            }
            else
            {
                Console.WriteLine($"[DbSeederTest] Second Patient user already exists with ID: {patientUserB.Id}");
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
                    CitizenServiceNumber = "987654322",
                    CreatedAt = DateTime.UtcNow,
                    RoleId = gpRole.Id
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
                    CitizenServiceNumber = "987654323",
                    CreatedAt = DateTime.UtcNow,
                    RoleId = gpRole.Id
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

            // ------------------------
            // Seed Specialisms first (user provided list)
            // ------------------------
            var specialismNames = new[]
            {
                "Cardiologie",
                "Dermatologie",
                "Longgeneeskunde",
                "Oogheelkunde",
                "KNO",
                "Orthopedie",
                "Chirurgie",
                "Radiologie",
                "Laboratoriumgeneeskunde",
                "Revalidatiegeneeskunde",
                "Psychiatrie",
                "Algemeen"
            };

            foreach (var name in specialismNames)
            {
                if (!context.Specialisms.Any(s => s.Name == name))
                {
                    context.Specialisms.Add(new Specialism { Name = name });
                    Console.WriteLine($"[DbSeederTest] Added specialism: {name}");
                }
                else
                {
                    Console.WriteLine($"[DbSeederTest] Specialism already exists: {name}");
                }
            }
            context.SaveChanges();

            // Helper to get specialism by name (fresh from DB so Id is populated)
            Specialism GetSpecialism(string name)
            {
                return context.Specialisms.First(s => s.Name == name);
            }

            var Cardiologie = GetSpecialism("Cardiologie");
            var Dermatologie = GetSpecialism("Dermatologie");
            var Longgeneeskunde = GetSpecialism("Longgeneeskunde");
            var Oogheelkunde = GetSpecialism("Oogheelkunde");
            var KNO = GetSpecialism("KNO");
            var Orthopedie = GetSpecialism("Orthopedie");
            var Chirurgie = GetSpecialism("Chirurgie");
            var Radiologie = GetSpecialism("Radiologie");
            var Laboratoriumgeneeskunde = GetSpecialism("Laboratoriumgeneeskunde");
            var Revalidatiegeneeskunde = GetSpecialism("Revalidatiegeneeskunde");
            var Psychiatrie = GetSpecialism("Psychiatrie");
            var Algemeen = GetSpecialism("Algemeen");

            // ------------------------
            // Seed Treatments (after specialisms)
            // ------------------------
            var treatmentsToEnsure = new[]
            {
new Treatment { Code = "Z0001", Description = "Consult hart", SpecialismId = Cardiologie.Id, DurationInQuarters = 3, Cost = 144 },
new Treatment { Code = "Z0002", Description = "Consult huid", SpecialismId = Dermatologie.Id, DurationInQuarters = 3, Cost = 51 },
new Treatment { Code = "Z0003", Description = "Consult longen", SpecialismId = Longgeneeskunde.Id, DurationInQuarters = 2, Cost = 117 },
new Treatment { Code = "Z0004", Description = "Consult ogen", SpecialismId = Oogheelkunde.Id, DurationInQuarters = 3, Cost = 114 },
new Treatment { Code = "Z0005", Description = "Consult oren", SpecialismId = KNO.Id, DurationInQuarters = 3, Cost = 65 },
new Treatment { Code = "Z0006", Description = "Operatie knie", SpecialismId = Orthopedie.Id, DurationInQuarters = 2, Cost = 1084 },
new Treatment { Code = "Z0007", Description = "Operatie heup", SpecialismId = Orthopedie.Id, DurationInQuarters = 3, Cost = 2909 },
new Treatment { Code = "Z0008", Description = "Operatie hart", SpecialismId = Cardiologie.Id, DurationInQuarters = 4, Cost = 2572 },
new Treatment { Code = "Z0009", Description = "Operatie huid", SpecialismId = Chirurgie.Id, DurationInQuarters = 4, Cost = 2739 },
new Treatment { Code = "Z0010", Description = "Operatie rug", SpecialismId = Orthopedie.Id, DurationInQuarters = 3, Cost = 2903 },
new Treatment { Code = "Z0011", Description = "Controle hart", SpecialismId = Cardiologie.Id, DurationInQuarters = 3, Cost = 187 },
new Treatment { Code = "Z0012", Description = "Controle huid", SpecialismId = Dermatologie.Id, DurationInQuarters = 4, Cost = 172 },
new Treatment { Code = "Z0013", Description = "Controle longen", SpecialismId = Longgeneeskunde.Id, DurationInQuarters = 3, Cost = 72 },
new Treatment { Code = "Z0014", Description = "Controle ogen", SpecialismId = Oogheelkunde.Id, DurationInQuarters = 3, Cost = 96 },
new Treatment { Code = "Z0015", Description = "Controle oren", SpecialismId = KNO.Id, DurationInQuarters = 4, Cost = 150 },
new Treatment { Code = "Z0016", Description = "MRI-scan hoofd", SpecialismId = Radiologie.Id, DurationInQuarters = 2, Cost = 609 },
new Treatment { Code = "Z0017", Description = "MRI-scan knie", SpecialismId = Radiologie.Id, DurationInQuarters = 3, Cost = 223 },
new Treatment { Code = "Z0018", Description = "MRI-scan rug", SpecialismId = Radiologie.Id, DurationInQuarters = 3, Cost = 314 },
new Treatment { Code = "Z0019", Description = "MRI-scan hart", SpecialismId = Radiologie.Id, DurationInQuarters = 2, Cost = 728 },
new Treatment { Code = "Z0020", Description = "MRI-scan buik", SpecialismId = Radiologie.Id, DurationInQuarters = 2, Cost = 395 },
new Treatment { Code = "Z0021", Description = "CT-scan hoofd", SpecialismId = Radiologie.Id, DurationInQuarters = 3, Cost = 682 },
new Treatment { Code = "Z0022", Description = "CT-scan knie", SpecialismId = Radiologie.Id, DurationInQuarters = 3, Cost = 616 },
new Treatment { Code = "Z0023", Description = "CT-scan rug", SpecialismId = Radiologie.Id, DurationInQuarters = 3, Cost = 378 },
new Treatment { Code = "Z0024", Description = "CT-scan hart", SpecialismId = Radiologie.Id, DurationInQuarters = 3, Cost = 793 },
new Treatment { Code = "Z0025", Description = "CT-scan buik", SpecialismId = Radiologie.Id, DurationInQuarters = 4, Cost = 448 },
new Treatment { Code = "Z0026", Description = "Bloedonderzoek algemeen", SpecialismId = Laboratoriumgeneeskunde.Id, DurationInQuarters = 1, Cost = 55 },
new Treatment { Code = "Z0027", Description = "Bloedonderzoek hormonen", SpecialismId = Laboratoriumgeneeskunde.Id, DurationInQuarters = 2, Cost = 92 },
new Treatment { Code = "Z0028", Description = "Bloedonderzoek infecties", SpecialismId = Laboratoriumgeneeskunde.Id, DurationInQuarters = 1, Cost = 71 },
new Treatment { Code = "Z0029", Description = "Bloedonderzoek suiker", SpecialismId = Laboratoriumgeneeskunde.Id, DurationInQuarters = 4, Cost = 98 },
new Treatment { Code = "Z0030", Description = "Bloedonderzoek cholesterol", SpecialismId = Laboratoriumgeneeskunde.Id, DurationInQuarters = 3, Cost = 38 },
new Treatment { Code = "Z0031", Description = "Fysiotherapie knie", SpecialismId = Revalidatiegeneeskunde.Id, DurationInQuarters = 1, Cost = 43 },
new Treatment { Code = "Z0032", Description = "Fysiotherapie rug", SpecialismId = Revalidatiegeneeskunde.Id, DurationInQuarters = 3, Cost = 70 },
new Treatment { Code = "Z0033", Description = "Fysiotherapie schouder", SpecialismId = Revalidatiegeneeskunde.Id, DurationInQuarters = 2, Cost = 101 },
new Treatment { Code = "Z0034", Description = "Fysiotherapie heup", SpecialismId = Revalidatiegeneeskunde.Id, DurationInQuarters = 4, Cost = 103 },
new Treatment { Code = "Z0035", Description = "Fysiotherapie nek", SpecialismId = Revalidatiegeneeskunde.Id, DurationInQuarters = 1, Cost = 93 },
new Treatment { Code = "Z0036", Description = "Psychologisch gesprek depressie", SpecialismId = Psychiatrie.Id, DurationInQuarters = 4, Cost = 123 },
new Treatment { Code = "Z0037", Description = "Psychologisch gesprek angst", SpecialismId = Psychiatrie.Id, DurationInQuarters = 3, Cost = 132 },
new Treatment { Code = "Z0038", Description = "Psychologisch gesprek trauma", SpecialismId = Psychiatrie.Id, DurationInQuarters = 1, Cost = 72 },
new Treatment { Code = "Z0039", Description = "Psychologisch gesprek burn-out", SpecialismId = Psychiatrie.Id, DurationInQuarters = 3, Cost = 172 },
new Treatment { Code = "Z0040", Description = "Psychologisch gesprek stress", SpecialismId = Psychiatrie.Id, DurationInQuarters = 4, Cost = 135 },
new Treatment { Code = "Z0041", Description = "Vaccinatie griep", SpecialismId = Algemeen.Id, DurationInQuarters = 3, Cost = 32 },
new Treatment { Code = "Z0042", Description = "Vaccinatie hepatitis", SpecialismId = Algemeen.Id, DurationInQuarters = 2, Cost = 57 },
new Treatment { Code = "Z0043", Description = "Vaccinatie tetanus", SpecialismId = Algemeen.Id, DurationInQuarters = 1, Cost = 61 },
new Treatment { Code = "Z0044", Description = "Vaccinatie covid", SpecialismId = Algemeen.Id, DurationInQuarters = 2, Cost = 45 },
new Treatment { Code = "Z0045", Description = "Vaccinatie HPV", SpecialismId = Algemeen.Id, DurationInQuarters = 4, Cost = 30 },
new Treatment { Code = "Z0046", Description = "Wondverzorging arm", SpecialismId = Chirurgie.Id, DurationInQuarters = 2, Cost = 55 },
new Treatment { Code = "Z0047", Description = "Wondverzorging been", SpecialismId = Chirurgie.Id, DurationInQuarters = 2, Cost = 111 },
new Treatment { Code = "Z0048", Description = "Wondverzorging hoofd", SpecialismId = Chirurgie.Id, DurationInQuarters = 2, Cost = 33 },
new Treatment { Code = "Z0049", Description = "Wondverzorging buik", SpecialismId = Chirurgie.Id, DurationInQuarters = 4, Cost = 56 },
new Treatment { Code = "Z0050", Description = "Wondverzorging rug", SpecialismId = Chirurgie.Id, DurationInQuarters = 4, Cost = 103 },
new Treatment { Code = "Z0051", Description = "Fysiotherapie rug", SpecialismId = Revalidatiegeneeskunde.Id, DurationInQuarters = 1, Cost = 100 },
new Treatment { Code = "Z0052", Description = "Fysiotherapie schouder", SpecialismId = Revalidatiegeneeskunde.Id, DurationInQuarters = 4, Cost = 79 },
new Treatment { Code = "Z0053", Description = "Wondverzorging rug", SpecialismId = Chirurgie.Id, DurationInQuarters = 4, Cost = 130 },
new Treatment { Code = "Z0054", Description = "Vaccinatie HPV", SpecialismId = Algemeen.Id, DurationInQuarters = 4, Cost = 76 },
new Treatment { Code = "Z0055", Description = "Vaccinatie hepatitis", SpecialismId = Algemeen.Id, DurationInQuarters = 1, Cost = 59 },
new Treatment { Code = "Z0056", Description = "Psychologisch gesprek stress", SpecialismId = Psychiatrie.Id, DurationInQuarters = 1, Cost = 105 },
new Treatment { Code = "Z0057", Description = "Consult ogen", SpecialismId = Oogheelkunde.Id, DurationInQuarters = 1, Cost = 126 },
new Treatment { Code = "Z0058", Description = "Operatie rug", SpecialismId = Orthopedie.Id, DurationInQuarters = 1, Cost = 2829 },
new Treatment { Code = "Z0059", Description = "MRI-scan buik", SpecialismId = Radiologie.Id, DurationInQuarters = 2, Cost = 222 },
new Treatment { Code = "Z0060", Description = "Controle huid", SpecialismId = Dermatologie.Id, DurationInQuarters = 3, Cost = 69 },
new Treatment { Code = "Z0061", Description = "MRI-scan hoofd", SpecialismId = Radiologie.Id, DurationInQuarters = 1, Cost = 616 },
new Treatment { Code = "Z0062", Description = "Controle longen", SpecialismId = Longgeneeskunde.Id, DurationInQuarters = 4, Cost = 181 },
new Treatment { Code = "Z0063", Description = "Bloedonderzoek algemeen", SpecialismId = Laboratoriumgeneeskunde.Id, DurationInQuarters = 4, Cost = 68 },
new Treatment { Code = "Z0064", Description = "Wondverzorging arm", SpecialismId = Chirurgie.Id, DurationInQuarters = 2, Cost = 65 },
new Treatment { Code = "Z0065", Description = "Operatie hart", SpecialismId = Cardiologie.Id, DurationInQuarters = 4, Cost = 1272 },
new Treatment { Code = "Z0066", Description = "Consult huid", SpecialismId = Dermatologie.Id, DurationInQuarters = 2, Cost = 72 },
new Treatment { Code = "Z0067", Description = "Psychologisch gesprek angst", SpecialismId = Psychiatrie.Id, DurationInQuarters = 4, Cost = 64 },
new Treatment { Code = "Z0068", Description = "CT-scan knie", SpecialismId = Radiologie.Id, DurationInQuarters = 1, Cost = 719 },
new Treatment { Code = "Z0069", Description = "Wondverzorging been", SpecialismId = Chirurgie.Id, DurationInQuarters = 1, Cost = 96 },
new Treatment { Code = "Z0070", Description = "Operatie huid", SpecialismId = Chirurgie.Id, DurationInQuarters = 4, Cost = 2818 },
new Treatment { Code = "Z0071", Description = "Consult hart", SpecialismId = Cardiologie.Id, DurationInQuarters = 2, Cost = 73 },
new Treatment { Code = "Z0072", Description = "Consult oren", SpecialismId = KNO.Id, DurationInQuarters = 4, Cost = 110 },
new Treatment { Code = "Z0073", Description = "Bloedonderzoek cholesterol", SpecialismId = Laboratoriumgeneeskunde.Id, DurationInQuarters = 3, Cost = 42 },
new Treatment { Code = "Z0074", Description = "Controle hart", SpecialismId = Cardiologie.Id, DurationInQuarters = 4, Cost = 192 },
new Treatment { Code = "Z0075", Description = "Vaccinatie griep", SpecialismId = Algemeen.Id, DurationInQuarters = 4, Cost = 80 },
new Treatment { Code = "Z0076", Description = "CT-scan rug", SpecialismId = Radiologie.Id, DurationInQuarters = 4, Cost = 482 },
new Treatment { Code = "Z0077", Description = "Bloedonderzoek hormonen", SpecialismId = Laboratoriumgeneeskunde.Id, DurationInQuarters = 2, Cost = 52 },
new Treatment { Code = "Z0078", Description = "Vaccinatie covid", SpecialismId = Algemeen.Id, DurationInQuarters = 2, Cost = 47 },
new Treatment { Code = "Z0079", Description = "Psychologisch gesprek trauma", SpecialismId = Psychiatrie.Id, DurationInQuarters = 1, Cost = 104 },
new Treatment { Code = "Z0080", Description = "MRI-scan hart", SpecialismId = Radiologie.Id, DurationInQuarters = 2, Cost = 781 },
new Treatment { Code = "Z0081", Description = "Fysiotherapie heup", SpecialismId = Revalidatiegeneeskunde.Id, DurationInQuarters = 2, Cost = 118 },
new Treatment { Code = "Z0082", Description = "MRI-scan knie", SpecialismId = Radiologie.Id, DurationInQuarters = 1, Cost = 441 },
new Treatment { Code = "Z0083", Description = "Psychologisch gesprek burn-out", SpecialismId = Psychiatrie.Id, DurationInQuarters = 4, Cost = 184 },
new Treatment { Code = "Z0084", Description = "CT-scan buik", SpecialismId = Radiologie.Id, DurationInQuarters = 2, Cost = 453 },
new Treatment { Code = "Z0085", Description = "Consult longen", SpecialismId = Longgeneeskunde.Id, DurationInQuarters = 2, Cost = 76 },
new Treatment { Code = "Z0086", Description = "CT-scan hoofd", SpecialismId = Radiologie.Id, DurationInQuarters = 4, Cost = 572 },
new Treatment { Code = "Z0087", Description = "Fysiotherapie knie", SpecialismId = Revalidatiegeneeskunde.Id, DurationInQuarters = 1, Cost = 69 },
new Treatment { Code = "Z0088", Description = "Wondverzorging buik", SpecialismId = Chirurgie.Id, DurationInQuarters = 4, Cost = 66 },
new Treatment { Code = "Z0089", Description = "MRI-scan rug", SpecialismId = Radiologie.Id, DurationInQuarters = 4, Cost = 598 },
new Treatment { Code = "Z0090", Description = "Psychologisch gesprek depressie", SpecialismId = Psychiatrie.Id, DurationInQuarters = 2, Cost = 159 },
new Treatment { Code = "Z0091", Description = "Operatie knie", SpecialismId = Orthopedie.Id, DurationInQuarters = 3, Cost = 1128 },
new Treatment { Code = "Z0092", Description = "Fysiotherapie nek", SpecialismId = Revalidatiegeneeskunde.Id, DurationInQuarters = 4, Cost = 102 },
new Treatment { Code = "Z0093", Description = "CT-scan hart", SpecialismId = Radiologie.Id, DurationInQuarters = 1, Cost = 501 },
new Treatment { Code = "Z0094", Description = "Controle ogen", SpecialismId = Oogheelkunde.Id, DurationInQuarters = 2, Cost = 167 },
new Treatment { Code = "Z0095", Description = "Bloedonderzoek suiker", SpecialismId = Laboratoriumgeneeskunde.Id, DurationInQuarters = 1, Cost = 48 },
new Treatment { Code = "Z0096", Description = "Controle oren", SpecialismId = KNO.Id, DurationInQuarters = 1, Cost = 138 },
new Treatment { Code = "Z0097", Description = "Wondverzorging hoofd", SpecialismId = Chirurgie.Id, DurationInQuarters = 1, Cost = 46 },
new Treatment { Code = "Z0098", Description = "Vaccinatie tetanus", SpecialismId = Algemeen.Id, DurationInQuarters = 1, Cost = 46 },
new Treatment { Code = "Z0099", Description = "Operatie heup", SpecialismId = Orthopedie.Id, DurationInQuarters = 1, Cost = 738 },
new Treatment { Code = "Z0100", Description = "Bloedonderzoek infecties", SpecialismId = Laboratoriumgeneeskunde.Id, DurationInQuarters = 4, Cost = 37 }
            };

            foreach (var t in treatmentsToEnsure)
            {
                if (!context.Treatments.Any(x => x.Code == t.Code))
                {
                    context.Treatments.Add(t);
                    Console.WriteLine($"[DbSeederTest] Adding treatment {t.Code} - {t.Description}");
                }
                else
                {
                    Console.WriteLine($"[DbSeederTest] Treatment with code {t.Code} already exists, skipping.");
                }
            }
            context.SaveChanges();

            // Summary
            var totalTreatments = context.Treatments.Count();
            Console.WriteLine($"[DbSeederTest] Total treatments in database: {totalTreatments}");
        }
    }
}
