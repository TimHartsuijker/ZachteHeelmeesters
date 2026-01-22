using backend.Data;
using backend.Models;
using backend.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
            Console.WriteLine($"Received: Code={request.Code}, TreatmentId={request.TreatmentId}, PatientId={request.PatientId}, DoctorId={request.DoctorId}");

            // 1️ Controleer of alle verplichte velden zijn ingevuld
            if (string.IsNullOrWhiteSpace(request.Code) ||
                request.TreatmentId == 0 ||
                request.PatientId ==  0 ||
                request.DoctorId ==  0)
            {
                return BadRequest(new { message = "Alle velden moeten ingevuld zijn." });
            }

            // 2 Controleer of patient bestaat
            var patient = _context.Users.FirstOrDefault(r => r.Id == request.PatientId);
            if (patient == null)
            {
                return BadRequest(new { message = "De opgegeven patient bestaat niet." });
            }

            // 3 Maak nieuwe doorverwijzing aan
            var referral = new Referral
            {
                Code = request.Code,
                TreatmentId = request.TreatmentId,
                PatientId = request.PatientId,
                DoctorId = request.DoctorId,
                CreatedAt = DateTime.UtcNow
            };

            // 4 Opslaan in database
            _context.Referrals.Add(referral);
            _context.SaveChanges();

            return Created("/", new { message = "Nieuwe doorverwijzing aangemaakt" });
        }
    }
}