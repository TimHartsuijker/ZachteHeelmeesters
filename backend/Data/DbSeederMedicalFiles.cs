using backend.Data;
using backend.Models;

namespace backend.Data
{
    public static class DbSeederMedicalFiles
    {
        public static void Seed(AppDbContext context)
        {
            // Check if we already have some test files
            if (context.MedicalRecordFiles.Any())
            {
                return; // Database already seeded
            }

            // Create test medical record entries and files only if we have appointments
            var appointments = context.Appointments.ToList();
            if (!appointments.Any())
            {
                return; // No appointments to attach files to
            }

            var firstAppointment = appointments.First();
            var patient = context.Users.Find(firstAppointment.PatientId);
            var doctor = context.Users.Find(firstAppointment.SpecialistId);

            if (patient == null || doctor == null)
            {
                return; // Can't seed without patient and doctor
            }

            // Create a medical record entry
            var medicalEntry = new MedicalRecordEntry
            {
                PatientId = patient.Id,
                AppointmentId = firstAppointment.Id,
                CreatedById = doctor.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-30)
            };
            context.MedicalRecordEntries.Add(medicalEntry);
            context.SaveChanges();

            // Create some sample files with dummy content
            var sampleFiles = new[]
            {
                new MedicalRecordFile
                {
                    MedicalRecordEntryId = medicalEntry.Id,
                    FileName = "Bloedonderzoek_Resultaten.pdf",
                    FileContent = System.Text.Encoding.UTF8.GetBytes("Sample PDF content for blood test results"),
                    ContentType = "application/pdf",
                    FileSize = 45000,
                    UploadedAt = DateTime.UtcNow.AddDays(-30),
                    Category = "Labresultaten",
                    Description = "Bloedonderzoek uitgevoerd op 15 december 2025"
                },
                new MedicalRecordFile
                {
                    MedicalRecordEntryId = medicalEntry.Id,
                    FileName = "Rontgen_Foto_Borst.jpg",
                    FileContent = System.Text.Encoding.UTF8.GetBytes("Sample JPEG content for X-ray"),
                    ContentType = "image/jpeg",
                    FileSize = 120000,
                    UploadedAt = DateTime.UtcNow.AddDays(-25),
                    Category = "Röntgenfoto",
                    Description = "Röntgenfoto van de borstkas"
                },
                new MedicalRecordFile
                {
                    MedicalRecordEntryId = medicalEntry.Id,
                    FileName = "Verwijsbrief_Specialist.docx",
                    FileContent = System.Text.Encoding.UTF8.GetBytes("Sample Word document content"),
                    ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    FileSize = 28000,
                    UploadedAt = DateTime.UtcNow.AddDays(-20),
                    Category = "Verwijsbrief",
                    Description = "Verwijzing naar cardioloog"
                },
                new MedicalRecordFile
                {
                    MedicalRecordEntryId = medicalEntry.Id,
                    FileName = "Medicatielijst_Actueel.pdf",
                    FileContent = System.Text.Encoding.UTF8.GetBytes("Sample medication list content"),
                    ContentType = "application/pdf",
                    FileSize = 35000,
                    UploadedAt = DateTime.UtcNow.AddDays(-10),
                    Category = "Medicatielijst",
                    Description = "Actuele medicatielijst per januari 2026"
                },
                new MedicalRecordFile
                {
                    MedicalRecordEntryId = medicalEntry.Id,
                    FileName = "Ontslagbrief_Ziekenhuis.pdf",
                    FileContent = System.Text.Encoding.UTF8.GetBytes("Sample discharge letter content"),
                    ContentType = "application/pdf",
                    FileSize = 52000,
                    UploadedAt = DateTime.UtcNow.AddDays(-5),
                    Category = "Ontslagbrief",
                    Description = "Ontslagbrief na ziekenhuisopname"
                }
            };

            context.MedicalRecordFiles.AddRange(sampleFiles);
            context.SaveChanges();

            Console.WriteLine($"✓ Seeded {sampleFiles.Length} medical files for patient {patient.FirstName} {patient.LastName}");
        }
    }
}
