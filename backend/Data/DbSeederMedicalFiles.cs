using backend.Data;
using backend.Models;
using System;
using System.Linq;

namespace backend.Data
{
    public static class DbSeederMedicalFiles
    {
        public static void Seed(AppDbContext context)
        {
            Console.WriteLine("[DbSeederMedicalFiles] Start: Controleren medische testdata...");

            try
            {
                // 1. Check of de tabel al gevuld is
                if (context.MedicalRecordEntries.Any())
                {
                    Console.WriteLine("[DbSeederMedicalFiles] Overslaan: Tabel MedicalRecordEntries bevat al data.");
                    return;
                }

                // 2. Vereiste gebruikers ophalen
                var patient = context.Users.FirstOrDefault(u => u.Email == "gebruiker@example.com");
                var doctor = context.Users.FirstOrDefault(u => u.Email == "testdoctor@example.com");

                if (patient == null || doctor == null)
                {
                    Console.WriteLine("[DbSeederMedicalFiles] Fout: Kan niet seeden. Gebruikers niet gevonden.");
                    Console.WriteLine($"[DbSeederMedicalFiles] Status - Patient: {(patient == null ? "Niet gevonden" : "OK")}");
                    Console.WriteLine($"[DbSeederMedicalFiles] Status - Doctor: {(doctor == null ? "Niet gevonden" : "OK")}");
                    return;
                }

                // 3. Specialismen aanmaken/ophalen
                string[] specNames = { "Cardiologie", "Dermatologie", "Longgeneeskunde" };
                foreach (var name in specNames)
                {
                    if (!context.Specialisms.Any(s => s.Name == name))
                    {
                        context.Specialisms.Add(new Specialism { Name = name });
                        Console.WriteLine($"[DbSeederMedicalFiles] Specialisme toegevoegd: {name}");
                    }
                }
                context.SaveChanges();

                var cardiologie = context.Specialisms.First(s => s.Name == "Cardiologie");
                var dermatologie = context.Specialisms.First(s => s.Name == "Dermatologie");
                var longgeneeskunde = context.Specialisms.First(s => s.Name == "Longgeneeskunde");

                // 4. Behandelingen aanmaken
                if (!context.Treatments.Any(t => t.Code == "Z0001"))
                {
                    context.Treatments.Add(new Treatment { Code = "Z0001", Description = "Consult hart", SpecialismId = cardiologie.Id, DurationInQuarters = 3, Cost = 144 });
                }
                if (!context.Treatments.Any(t => t.Code == "Z0002"))
                {
                    context.Treatments.Add(new Treatment { Code = "Z0002", Description = "Consult huid", SpecialismId = dermatologie.Id, DurationInQuarters = 3, Cost = 51 });
                }
                if (!context.Treatments.Any(t => t.Code == "Z0003"))
                {
                    context.Treatments.Add(new Treatment { Code = "Z0003", Description = "Consult longen", SpecialismId = longgeneeskunde.Id, DurationInQuarters = 2, Cost = 117 });
                }
                context.SaveChanges();
                Console.WriteLine("[DbSeederMedicalFiles] Behandelingen verwerkt.");

                // 5. Afdeling
                var department = context.Departments.FirstOrDefault();
                if (department == null)
                {
                    department = new Department { Name = "Algemeen" };
                    context.Departments.Add(department);
                    context.SaveChanges();
                    Console.WriteLine("[DbSeederMedicalFiles] Afdeling 'Algemeen' aangemaakt.");
                }

                // 6. Verwijzingen (Referrals)
                var consultHart = context.Treatments.First(t => t.Code == "Z0001");
                var consultHuid = context.Treatments.First(t => t.Code == "Z0002");
                var consultLongen = context.Treatments.First(t => t.Code == "Z0003");

                if (!context.Referrals.Any(r => r.Code == "REF001"))
                {
                    context.Referrals.Add(new Referral { Code = "REF001", TreatmentId = consultHart.Id, PatientId = patient.Id, DoctorId = doctor.Id, CreatedAt = DateTime.UtcNow.AddDays(-30) });
                    context.Referrals.Add(new Referral { Code = "REF002", TreatmentId = consultHuid.Id, PatientId = patient.Id, DoctorId = doctor.Id, CreatedAt = DateTime.UtcNow.AddDays(-20) });
                    context.Referrals.Add(new Referral { Code = "REF003", TreatmentId = consultLongen.Id, PatientId = patient.Id, DoctorId = doctor.Id, CreatedAt = DateTime.UtcNow.AddDays(-10) });
                    context.SaveChanges();
                    Console.WriteLine("[DbSeederMedicalFiles] Verwijzingen aangemaakt.");
                }

                var ref1 = context.Referrals.First(r => r.Code == "REF001");
                var ref2 = context.Referrals.First(r => r.Code == "REF002");
                var ref3 = context.Referrals.First(r => r.Code == "REF003");

                // 7. Afspraken, Dossierregels en Bestanden
                // Dossier 1: Cardiologie
                var app1 = new Appointment { PatientId = patient.Id, SpecialistId = doctor.Id, TreatmentId = consultHart.Id, DepartmentId = department.Id, ReferralId = ref1.Id, AppointmentDateTime = DateTime.UtcNow.AddDays(-25) };
                context.Appointments.Add(app1);
                context.SaveChanges();

                var entry1 = new MedicalRecordEntry { 
                    PatientId = patient.Id, 
                    AppointmentId = app1.Id, 
                    CreatedById = doctor.Id, 
                    CreatedAt = app1.AppointmentDateTime, 
                    Title = "ECG Resultaten", 
                    Notes = "ECG uitgevoerd, geen afwijkingen gevonden.", 
                    Category = "Cardiologie" 
                };
                context.MedicalRecordEntries.Add(entry1);
                context.SaveChanges();

                context.MedicalRecordFiles.Add(new MedicalRecordFile {
                    MedicalRecordEntryId = entry1.Id,
                    FileName = "ECG_Rapport.pdf",
                    FileContent = System.Text.Encoding.UTF8.GetBytes("DATA: ECG_NORMAL"),
                    ContentType = "application/pdf",
                    FileSize = 1024,
                    UploadedAt = DateTime.UtcNow,
                    Category = "Onderzoek",
                    Description = "Scan van ECG"
                });

                // Dossier 2: Dermatologie
                var app2 = new Appointment { PatientId = patient.Id, SpecialistId = doctor.Id, TreatmentId = consultHuid.Id, DepartmentId = department.Id, ReferralId = ref2.Id, AppointmentDateTime = DateTime.UtcNow.AddDays(-15) };
                context.Appointments.Add(app2);
                context.SaveChanges();

                var entry2 = new MedicalRecordEntry { 
                    PatientId = patient.Id, 
                    AppointmentId = app2.Id, 
                    CreatedById = doctor.Id, 
                    CreatedAt = app2.AppointmentDateTime, 
                    Title = "Huidonderzoek", 
                    Notes = "Lichte uitslag op de linkerarm.", 
                    Category = "Dermatologie" 
                };
                context.MedicalRecordEntries.Add(entry2);
                context.SaveChanges();

                context.SaveChanges();
                Console.WriteLine($"[DbSeederMedicalFiles] Succes: Medische dossiers en bestanden gesproeid voor {patient.Email}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DbSeederMedicalFiles] Fout tijdens seeden: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"[DbSeederMedicalFiles] Inner Exception: {ex.InnerException.Message}");
                }
            }
        }
    }
}