using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicalDossierController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<MedicalDossierController> _logger;
        private const long MaxFileSize = 10 * 1024 * 1024; // 10 MB

        public MedicalDossierController(AppDbContext context, ILogger<MedicalDossierController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get patient's medical dossier (all entries with files and notes)
        /// </summary>
        [HttpGet("patient/{patientId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetPatientDossier(
            int patientId,
            [FromQuery] string? category = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] int? treatmentId = null,
            [FromQuery] string? treatmentName = null)
        {
            try
            {
                // Check if patient exists
                var patient = await _context.Users.FindAsync(patientId);
                if (patient == null)
                {
                    return NotFound(new { message = "Patiënt niet gevonden" });
                }

                // Build query for entries
                var query = _context.MedicalRecordEntries
                    .Include(e => e.CreatedBy)
                    .Include(e => e.Appointment)
                        .ThenInclude(a => a.Treatment)
                    .Include(e => e.Files)
                    .Where(e => e.PatientId == patientId);

                // Filter by category
                if (!string.IsNullOrEmpty(category))
                {
                    query = query.Where(e => e.Category == category);
                }

                // Filter by treatment
                if (treatmentId.HasValue)
                {
                    query = query.Where(e => e.Appointment != null && e.Appointment.TreatmentId == treatmentId.Value);
                }
                else if (!string.IsNullOrWhiteSpace(treatmentName))
                {
                    query = query.Where(e => e.Appointment != null && e.Appointment.Treatment.Description == treatmentName);
                }

                // Filter by date range
                if (fromDate.HasValue)
                {
                    query = query.Where(e => e.CreatedAt >= fromDate.Value);
                }

                if (toDate.HasValue)
                {
                    query = query.Where(e => e.CreatedAt <= toDate.Value);
                }

                // Order by date (most recent first)
                query = query.OrderByDescending(e => e.CreatedAt);

                var entries = await query.Select(e => new
                {
                    e.Id,
                    e.Title,
                    e.Notes,
                    e.Category,
                    e.CreatedAt,
                    CreatedBy = new
                    {
                        e.CreatedBy.Id,
                        e.CreatedBy.FirstName,
                        e.CreatedBy.LastName
                    },
                    AppointmentDate = e.Appointment != null ? (DateTime?)e.Appointment.AppointmentDateTime : null,
                    Treatment = e.Appointment != null ? new
                    {
                        e.Appointment.Treatment.Id,
                        e.Appointment.Treatment.Code,
                        Name = e.Appointment.Treatment.Description,
                        e.Appointment.Treatment.Specialism
                    } : null,
                    Files = e.Files.Select(f => new
                    {
                        f.Id,
                        f.FileName,
                        f.ContentType,
                        f.FileSize,
                        f.UploadedAt,
                        f.Category,
                        f.Description
                    }).ToList()
                }).ToListAsync();

                return Ok(entries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting patient dossier for patient {PatientId}", patientId);
                return StatusCode(500, new { message = "Er is een fout opgetreden bij het ophalen van het dossier" });
            }
        }

        /// <summary>
        /// Get distinct treatments for patient dossier
        /// </summary>
        [HttpGet("patient/{patientId}/treatments")]
        public async Task<ActionResult<IEnumerable<object>>> GetPatientTreatments(int patientId)
        {
            try
            {
                var treatments = await _context.MedicalRecordEntries
                    .Include(e => e.Appointment)
                        .ThenInclude(a => a.Treatment)
                    .Where(e => e.PatientId == patientId && e.Appointment != null)
                    .Select(e => e.Appointment!.Treatment)
                    .Distinct()
                    .Select(t => new { t.Id, t.Code, Name = t.Description, t.Specialism })
                    .OrderBy(t => t.Name)
                    .ToListAsync();

                return Ok(treatments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting treatments for patient {PatientId}", patientId);
                return StatusCode(500, new { message = "Er is een fout opgetreden" });
            }
        }

        /// <summary>
        /// Download a specific file
        /// </summary>
        [HttpGet("file/{fileId}")]
        public async Task<IActionResult> DownloadFile(int fileId)
        {
            try
            {
                var file = await _context.MedicalRecordFiles.FindAsync(fileId);

                if (file == null)
                {
                    return NotFound(new { message = "Bestand niet gevonden" });
                }

                return File(file.FileContent, file.ContentType, file.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading file {FileId}", fileId);
                return StatusCode(500, new { message = "Er is een fout opgetreden bij het downloaden van het bestand" });
            }
        }

        /// <summary>
        /// Create a note-only entry (no file required)
        /// </summary>
        [HttpPost("entry")]
        public async Task<ActionResult> CreateEntry([FromBody] CreateEntryRequest request)
        {
            try
            {
                // Validate patient exists
                var patient = await _context.Users.FindAsync(request.PatientId);
                if (patient == null)
                {
                    return NotFound(new { message = "Patiënt niet gevonden" });
                }

                // Validate doctor exists
                var doctor = await _context.Users.FindAsync(request.DoctorId);
                if (doctor == null)
                {
                    return NotFound(new { message = "Arts niet gevonden" });
                }

                // Create medical record entry
                var medicalEntry = new MedicalRecordEntry
                {
                    PatientId = request.PatientId,
                    AppointmentId = request.AppointmentId,
                    CreatedById = request.DoctorId,
                    CreatedAt = DateTime.UtcNow,
                    Title = request.Title,
                    Notes = request.Notes,
                    Category = request.Category
                };

                _context.MedicalRecordEntries.Add(medicalEntry);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Notitie succesvol toegevoegd",
                    entryId = medicalEntry.Id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating entry for patient {PatientId}", request.PatientId);
                return StatusCode(500, new { message = "Er is een fout opgetreden bij het toevoegen van de notitie" });
            }
        }

        /// <summary>
        /// Upload a medical document (for doctors)
        /// </summary>
        [HttpPost("upload")]
        public async Task<ActionResult> UploadFile([FromForm] int patientId, [FromForm] int doctorId, [FromForm] int? appointmentId, [FromForm] IFormFile file, [FromForm] string? title = null, [FromForm] string? notes = null, [FromForm] string? category = null, [FromForm] string? description = null)
        {
            try
            {
                // Validate file
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "Geen bestand geselecteerd" });
                }

                if (file.Length > MaxFileSize)
                {
                    return BadRequest(new { message = $"Bestand is te groot. Maximum grootte is {MaxFileSize / 1024 / 1024} MB" });
                }

                // Validate patient exists
                var patient = await _context.Users.FindAsync(patientId);
                if (patient == null)
                {
                    return NotFound(new { message = "Patiënt niet gevonden" });
                }

                // Validate doctor exists
                var doctor = await _context.Users.FindAsync(doctorId);
                if (doctor == null)
                {
                    return NotFound(new { message = "Arts niet gevonden" });
                }

                // Create or find medical record entry
                MedicalRecordEntry? medicalEntry = null;
                
                if (appointmentId.HasValue)
                {
                    medicalEntry = await _context.MedicalRecordEntries
                        .FirstOrDefaultAsync(e => e.PatientId == patientId && e.AppointmentId == appointmentId);
                }

                if (medicalEntry == null)
                {
                    medicalEntry = new MedicalRecordEntry
                    {
                        PatientId = patientId,
                        AppointmentId = appointmentId,
                        CreatedById = doctorId,
                        CreatedAt = DateTime.UtcNow,
                        Title = title,
                        Notes = notes,
                        Category = category
                    };
                    _context.MedicalRecordEntries.Add(medicalEntry);
                    await _context.SaveChangesAsync();
                }

                // Read file content
                byte[] fileContent;
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    fileContent = memoryStream.ToArray();
                }

                // Create file record
                var medicalFile = new MedicalRecordFile
                {
                    MedicalRecordEntryId = medicalEntry.Id,
                    FileName = file.FileName,
                    FileContent = fileContent,
                    ContentType = file.ContentType,
                    FileSize = file.Length,
                    UploadedAt = DateTime.UtcNow,
                    Category = category,
                    Description = description
                };

                _context.MedicalRecordFiles.Add(medicalFile);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Bestand succesvol geüpload",
                    entryId = medicalEntry.Id,
                    fileId = medicalFile.Id,
                    fileName = medicalFile.FileName
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file for patient {PatientId}", patientId);
                return StatusCode(500, new { message = "Er is een fout opgetreden bij het uploaden van het bestand" });
            }
        }

        /// <summary>
        /// Delete a file (for doctors only)
        /// </summary>
        [HttpDelete("file/{fileId}")]
        public async Task<ActionResult> DeleteFile(int fileId)
        {
            try
            {
                var file = await _context.MedicalRecordFiles.FindAsync(fileId);

                if (file == null)
                {
                    return NotFound(new { message = "Bestand niet gevonden" });
                }

                _context.MedicalRecordFiles.Remove(file);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Bestand succesvol verwijderd" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file {FileId}", fileId);
                return StatusCode(500, new { message = "Er is een fout opgetreden bij het verwijderen van het bestand" });
            }
        }
    }

    // Request models
    public class CreateEntryRequest
    {
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int? AppointmentId { get; set; }
        public string? Title { get; set; }
        public string? Notes { get; set; }
        public string? Category { get; set; }
    }
}
