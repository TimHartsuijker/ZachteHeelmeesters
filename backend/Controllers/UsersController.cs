using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;
using backend.DTOs;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var gebruikers = await _context.Users
                    .OrderBy(u => u.Id)
                    .Select(u => new UserResponseDto // Map to DTO
                    {
                        Id = u.Id,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Email = u.Email,
                        RoleId = u.RoleId,
                        DoctorId = u.DoctorId
                    })
                    .ToListAsync();

                Console.WriteLine(gebruikers);

                return Ok(gebruikers);
            }

            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching users", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser(int id, UserUpdateDto userUpdateDto)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null) return NotFound();

                user.RoleId = userUpdateDto.RoleId;

                await _context.SaveChangesAsync();
                return Ok(new { message = "User updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating user", error = ex.Message });
            }
        }

        [HttpGet("doctor/{doctorId}/patients")]
        public async Task<IActionResult> GetDoctorPatients(int doctorId)
        {
            try
            {
                Console.WriteLine($"[GetDoctorPatients] Looking for patients with DoctorId = {doctorId}");
                
                // Verify doctor exists
                var doctor = await _context.Users.FindAsync(doctorId);
                if (doctor == null)
                {
                    Console.WriteLine($"[GetDoctorPatients] Doctor with ID {doctorId} not found");
                    return NotFound(new { message = "Doctor not found" });
                }

                Console.WriteLine($"[GetDoctorPatients] Found doctor: {doctor.Email}");

                // Get all patients assigned to this doctor
                var patients = await _context.Users
                    .Where(u => u.DoctorId == doctorId)
                    .Include(u => u.Role)
                    .ToListAsync();

                Console.WriteLine($"[GetDoctorPatients] Found {patients.Count} patients for doctor ID {doctorId}");
                foreach (var p in patients)
                {
                    Console.WriteLine($"[GetDoctorPatients]   - Patient: {p.Email}, DoctorId: {p.DoctorId}");
                }

                var result = patients.Select(u => new
                {
                    u.Id,
                    u.FirstName,
                    u.LastName,
                    u.Email,
                    u.PhoneNumber,
                    u.DateOfBirth,
                    u.StreetName,
                    u.HouseNumber,
                    u.PostalCode,
                    u.CitizenServiceNumber,
                    u.Gender,
                    Role = u.Role.RoleName
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetDoctorPatients] Error: {ex.Message}");
                return StatusCode(500, new { message = "Error fetching patients", error = ex.Message });
            }
        }

        [HttpGet("{patientId}/medical-record")]
        public async Task<IActionResult> GetMedicalRecord(int patientId)
        {
            try
            {
                // Get patient with all medical record entries
                var patient = await _context.Users
                    .Where(u => u.Id == patientId)
                    .Include(u => u.MedicalRecordEntries)
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync();

                if (patient == null)
                    return NotFound(new { message = "Patient not found" });

                // Build medical record response
                var medicalRecord = new
                {
                    patient.Id,
                    patient.FirstName,
                    patient.LastName,
                    patient.Email,
                    patient.PhoneNumber,
                    patient.DateOfBirth,
                    patient.StreetName,
                    patient.HouseNumber,
                    patient.PostalCode,
                    patient.CitizenServiceNumber,
                    patient.Gender,
                    Role = patient.Role?.RoleName,
                    Complaints = patient.MedicalRecordEntries
                        .Where(e => e.Title == "Klacht")
                        .OrderByDescending(e => e.CreatedAt)
                        .FirstOrDefault()?.Notes,
                    Diagnoses = patient.MedicalRecordEntries
                        .Where(e => e.Title == "Diagnose")
                        .OrderByDescending(e => e.CreatedAt)
                        .FirstOrDefault()?.Notes,
                    Treatments = patient.MedicalRecordEntries
                        .Where(e => e.Title == "Behandeling")
                        .OrderByDescending(e => e.CreatedAt)
                        .FirstOrDefault()?.Notes,
                    Referrals = patient.MedicalRecordEntries
                        .Where(e => e.Title == "Doorverwijzing")
                        .OrderByDescending(e => e.CreatedAt)
                        .FirstOrDefault()?.Notes,
                    MedicalRecordEntries = patient.MedicalRecordEntries
                        .OrderByDescending(e => e.CreatedAt)
                        .Select(e => new
                        {
                            e.Id,
                            e.Title,
                            e.Notes,
                            e.CreatedAt
                        })
                };

                return Ok(medicalRecord);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching medical record", error = ex.Message });
            }
        }
        [HttpGet("all-with-roles")]
        public async Task<IActionResult> GetAllUsersWithRoles()
        {
            try
            {
                var users = await _context.Users
                    .Include(u => u.Role)
                    .OrderByDescending(u => u.CreatedAt)
                    .Select(u => new
                    {
                        u.Id,
                        u.FirstName,
                        u.LastName,
                        u.Email,
                        u.PhoneNumber,
                        u.CreatedAt,
                        RoleId = u.Role.Id,
                        RoleName = u.Role.RoleName
                    })
                    .ToListAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching users", error = ex.Message });
            }
        }
    }
}