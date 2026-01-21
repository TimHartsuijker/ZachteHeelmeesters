using backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RolesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            try
            {
                var rollen = await _context.Roles
                   .OrderBy(r => r.Id)
                   .ToListAsync();
                return Ok(rollen);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Error fetching roles" });
            }
        }
    }
}