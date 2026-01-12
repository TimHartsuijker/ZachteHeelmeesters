using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PatientController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PatientResponse>> GetPatient(int id)
        {
            var patient = await _context.Users
                .Include(u => u.Doctor)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (patient == null) return NotFound();

            return Ok(new PatientResponse
            {
                PatientID = patient.Id,
                Voornaam = patient.FirstName,
                Achternaam = patient.LastName,
                Email = patient.Email,
                Straatnaam = patient.StreetName,
                Huisnummer = patient.HouseNumber,
                Postcode = patient.PostalCode,
                Telefoonnummer = patient.PhoneNumber,
                Huisartspraktijk = patient.Doctor?.PracticeName,
                Huisartsnaam = patient.Doctor != null
                    ? $"{patient.Doctor.FirstName} {patient.Doctor.LastName}"
                    : null,
                BSN = patient.CitizenServiceNumber,
                Geboortedatum = patient.DateOfBirth.ToString("dd-MM-yyyy"),
                Geslacht = patient.Gender,
            });
        }
    }
}
