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
        /// AC2.12.1: Get patient's medical dossier (all files)
        /// AC2.12.2: Complete medical history
        /// </summary>
        [HttpGet("patient/{patientId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetPatientDossier(int patientId, [FromQuery] string? category = null, [FromQuery] DateTime? fromDate = null, [FromQuery] DateTime? toDate = null)
        {
            try
            {
                // Check if patient exists
                var patient = await _context.Users.FindAsync(patientId);
                if (patient == null)
                {
                    return NotFound(new { message = "Patiënt niet gevonden" });
                }

                // Build query with filters
                var query = _context.MedicalRecordFiles
                    .Include(f => f.MedicalRecordEntry)
                        .ThenInclude(e => e.CreatedBy)
                    .Include(f => f.MedicalRecordEntry)
                        .ThenInclude(e => e.Appointment)
                    .Where(f => f.MedicalRecordEntry.PatientId == patientId);

                // AC2.12.3: Filter by category
                if (!string.IsNullOrEmpty(category))
                {
                    query = query.Where(f => f.Category == category);
                }

                // AC2.12.3: Filter by date range
                if (fromDate.HasValue)
                {
                    query = query.Where(f => f.UploadedAt >= fromDate.Value);
                }

                if (toDate.HasValue)
                {
                    query = query.Where(f => f.UploadedAt <= toDate.Value);
                }

                // Order by date (most recent first)
                query = query.OrderByDescending(f => f.UploadedAt);

                var files = await query.Select(f => new
                {
                    f.Id,
                    f.FileName,
                    f.ContentType,
                    f.FileSize,
                    f.UploadedAt,
                    f.Category,
                    f.Description,
                    UploadedBy = new
                    {
                        f.MedicalRecordEntry.CreatedBy.Id,
                        f.MedicalRecordEntry.CreatedBy.FirstName,
                        f.MedicalRecordEntry.CreatedBy.LastName
                    },
                    AppointmentDate = f.MedicalRecordEntry.Appointment.AppointmentDateTime
                }).ToListAsync();

                return Ok(files);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting patient dossier for patient {PatientId}", patientId);
                return StatusCode(500, new { message = "Er is een fout opgetreden bij het ophalen van het dossier" });
            }
        }

        /// <summary>
        /// Get unique categories for filtering
        /// </summary>
        [HttpGet("patient/{patientId}/categories")]
        public async Task<ActionResult<IEnumerable<string>>> GetPatientCategories(int patientId)
        {
            try
            {
                var categories = await _context.MedicalRecordFiles
                    .Include(f => f.MedicalRecordEntry)
                    .Where(f => f.MedicalRecordEntry.PatientId == patientId && f.Category != null)
                    .Select(f => f.Category!)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToListAsync();

                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting categories for patient {PatientId}", patientId);
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
        /// Upload a medical document (for doctors)
        /// </summary>
        [HttpPost("upload")]
        public async Task<ActionResult> UploadFile([FromForm] int patientId, [FromForm] int doctorId, [FromForm] int appointmentId, [FromForm] IFormFile file, [FromForm] string? category = null, [FromForm] string? description = null)
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

                // Find or create medical record entry
                var medicalEntry = await _context.MedicalRecordEntries
                    .FirstOrDefaultAsync(e => e.PatientId == patientId && e.AppointmentId == appointmentId);

                if (medicalEntry == null)
                {
                    medicalEntry = new MedicalRecordEntry
                    {
                        PatientId = patientId,
                        AppointmentId = appointmentId,
                        CreatedById = doctorId,
                        CreatedAt = DateTime.UtcNow
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
}
