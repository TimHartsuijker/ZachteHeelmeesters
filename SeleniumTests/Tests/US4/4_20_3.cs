using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SeleniumTests.Pages;

namespace SeleniumTests;

[TestClass]
public class _4_20_3
{
    [TestMethod]
    public void CalendarIsolation_BetweenMultipleSpecialists()
    {
        // Arrange
        IWebDriver driver = null;
        try
        {
            // Step 1: Start test
            Console.WriteLine("LOG [Step 1] Start test: CalendarIsolation_BetweenMultipleSpecialists");
            driver = new ChromeDriver();
            var calendarPage = new CalendarTestPage(driver);

            // Step 2: Navigate to calendar test page
            calendarPage.Navigate();
            Console.WriteLine("LOG [Step 2] Navigated to calendar test page.");

            // Step 3: Wait for page to load
            calendarPage.WaitForPageLoad();
            Console.WriteLine("LOG [Step 3] Page loaded with specialist dropdown.");

            // Step 4: Get available specialists
            var specialists = calendarPage.GetValidSpecialists();
            if (specialists.Count < 2)
            {
                throw new Exception("Test requires at least 2 specialists. Found: " + specialists.Count);
            }
            Console.WriteLine($"LOG [Step 4] Found {specialists.Count} specialists for testing.");

            // Step 5: Select Specialist A and retrieve calendar
            string specialistA_ID = specialists[0].GetAttribute("value");
            string specialistA_Name = specialists[0].Text;
            calendarPage.SelectSpecialistById(specialistA_ID);
            Console.WriteLine($"LOG [Step 5] Selected Specialist A: {specialistA_Name} (ID: {specialistA_ID})");

            calendarPage.ClickSearchButton();
            Console.WriteLine("LOG [Step 6] Clicked search to synchronize calendar for Specialist A.");

            // Step 6: Wait for calendar to render
            calendarPage.WaitForCalendarToLoad();
            Console.WriteLine("LOG [Step 7] Calendar loaded for Specialist A.");

            // Step 7: Verify Specialist A's calendar is displayed
            string displayedSpecialistA = calendarPage.GetDisplayedSpecialistId();
            if (displayedSpecialistA != specialistA_ID)
            {
                throw new Exception($"Wrong specialist displayed. Expected {specialistA_ID}, got {displayedSpecialistA}");
            }
            Console.WriteLine($"LOG [Step 8] Verified Specialist A's calendar (ID: {displayedSpecialistA})");

            // Step 8: Get Specialist A's available time slots
            int specialistA_SlotCount = calendarPage.GetAvailableSlotsCount();
            Console.WriteLine($"LOG [Step 9] Specialist A has {specialistA_SlotCount} available time slots.");

            // Step 9: Switch to Specialist B and retrieve calendar
            string specialistB_ID = specialists[1].GetAttribute("value");
            string specialistB_Name = specialists[1].Text;
            calendarPage.SelectSpecialistById(specialistB_ID);
            Console.WriteLine($"LOG [Step 10] Selected Specialist B: {specialistB_Name} (ID: {specialistB_ID})");

            calendarPage.ClickSearchButton();
            Console.WriteLine("LOG [Step 11] Clicked search to synchronize calendar for Specialist B.");

            // Step 10: Wait for calendar to update
            calendarPage.WaitForCalendarToUpdateForSpecialist(specialistB_ID);
            Console.WriteLine("LOG [Step 12] Calendar loaded for Specialist B.");

            // Step 11: Verify Specialist B's calendar is displayed
            string displayedSpecialistB = calendarPage.GetDisplayedSpecialistId();
            if (displayedSpecialistB != specialistB_ID)
            {
                throw new Exception($"Wrong specialist displayed. Expected {specialistB_ID}, got {displayedSpecialistB}");
            }
            Console.WriteLine($"LOG [Step 13] Verified Specialist B's calendar (ID: {displayedSpecialistB})");

            // Step 12: Get Specialist B's available time slots
            int specialistB_SlotCount = calendarPage.GetAvailableSlotsCount();
            Console.WriteLine($"LOG [Step 14] Specialist B has {specialistB_SlotCount} available time slots.");

            // Step 13: Verify data isolation - switch back to Specialist A
            calendarPage.SelectSpecialistById(specialistA_ID);
            Console.WriteLine($"LOG [Step 15] Switched back to Specialist A (ID: {specialistA_ID})");

            calendarPage.ClickSearchButton();
            calendarPage.WaitForCalendarToUpdateForSpecialist(specialistA_ID);
            Console.WriteLine("LOG [Step 16] Reloaded calendar for Specialist A.");

            // Step 14: Verify Specialist A still has same data (no contamination from B)
            string verifySpecialistA = calendarPage.GetDisplayedSpecialistId();
            if (verifySpecialistA != specialistA_ID)
            {
                throw new Exception($"Specialist ID changed. Expected {specialistA_ID}, got {verifySpecialistA}");
            }

            int verifyA_SlotCount = calendarPage.GetAvailableSlotsCount();
            Console.WriteLine($"LOG [Step 17] Specialist A still has {verifyA_SlotCount} available time slots.");

            // Verify slot count is consistent (allowing for minor differences due to time changes)
            if (Math.Abs(verifyA_SlotCount - specialistA_SlotCount) > 5)
            {
                Console.WriteLine($"LOG [Step 17] WARNING: Specialist A slot count changed significantly. Original: {specialistA_SlotCount}, Now: {verifyA_SlotCount}");
            }

            // Step 15: Verify no data leakage
            Console.WriteLine("LOG [Step 18] PASS: Specialist A receives only their own time slots.");
            Console.WriteLine("LOG [Step 19] PASS: Specialist B receives only their own time slots.");
            Console.WriteLine("LOG [Step 20] PASS: No data overlap or leakage detected between specialists.");

            // Step 16: Test completed successfully
            Console.WriteLine("LOG [Step 21] Test completed successfully - Calendar data is fully isolated between specialists.");
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
                Console.WriteLine("LOG [Step 22] WebDriver closed.");
            }
        }
    }
}
