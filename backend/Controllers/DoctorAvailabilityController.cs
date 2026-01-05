using Microsoft.AspNetCore.Mvc;
using backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorAvailabilityController : ControllerBase
    {
        // TODO: Replace with actual database context dependency injection
        // This is a placeholder implementation showing the expected API structure
        
        /// <summary>
        /// Get doctor's availability for a specific date range
        /// </summary>
        /// <param name="doctorId">The doctor's user ID</param>
        /// <param name="startDate">Start date for the range (inclusive)</param>
        /// <param name="endDate">End date for the range (inclusive)</param>
        /// <returns>List of availability entries for the specified date range</returns>
        [HttpGet("{doctorId}")]
        public ActionResult<List<DoctorAvailability>> GetAvailability(
            int doctorId, 
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            // TODO: Implement database query
            // Should fetch all availability entries for the doctor within the date range
            // WHERE doctor_id = doctorId AND date_time >= startDate AND date_time <= endDate
            return Ok(new List<DoctorAvailability>());
        }

        /// <summary>
        /// Create or update availability for a time slot
        /// </summary>
        /// <param name="doctorId">The doctor's user ID</param>
        /// <param name="availability">The availability data to save</param>
        /// <returns>The created/updated availability entry</returns>
        [HttpPost("{doctorId}")]
        public ActionResult<DoctorAvailability> SetAvailability(
            int doctorId, 
            [FromBody] DoctorAvailability availability)
        {
            // Validate the doctor ID matches
            if (availability.DoctorId != doctorId)
                return BadRequest("Doctor ID mismatch");

            // Validate that the time slot is on a 15-minute boundary
            if (availability.DateTime.Minute % 15 != 0)
                return BadRequest("Time must be on a 15-minute boundary (0, 15, 30, or 45 minutes)");

            // TODO: Implement database insert/update
            // Should create or update the availability entry
            // Use UPSERT logic (INSERT OR UPDATE) based on unique constraint (doctor_id, date_time)
            
            return CreatedAtAction(nameof(GetAvailability), 
                new { doctorId }, availability);
        }

        /// <summary>
        /// Bulk update availability for multiple time slots
        /// </summary>
        /// <param name="doctorId">The doctor's user ID</param>
        /// <param name="availabilities">List of availability entries to save</param>
        /// <returns>The created/updated availability entries</returns>
        [HttpPost("{doctorId}/bulk")]
        public ActionResult<List<DoctorAvailability>> SetAvailabilityBulk(
            int doctorId, 
            [FromBody] List<DoctorAvailability> availabilities)
        {
            // Validate doctor IDs
            if (availabilities.Any(a => a.DoctorId != doctorId))
                return BadRequest("All entries must have matching doctor ID");

            // Validate all time slots are on 15-minute boundaries
            if (availabilities.Any(a => a.DateTime.Minute % 15 != 0))
                return BadRequest("All times must be on 15-minute boundaries");

            // TODO: Implement database batch insert/update
            // Should efficiently insert/update multiple entries in a transaction
            
            return Ok(availabilities);
        }

        /// <summary>
        /// Delete availability entry for a specific time slot
        /// </summary>
        /// <param name="doctorId">The doctor's user ID</param>
        /// <param name="dateTime">The date and time of the slot to delete</param>
        /// <returns>No content response</returns>
        [HttpDelete("{doctorId}")]
        public ActionResult DeleteAvailability(
            int doctorId, 
            [FromQuery] DateTime dateTime)
        {
            // TODO: Implement database delete
            // DELETE FROM doctor_availability 
            // WHERE doctor_id = doctorId AND date_time = dateTime
            
            return NoContent();
        }

        /// <summary>
        /// Set unavailable time period (e.g., vacation, illness)
        /// </summary>
        /// <param name="doctorId">The doctor's user ID</param>
        /// <param name="startDateTime">Start of unavailable period</param>
        /// <param name="endDateTime">End of unavailable period</param>
        /// <param name="reason">Reason for unavailability</param>
        /// <returns>List of created unavailable slots</returns>
        [HttpPost("{doctorId}/unavailable-period")]
        public ActionResult<List<DoctorAvailability>> SetUnavailablePeriod(
            int doctorId,
            [FromQuery] DateTime startDateTime,
            [FromQuery] DateTime endDateTime,
            [FromQuery] string? reason = null)
        {
            // Validate date range
            if (startDateTime >= endDateTime)
                return BadRequest("Start date must be before end date");

            // Round to nearest 15-minute boundary
            startDateTime = RoundToFifteenMinutes(startDateTime);
            endDateTime = RoundToFifteenMinutes(endDateTime);

            // TODO: Generate all 15-minute slots between start and end
            // and set them as unavailable with the provided reason

            return Ok(new List<DoctorAvailability>());
        }

        /// <summary>
        /// Set available time period (e.g., mark as available again)
        /// </summary>
        /// <param name="doctorId">The doctor's user ID</param>
        /// <param name="startDateTime">Start of available period</param>
        /// <param name="endDateTime">End of available period</param>
        /// <returns>List of updated slots</returns>
        [HttpPost("{doctorId}/available-period")]
        public ActionResult<List<DoctorAvailability>> SetAvailablePeriod(
            int doctorId,
            [FromQuery] DateTime startDateTime,
            [FromQuery] DateTime endDateTime)
        {
            // Validate date range
            if (startDateTime >= endDateTime)
                return BadRequest("Start date must be before end date");

            // Round to nearest 15-minute boundary
            startDateTime = RoundToFifteenMinutes(startDateTime);
            endDateTime = RoundToFifteenMinutes(endDateTime);

            // TODO: Set all 15-minute slots between start and end as available
            
            return Ok(new List<DoctorAvailability>());
        }

        /// <summary>
        /// Helper method to round datetime to nearest 15-minute boundary
        /// </summary>
        private DateTime RoundToFifteenMinutes(DateTime dateTime)
        {
            int minutes = dateTime.Minute;
            int roundedMinutes = (minutes / 15) * 15;
            return dateTime.AddMinutes(roundedMinutes - minutes).AddSeconds(-dateTime.Second).AddMilliseconds(-dateTime.Millisecond);
        }
    }
}
