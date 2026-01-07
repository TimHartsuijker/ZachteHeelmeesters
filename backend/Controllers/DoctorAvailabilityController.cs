using Microsoft.AspNetCore.Mvc;
using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorAvailabilityController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DoctorAvailabilityController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{doctorId}")]
        public async Task<ActionResult<List<DoctorAvailability>>> GetAvailability(
            int doctorId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var results = await _context.DoctorAvailabilities
                .Where(a => a.DoctorId == doctorId && a.DateTime >= startDate && a.DateTime <= endDate)
                .OrderBy(a => a.DateTime)
                .ToListAsync();

            return Ok(results);
        }

        [HttpPost("{doctorId}")]
        public async Task<ActionResult<DoctorAvailability>> SetAvailability(
            int doctorId,
            [FromBody] DoctorAvailability availability)
        {
            if (availability.DoctorId != doctorId)
                return BadRequest("Doctor ID mismatch");

            if (!IsOnQuarterHour(availability.DateTime))
                return BadRequest("Time must be on a 15-minute boundary (0, 15, 30, or 45 minutes)");

            var existing = await _context.DoctorAvailabilities
                .FirstOrDefaultAsync(a => a.DoctorId == doctorId && a.DateTime == availability.DateTime);

            if (existing is null)
            {
                availability.CreatedAt = DateTime.UtcNow;
                availability.UpdatedAt = DateTime.UtcNow;
                _context.DoctorAvailabilities.Add(availability);
            }
            else
            {
                existing.IsAvailable = availability.IsAvailable;
                existing.Reason = availability.Reason;
                existing.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAvailability), new { doctorId, startDate = availability.DateTime, endDate = availability.DateTime }, availability);
        }

        [HttpPost("{doctorId}/bulk")]
        public async Task<ActionResult<List<DoctorAvailability>>> SetAvailabilityBulk(
            int doctorId,
            [FromBody] List<DoctorAvailability> availabilities)
        {
            if (availabilities.Any(a => a.DoctorId != doctorId))
                return BadRequest("All entries must have matching doctor ID");

            if (availabilities.Any(a => !IsOnQuarterHour(a.DateTime)))
                return BadRequest("All times must be on 15-minute boundaries");

            var dateTimes = availabilities.Select(a => a.DateTime).ToList();
            var existing = await _context.DoctorAvailabilities
                .Where(a => a.DoctorId == doctorId && dateTimes.Contains(a.DateTime))
                .ToListAsync();

            foreach (var slot in availabilities)
            {
                var match = existing.FirstOrDefault(e => e.DateTime == slot.DateTime);
                if (match is null)
                {
                    slot.CreatedAt = DateTime.UtcNow;
                    slot.UpdatedAt = DateTime.UtcNow;
                    _context.DoctorAvailabilities.Add(slot);
                }
                else
                {
                    match.IsAvailable = slot.IsAvailable;
                    match.Reason = slot.Reason;
                    match.UpdatedAt = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync();
            return Ok(availabilities);
        }

        [HttpDelete("{doctorId}")]
        public async Task<ActionResult> DeleteAvailability(
            int doctorId,
            [FromQuery] DateTime dateTime)
        {
            var existing = await _context.DoctorAvailabilities
                .FirstOrDefaultAsync(a => a.DoctorId == doctorId && a.DateTime == dateTime);

            if (existing is null)
                return NotFound();

            _context.DoctorAvailabilities.Remove(existing);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("{doctorId}/unavailable-period")]
        public async Task<ActionResult<List<DoctorAvailability>>> SetUnavailablePeriod(
            int doctorId,
            [FromQuery] DateTime startDateTime,
            [FromQuery] DateTime endDateTime,
            [FromQuery] string? reason = null)
        {
            if (startDateTime >= endDateTime)
                return BadRequest("Start date must be before end date");

            startDateTime = RoundToFifteenMinutes(startDateTime);
            endDateTime = RoundToFifteenMinutes(endDateTime);

            var slots = GenerateSlots(doctorId, startDateTime, endDateTime, false, reason);
            return await SaveSlots(slots);
        }

        [HttpPost("{doctorId}/available-period")]
        public async Task<ActionResult<List<DoctorAvailability>>> SetAvailablePeriod(
            int doctorId,
            [FromQuery] DateTime startDateTime,
            [FromQuery] DateTime endDateTime)
        {
            if (startDateTime >= endDateTime)
                return BadRequest("Start date must be before end date");

            startDateTime = RoundToFifteenMinutes(startDateTime);
            endDateTime = RoundToFifteenMinutes(endDateTime);

            var slots = GenerateSlots(doctorId, startDateTime, endDateTime, true, null);
            return await SaveSlots(slots);
        }

        private List<DoctorAvailability> GenerateSlots(int doctorId, DateTime start, DateTime end, bool isAvailable, string? reason)
        {
            var slots = new List<DoctorAvailability>();
            var current = start;
            while (current < end)
            {
                slots.Add(new DoctorAvailability
                {
                    DoctorId = doctorId,
                    DateTime = current,
                    IsAvailable = isAvailable,
                    Reason = isAvailable ? null : reason,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });

                current = current.AddMinutes(15);
            }
            return slots;
        }

        private async Task<ActionResult<List<DoctorAvailability>>> SaveSlots(List<DoctorAvailability> slots)
        {
            if (slots.Count == 0)
                return Ok(slots);

            var doctorId = slots.First().DoctorId;
            var dateTimes = slots.Select(s => s.DateTime).ToList();
            var existing = await _context.DoctorAvailabilities
                .Where(a => a.DoctorId == doctorId && dateTimes.Contains(a.DateTime))
                .ToListAsync();

            foreach (var slot in slots)
            {
                var match = existing.FirstOrDefault(e => e.DateTime == slot.DateTime);
                if (match is null)
                {
                    _context.DoctorAvailabilities.Add(slot);
                }
                else
                {
                    match.IsAvailable = slot.IsAvailable;
                    match.Reason = slot.Reason;
                    match.UpdatedAt = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync();
            return Ok(slots);
        }

        private bool IsOnQuarterHour(DateTime dateTime) => dateTime.Minute % 15 == 0;

        private DateTime RoundToFifteenMinutes(DateTime dateTime)
        {
            var minutes = dateTime.Minute;
            var roundedMinutes = (minutes / 15) * 15;
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, roundedMinutes, 0, dateTime.Kind);
        }
    }
}
