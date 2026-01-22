using backend.Data;
using backend.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TreatmentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TreatmentsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetTreatments()
        {
            try
            {
                var treatments = await _context.Treatments
                    .Select(t => new TreatmentDto
                    {
                        Id = t.Id,
                        Code = t.Code,
                        Description = t.Description
                    })
                    .ToListAsync();

                return Ok(treatments);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Error fetching treatments." });
            }
        }
    }
}
