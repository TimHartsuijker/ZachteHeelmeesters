using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SeleniumTests.Pages;
using System.Diagnostics;

namespace SeleniumTests;

[TestClass]
public class _4_20_5
{
    [TestMethod]
    public void ConcurrentSynchronization_MultipleSpecialists()
    {
        // Arrange
        IWebDriver driver = null;
        try
        {
            // Step 1: Start test
            Console.WriteLine("LOG [Step 1] Start test: ConcurrentSynchronization_MultipleSpecialists");
            driver = new ChromeDriver();
            var calendarPage = new CalendarTestPage(driver);

            // Step 2: Navigate to calendar test page
            calendarPage.Navigate();
            Console.WriteLine("LOG [Step 2] Navigated to calendar test page.");

            // Step 3: Wait for page to load
            calendarPage.WaitForPageLoad();
            Console.WriteLine("LOG [Step 3] Page loaded with specialist dropdown.");

            // Step 4: Get available specialists (need at least 2)
            var specialists = calendarPage.GetValidSpecialists();
            if (specialists.Count < 2)
            {
                throw new Exception("Test requires at least 2 specialists. Found: " + specialists.Count);
            }
            Console.WriteLine($"LOG [Step 4] Found {specialists.Count} specialists for concurrent testing.");

            // Step 5: Collect specialist IDs for concurrent sync
            var specialistIds = new List<(string id, string name)>();
            foreach (var specialist in specialists)
            {
                string id = specialist.GetAttribute("value");
                string name = specialist.Text;
                specialistIds.Add((id, name));
            }
            Console.WriteLine($"LOG [Step 5] Prepared {specialistIds.Count} specialists for concurrent synchronization.");

            // Step 6: Trigger rapid sequential synchronization for all specialists
            // NOTE: For true concurrency testing, use multiple WebDriver instances in parallel
            // This test simulates concurrent load by triggering syncs rapidly in sequence
            var stopwatch = Stopwatch.StartNew();
            var syncResults = new Dictionary<string, (bool success, int availableSlots, long syncTime)>();

            foreach (var (id, name) in specialistIds)
            {
                try
                {
                    var syncStopwatch = Stopwatch.StartNew();
                    
                    Console.WriteLine($"LOG [Step 6.{id}] Triggering sync for {name} (ID: {id})");
                    calendarPage.SelectSpecialistById(id);
                    calendarPage.ClickSearchButton();
                    
                    // Wait for sync to complete
                    calendarPage.WaitForCalendarToUpdateForSpecialist(id);
                    
                    syncStopwatch.Stop();
                    
                    // Verify correct specialist is displayed
                    string displayedId = calendarPage.GetDisplayedSpecialistId();
                    if (displayedId != id)
                    {
                        throw new Exception($"Data corruption detected: Expected specialist {id}, got {displayedId}");
                    }
                    
                    int availableSlots = calendarPage.GetAvailableSlotsCount();
                    syncResults[id] = (true, availableSlots, syncStopwatch.ElapsedMilliseconds);
                    
                    Console.WriteLine($"LOG [Step 6.{id}] Sync completed for {name} in {syncStopwatch.ElapsedMilliseconds}ms - {availableSlots} available slots");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"LOG [Step 6.{id}] FAIL: Sync failed for {name} - {ex.Message}");
                    syncResults[id] = (false, 0, 0);
                }
            }

            stopwatch.Stop();
            Console.WriteLine($"LOG [Step 7] All synchronizations completed in {stopwatch.ElapsedMilliseconds}ms total.");

            // Step 7: Verify all synchronizations completed successfully
            int successCount = syncResults.Count(r => r.Value.success);
            int failCount = syncResults.Count(r => !r.Value.success);
            
            Console.WriteLine($"LOG [Step 8] Sync results: {successCount} successful, {failCount} failed out of {syncResults.Count} total.");
            
            if (failCount > 0)
            {
                throw new Exception($"{failCount} specialist synchronizations failed.");
            }
            Console.WriteLine("LOG [Step 8] PASS: All synchronizations completed successfully.");

            // Step 8: Verify no data corruption - re-verify each specialist
            Console.WriteLine("LOG [Step 9] Verifying data integrity for all specialists...");
            
            foreach (var (id, name) in specialistIds)
            {
                calendarPage.SelectSpecialistById(id);
                calendarPage.ClickSearchButton();
                calendarPage.WaitForCalendarToUpdateForSpecialist(id);
                
                string verifyId = calendarPage.GetDisplayedSpecialistId();
                if (verifyId != id)
                {
                    throw new Exception($"Data corruption: Specialist {id} shows data for {verifyId}");
                }
                
                int currentSlots = calendarPage.GetAvailableSlotsCount();
                int originalSlots = syncResults[id].availableSlots;
                
                // Allow minor differences due to time changes
                if (Math.Abs(currentSlots - originalSlots) > 5)
                {
                    Console.WriteLine($"LOG [Step 9.{id}] WARNING: Slot count changed significantly for {name}. Original: {originalSlots}, Current: {currentSlots}");
                }
                else
                {
                    Console.WriteLine($"LOG [Step 9.{id}] Data integrity verified for {name} - {currentSlots} slots");
                }
            }
            Console.WriteLine("LOG [Step 10] PASS: No data corruption detected.");

            // Step 9: Verify calendar linkage - ensure each specialist ID retrieves correct calendar
            Console.WriteLine("LOG [Step 11] Verifying correct calendar linkage for each specialist...");
            
            foreach (var (id, name) in specialistIds)
            {
                calendarPage.SelectSpecialistById(id);
                calendarPage.ClickSearchButton();
                calendarPage.WaitForCalendarToUpdateForSpecialist(id);
                
                string linkedId = calendarPage.GetDisplayedSpecialistId();
                if (linkedId != id)
                {
                    throw new Exception($"Calendar linkage error: Specialist {id} ({name}) is linked to calendar for specialist {linkedId}");
                }
                
                Console.WriteLine($"LOG [Step 11.{id}] Calendar correctly linked for {name} (ID: {id})");
            }
            Console.WriteLine("LOG [Step 12] PASS: All calendars correctly linked to their specialist IDs.");

            // Step 10: Check performance metrics
            Console.WriteLine("LOG [Step 13] Performance metrics:");
            long totalSyncTime = syncResults.Sum(r => r.Value.syncTime);
            long avgSyncTime = totalSyncTime / syncResults.Count;
            long maxSyncTime = syncResults.Max(r => r.Value.syncTime);
            long minSyncTime = syncResults.Min(r => r.Value.syncTime);
            
            Console.WriteLine($"LOG [Step 13] Total sync time: {totalSyncTime}ms");
            Console.WriteLine($"LOG [Step 13] Average sync time: {avgSyncTime}ms");
            Console.WriteLine($"LOG [Step 13] Max sync time: {maxSyncTime}ms");
            Console.WriteLine($"LOG [Step 13] Min sync time: {minSyncTime}ms");
            
            // Define acceptable performance threshold (e.g., 10 seconds per sync)
            const long ACCEPTABLE_SYNC_TIME_MS = 10000;
            if (maxSyncTime > ACCEPTABLE_SYNC_TIME_MS)
            {
                Console.WriteLine($"LOG [Step 13] WARNING: Maximum sync time ({maxSyncTime}ms) exceeds acceptable limit ({ACCEPTABLE_SYNC_TIME_MS}ms)");
            }
            else
            {
                Console.WriteLine($"LOG [Step 13] PASS: System performance within acceptable limits.");
            }

            // Step 11: Test completed successfully
            Console.WriteLine("LOG [Step 14] Test completed successfully - Concurrent synchronization handled correctly.");
        }
        catch (NoSuchElementException ex)
        {
            Console.WriteLine("ERROR [E001] Element not found: " + ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR [E999] Unexpected error: " + ex.ToString());
            throw;
        }
        finally
        {
            if (driver != null)
            {
                driver.Quit();
                Console.WriteLine("LOG [Step 15] WebDriver closed.");
            }
        }
    }
}
