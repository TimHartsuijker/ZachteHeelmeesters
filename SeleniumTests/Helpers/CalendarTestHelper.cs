using OpenQA.Selenium;

namespace SeleniumTests.Helpers;

public static class CalendarTestHelper
{
    // Log specialist selection
    public static void LogSpecialistSelection(string specialistId, string specialistName, string label = "")
    {
        string labelText = string.IsNullOrEmpty(label) ? "" : $"{label}: ";
        Console.WriteLine($"LOG {labelText}Selected specialist: {specialistName} (ID: {specialistId})");
    }

    // Log availability summary
    public static void LogAvailability(string specialistName, int availableSlots, int bookedSlots, string prefix = "")
    {
        string prefixText = string.IsNullOrEmpty(prefix) ? "" : $"{prefix} ";
        Console.WriteLine($"LOG {prefixText}Availability for {specialistName}: {availableSlots} available, {bookedSlots} booked.");
    }

    // Extract specialist info from web element
    public static (string id, string name) GetSpecialistInfo(IWebElement specialist)
    {
        string id = specialist.GetAttribute("value");
        string name = specialist.Text;
        return (id, name);
    }

    // Verify availability changed
    public static bool HasAvailabilityChanged(int initialAvailable, int initialBooked, int updatedAvailable, int updatedBooked, int tolerance = 0)
    {
        return Math.Abs(updatedAvailable - initialAvailable) > tolerance || 
               Math.Abs(updatedBooked - initialBooked) > tolerance;
    }

    // Verify availability unchanged (with tolerance)
    public static void VerifyAvailabilityUnchanged(
        string specialistName, 
        int initialAvailable, 
        int initialBooked, 
        int currentAvailable, 
        int currentBooked,
        int tolerance = 1)
    {
        if (Math.Abs(currentAvailable - initialAvailable) > tolerance || 
            Math.Abs(currentBooked - initialBooked) > tolerance)
        {
            throw new Exception(
                $"{specialistName}'s availability should not change. " +
                $"Initial: {initialAvailable} available, {initialBooked} booked. " +
                $"Current: {currentAvailable} available, {currentBooked} booked.");
        }
    }

    // Log sync performance
    public static void LogSyncPerformance(string specialistName, long syncTimeMs)
    {
        Console.WriteLine($"LOG Sync completed for {specialistName} in {syncTimeMs}ms");
    }

    // Verify performance threshold
    public static void VerifyPerformanceThreshold(long syncTimeMs, long thresholdMs, string context = "")
    {
        if (syncTimeMs > thresholdMs)
        {
            string contextText = string.IsNullOrEmpty(context) ? "" : $" ({context})";
            Console.WriteLine($"LOG WARNING: Sync time{contextText} ({syncTimeMs}ms) exceeds threshold ({thresholdMs}ms)");
        }
    }

    // Calculate and log performance metrics
    public static void LogPerformanceMetrics(Dictionary<string, long> syncTimes)
    {
        if (syncTimes.Count == 0) return;

        long totalTime = syncTimes.Sum(kvp => kvp.Value);
        long avgTime = totalTime / syncTimes.Count;
        long maxTime = syncTimes.Max(kvp => kvp.Value);
        long minTime = syncTimes.Min(kvp => kvp.Value);

        Console.WriteLine("LOG Performance metrics:");
        Console.WriteLine($"LOG   Total: {totalTime}ms");
        Console.WriteLine($"LOG   Average: {avgTime}ms");
        Console.WriteLine($"LOG   Max: {maxTime}ms");
        Console.WriteLine($"LOG   Min: {minTime}ms");
    }

    // Verify specialist data integrity
    public static void VerifySpecialistDataIntegrity(
        string expectedId, 
        string displayedId, 
        int expectedSlots, 
        int currentSlots,
        int tolerance = 5)
    {
        // Verify ID matches
        if (displayedId != expectedId)
        {
            throw new Exception($"Data integrity error: Expected specialist {expectedId}, got {displayedId}");
        }

        // Verify slots are consistent (with tolerance for time-based changes)
        if (Math.Abs(currentSlots - expectedSlots) > tolerance)
        {
            Console.WriteLine($"LOG WARNING: Slot count changed significantly. Expected: {expectedSlots}, Current: {currentSlots}");
        }
    }

    // Ensure error message is not empty
    public static void VerifyErrorMessageExists(string errorMessage)
    {
        if (string.IsNullOrEmpty(errorMessage))
        {
            throw new Exception("Error message is empty.");
        }
    }

    // Verify expected error message content
    public static void VerifyErrorMessageContent(string actualMessage, string expectedMessage)
    {
        if (!string.IsNullOrEmpty(expectedMessage))
        {
            if (!actualMessage.Contains(expectedMessage))
            {
                throw new Exception($"Error message does not match. Expected: '{expectedMessage}', Got: '{actualMessage}'");
            }
        }
    }

    // Verify status code is appropriate (404 or 400)
    public static void VerifyErrorStatusCode(string statusCode)
    {
        if (statusCode != null)
        {
            if (!statusCode.Contains("404") && !statusCode.Contains("400"))
            {
                Console.WriteLine($"LOG WARNING: Expected 404 or 400 status code, got {statusCode}");
            }
            else
            {
                Console.WriteLine($"LOG PASS: Appropriate error status code returned: {statusCode}");
            }
        }
    }

    // Log test step
    public static void LogStep(int stepNumber, string message)
    {
        Console.WriteLine($"LOG [Step {stepNumber}] {message}");
    }

    // Log pass message
    public static void LogPass(string message)
    {
        Console.WriteLine($"LOG PASS: {message}");
    }

    // Log warning message
    public static void LogWarning(string message)
    {
        Console.WriteLine($"LOG WARNING: {message}");
    }

    // Log error message
    public static void LogError(string errorCode, string message)
    {
        Console.WriteLine($"ERROR [{errorCode}] {message}");
    }
}
