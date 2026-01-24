using backend.Data;
using backend.Models;
using backend.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReferralController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReferralController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult CreateReferral([FromBody] CreateReferralDto request)
        {
            // Log received data for debugging
            Console.WriteLine($"Received: Code={request.Code}, TreatmentId={request.TreatmentId}, PatientId={request.PatientId}, DoctorId={request.DoctorId}, Note={request.Note}");

            // 1️ Controleer of alle verplichte velden zijn ingevuld
            if (string.IsNullOrWhiteSpace(request.Code) ||
                request.TreatmentId == 0 ||
                request.PatientId ==  0 ||
                request.DoctorId ==  0)
            {
                return BadRequest(new { message = "Alle velden moeten ingevuld zijn." });
            }

            // 2 Controleer of code al bestaat
            if (_context.Referrals.Any(r => r.Code == request.Code))
            {
                return BadRequest(new { message = "deze code bestaal al" });
            }

            // 3 Controleer of patient bestaat
            var patient = _context.Users.FirstOrDefault(r => r.Id == request.PatientId);
            if (patient == null)
            {
                return BadRequest(new { message = "De opgegeven patient bestaat niet." });
            }

            // 4 Maak nieuwe doorverwijzing aan
            var referral = new Referral
            {
                Code = request.Code,
                TreatmentId = request.TreatmentId,
                PatientId = request.PatientId,
                DoctorId = request.DoctorId,
                Note = request.Note,
                CreatedAt = DateTime.UtcNow
            };

            // 5 Opslaan in database
            _context.Referrals.Add(referral);
            _context.SaveChanges();

            return Created("/", new { message = "Nieuwe doorverwijzing aangemaakt" });
        }

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetReferralsByPatient(int patientId)
        {
            try
            {
                var referrals = await _context.Referrals
                    .Include(r => r.Treatment)
                    .Include(r => r.Patient)
                    .Include(r => r.Doctor)
                    .Where(r => r.PatientId == patientId)
                    .Select(r => new ReferralDto
                    {
                        Id = r.Id,
                        Code = r.Code,
                        TreatmentId = r.TreatmentId,
                        TreatmentCode = r.Treatment.Code,
                        TreatmentDescription = r.Treatment.Description,
                        PatientId = r.PatientId,
                        PatientName = $"{r.Patient.FirstName} {r.Patient.LastName}",
                        DoctorId = r.DoctorId,
                        DoctorName = $"{r.Doctor.FirstName} {r.Doctor.LastName}",
                        Note = r.Note,
                        CreatedAt = r.CreatedAt
                    })
                    .ToListAsync();

                return Ok(referrals);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching referrals", error = ex.Message });
            }
        }
    }
}