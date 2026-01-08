using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SeleniumTests.Pages;

namespace SeleniumTests;

[TestClass]
public class _4_20_1
{
    [TestMethod]
    public void RetrieveAvailableTimeSlots_BySpecialistID()
    {
        // Arrange
        IWebDriver driver = null;
        try
        {
            // Step 1: Start test
            Console.WriteLine("LOG [Step 1] Start test: RetrieveAvailableTimeSlots_BySpecialistID");
            driver = new ChromeDriver();
            var calendarPage = new CalendarTestPage(driver);

            // Step 2: Navigate to calendar test page
            calendarPage.Navigate();
            Console.WriteLine("LOG [Step 2] Navigated to calendar test page.");

            // Step 3: Wait for page to load
            calendarPage.WaitForPageLoad();
            Console.WriteLine("LOG [Step 3] Page loaded with specialist dropdown.");

            // Step 4: Get specialists and select first one
            var specialists = calendarPage.GetValidSpecialists();
            if (specialists.Count == 0)
            {
                throw new Exception("No valid specialists found in dropdown.");
            }

            string selectedSpecialistId = specialists[0].GetAttribute("value");
            string selectedSpecialistName = specialists[0].Text;
            calendarPage.SelectSpecialistById(selectedSpecialistId);
            Console.WriteLine($"LOG [Step 4] Selected specialist: {selectedSpecialistName} (ID: {selectedSpecialistId})");

            // Step 5: Submit request - Click search button to retrieve available time slots
            calendarPage.ClickSearchButton();
            Console.WriteLine("LOG [Step 5] Clicked search button to retrieve calendar for specialist.");

            // Step 6: Wait for calendar to render
            calendarPage.WaitForCalendarToLoad();
            Console.WriteLine("LOG [Step 6] Calendar view rendered successfully.");

            // Step 7: Verify the correct specialist's calendar is displayed
            string displayedSpecialistId = calendarPage.GetDisplayedSpecialistId();
            if (displayedSpecialistId != selectedSpecialistId)
            {
                throw new Exception($"Wrong specialist calendar displayed. Expected {selectedSpecialistId}, got {displayedSpecialistId}");
            }
            Console.WriteLine($"LOG [Step 7] PASS: Correct specialist calendar retrieved (ID: {displayedSpecialistId})");

            // Step 8: Verify available time slots are displayed (not booked appointments)
            int availableSlots = calendarPage.GetAvailableSlotsCount();
            int bookedSlots = calendarPage.GetBookedSlotsCount();
            
            Console.WriteLine($"LOG [Step 8] Available slots found: {availableSlots}");
            Console.WriteLine($"LOG [Step 9] Booked slots found: {bookedSlots}");

            // Step 9: Verify only available time slots are returned (no booked appointments)
            if (availableSlots == 0)
            {
                throw new Exception("No available time slots returned for specialist.");
            }
            
            if (bookedSlots > 0)
            {
                throw new Exception("Booked appointments should be filtered out - only available slots should be returned.");
            }

            Console.WriteLine("LOG [Step 10] PASS: Only available time slots are displayed (no booked appointments).");

            // Step 10: Test completed successfully
            Console.WriteLine("LOG [Step 11] Test completed successfully - Available time slots retrieved for specialist via calendar view.");
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
                Console.WriteLine("LOG [Step 12] WebDriver closed.");
            }
        }
    }
}
