using backend.Data;
using backend.Models;

namespace backend.Data
{
    public static class DbSeederMedicalFiles
    {
        public static void Seed(AppDbContext context)
        {
            try
            {
                // Check if we already have some test entries
                if (context.MedicalRecordEntries.Any())
                {
                    return; // Database already seeded
                }

                // Get test patient and doctor
                var patient = context.Users.FirstOrDefault(u => u.Email == "gebruiker@example.com");
                var doctor = context.Users.FirstOrDefault(u => u.Email == "testdoctor@example.com");

                if (patient == null || doctor == null)
                {
                    return; // Can't seed without patient and doctor
                }

                // Seed specialisms if not exist
                var cardiologie = context.Specialisms.FirstOrDefault(s => s.Name == "Cardiologie");
                if (cardiologie == null)
                {
                    cardiologie = new Specialism { Name = "Cardiologie" };
                    context.Specialisms.Add(cardiologie);
                }

                var dermatologie = context.Specialisms.FirstOrDefault(s => s.Name == "Dermatologie");
                if (dermatologie == null)
                {
                    dermatologie = new Specialism { Name = "Dermatologie" };
                    context.Specialisms.Add(dermatologie);
                }

                var longgeneeskunde = context.Specialisms.FirstOrDefault(s => s.Name == "Longgeneeskunde");
                if (longgeneeskunde == null)
                {
                    longgeneeskunde = new Specialism { Name = "Longgeneeskunde" };
                    context.Specialisms.Add(longgeneeskunde);
                }

                context.SaveChanges();

                // Seed treatments if not exist
                var consultHart = context.Treatments.FirstOrDefault(t => t.Code == "Z0001");
                if (consultHart == null)
                {
                    consultHart = new Treatment
                    {
                        Code = "Z0001",
                        Description = "Consult hart",
                        SpecialismId = cardiologie.Id,
                        DurationInQuarters = 3,
                        Cost = 144
                    };
                    context.Treatments.Add(consultHart);
                }

                var consultHuid = context.Treatments.FirstOrDefault(t => t.Code == "Z0002");
                if (consultHuid == null)
                {
                    consultHuid = new Treatment
                    {
                        Code = "Z0002",
                        Description = "Consult huid",
                        SpecialismId = dermatologie.Id,
                        DurationInQuarters = 3,
                        Cost = 51
                    };
                    context.Treatments.Add(consultHuid);
                }

                var consultLongen = context.Treatments.FirstOrDefault(t => t.Code == "Z0003");
                if (consultLongen == null)
                {
                    consultLongen = new Treatment
                    {
                        Code = "Z0003",
                        Description = "Consult longen",
                        SpecialismId = longgeneeskunde.Id,
                        DurationInQuarters = 2,
                        Cost = 117
                    };
                    context.Treatments.Add(consultLongen);
                }

                context.SaveChanges();

                // Seed department if not exist
                var department = context.Departments.FirstOrDefault();
                if (department == null)
                {
                    department = new Department { Name = "Algemeen" };
                    context.Departments.Add(department);
                    context.SaveChanges();
                }

                // Create referrals for appointments (required by Appointment model)
                var referral1 = context.Referrals.FirstOrDefault(r => r.Code == "REF001");
                if (referral1 == null)
                {
                    referral1 = new Referral
                    {
                        Code = "REF001",
                        TreatmentId = consultHart.Id,
                        PatientId = patient.Id,
                        DoctorId = doctor.Id,
                        CreatedAt = DateTime.UtcNow.AddDays(-30)
                    };
                    context.Referrals.Add(referral1);
                }

                var referral2 = context.Referrals.FirstOrDefault(r => r.Code == "REF002");
                if (referral2 == null)
                {
                    referral2 = new Referral
                    {
                        Code = "REF002",
                        TreatmentId = consultHuid.Id,
                        PatientId = patient.Id,
                        DoctorId = doctor.Id,
                        CreatedAt = DateTime.UtcNow.AddDays(-20)
                    };
                    context.Referrals.Add(referral2);
                }

                var referral3 = context.Referrals.FirstOrDefault(r => r.Code == "REF003");
                if (referral3 == null)
                {
                    referral3 = new Referral
                    {
                        Code = "REF003",
                        TreatmentId = consultLongen.Id,
                        PatientId = patient.Id,
                        DoctorId = doctor.Id,
                        CreatedAt = DateTime.UtcNow.AddDays(-10)
                    };
                    context.Referrals.Add(referral3);
                }

                context.SaveChanges();

                // Create appointments
                var appointment1 = new Appointment
                {
                    PatientId = patient.Id,
                    SpecialistId = doctor.Id,
                    TreatmentId = consultHart.Id,
                    DepartmentId = department.Id,
                    ReferralId = referral1.Id,
                    AppointmentDateTime = DateTime.UtcNow.AddDays(-25)
                };
                context.Appointments.Add(appointment1);

                var appointment2 = new Appointment
                {
                    PatientId = patient.Id,
                    SpecialistId = doctor.Id,
                    TreatmentId = consultHuid.Id,
                    DepartmentId = department.Id,
                    ReferralId = referral2.Id,
                    AppointmentDateTime = DateTime.UtcNow.AddDays(-15)
                };
                context.Appointments.Add(appointment2);

                var appointment3 = new Appointment
                {
                    PatientId = patient.Id,
                    SpecialistId = doctor.Id,
                    TreatmentId = consultLongen.Id,
                    DepartmentId = department.Id,
                    ReferralId = referral3.Id,
                    AppointmentDateTime = DateTime.UtcNow.AddDays(-5)
                };
                context.Appointments.Add(appointment3);
                context.SaveChanges();

                // 1. Entry with file for Consult hart (Cardiologie)
                var hartEntry = new MedicalRecordEntry
                {
                    PatientId = patient.Id,
                    AppointmentId = appointment1.Id,
                    CreatedById = doctor.Id,
                    CreatedAt = appointment1.AppointmentDateTime,
                    Title = "Consult hart - Cardiologie",
                    Notes = "Patient gecontroleerd voor hartklachten. ECG normaal.",
                    Category = "Cardiologie"
                };
                context.MedicalRecordEntries.Add(hartEntry);
                context.SaveChanges();

                var hartFile = new MedicalRecordFile
                {
                    MedicalRecordEntryId = hartEntry.Id,
                    FileName = "ECG_Onderzoek_Cardiologie.pdf",
                    FileContent = System.Text.Encoding.UTF8.GetBytes(@"ECG RAPPORT - Cardiologie"),
                    ContentType = "application/pdf",
                    FileSize = 2500,
                    UploadedAt = appointment1.AppointmentDateTime,
                    Category = "Cardiologie",
                    Description = "ECG onderzoek resultaten"
                };
                context.MedicalRecordFiles.Add(hartFile);

                // 2. Entry with file for Consult huid (Dermatologie)
                var huidEntry = new MedicalRecordEntry
                {
                    PatientId = patient.Id,
                    AppointmentId = appointment2.Id,
                    CreatedById = doctor.Id,
                    CreatedAt = appointment2.AppointmentDateTime,
                    Title = "Consult huid - Dermatologie",
                    Notes = "Patient gezien voor huidafwijking. Eczema gediagnostiseerd.",
                    Category = "Dermatologie"
                };
                context.MedicalRecordEntries.Add(huidEntry);
                context.SaveChanges();

                var huidFile = new MedicalRecordFile
                {
                    MedicalRecordEntryId = huidEntry.Id,
                    FileName = "Huidfoto_Dermatologie.jpg",
                    FileContent = System.Text.Encoding.UTF8.GetBytes(@"DERMATOLOGIE FOTODOCUMENTATIE"),
                    ContentType = "image/jpeg",
                    FileSize = 3200,
                    UploadedAt = appointment2.AppointmentDateTime,
                    Category = "Dermatologie",
                    Description = "Fotodocumentatie huidafwijking"
                };
                context.MedicalRecordFiles.Add(huidFile);

                // 3. Entry with file for Consult longen (Longgeneeskunde)
                var longenEntry = new MedicalRecordEntry
                {
                    PatientId = patient.Id,
                    AppointmentId = appointment3.Id,
                    CreatedById = doctor.Id,
                    CreatedAt = appointment3.AppointmentDateTime,
                    Title = "Consult longen - Longgeneeskunde",
                    Notes = "Patient gezien voor hoest. Spirometrie normaal.",
                    Category = "Longgeneeskunde"
                };
                context.MedicalRecordEntries.Add(longenEntry);
                context.SaveChanges();

                var longenFile = new MedicalRecordFile
                {
                    MedicalRecordEntryId = longenEntry.Id,
                    FileName = "Rontgen_Thorax_Longgeneeskunde.pdf",
                    FileContent = System.Text.Encoding.UTF8.GetBytes(@"RÖNTGEN THORAX RAPPORT"),
                    ContentType = "application/pdf",
                    FileSize = 3800,
                    UploadedAt = appointment3.AppointmentDateTime,
                    Category = "Longgeneeskunde",
                    Description = "Röntgen thorax PA en lateraal"
                };
                context.MedicalRecordFiles.Add(longenFile);

                context.SaveChanges();

                Console.WriteLine($"✓ Seeded medical entries for {patient.FirstName} {patient.LastName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠ Medical files seeding already done or error: {ex.Message}");
            }
        }
    }
}
