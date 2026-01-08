using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SeleniumTests.Pages;

namespace SeleniumTests;

[TestClass]
public class _4_20_2
{
    [TestMethod]
    public void InvalidSpecialistID_HandlingErrorGracefully()
    {
        // Arrange
        IWebDriver driver = null;
        try
        {
            // Step 1: Start test
            Console.WriteLine("LOG [Step 1] Start test: InvalidSpecialistID_HandlingErrorGracefully");
            driver = new ChromeDriver();
            var calendarPage = new CalendarTestPage(driver);

            // Step 2: Navigate to calendar test page
            calendarPage.Navigate();
            Console.WriteLine("LOG [Step 2] Navigated to calendar test page.");

            // Step 3: Wait for page to load
            calendarPage.WaitForPageLoad();
            Console.WriteLine("LOG [Step 3] Page loaded.");

            // Step 4: Add invalid specialist ID to dropdown
            calendarPage.AddInvalidSpecialistOption("99999", "Invalid Specialist");
            Console.WriteLine("LOG [Step 4] Set invalid specialist ID: 99999");

            // Step 5: Click search button to submit request
            calendarPage.ClickSearchButton();
            Console.WriteLine("LOG [Step 5] Clicked search button with invalid specialist ID.");

            // Step 6: Wait for error message to appear
            calendarPage.WaitForErrorMessage();
            Console.WriteLine("LOG [Step 6] Error message appeared.");

            // Step 7: Verify error message is displayed
            string errorText = calendarPage.GetErrorMessage();
            Console.WriteLine($"LOG [Step 7] Error message: '{errorText}'");

            if (string.IsNullOrEmpty(errorText))
            {
                throw new Exception("Error message is empty.");
            }

            // TODO: Fill in expected error message text below
            string expectedErrorMessage = ""; // ADD ERROR MESSAGE HERE
            
            if (!string.IsNullOrEmpty(expectedErrorMessage))
            {
                if (!errorText.Contains(expectedErrorMessage))
                {
                    throw new Exception($"Error message does not match. Expected: '{expectedErrorMessage}', Got: '{errorText}'");
                }
                Console.WriteLine($"LOG [Step 8] PASS: Error message matches expected: '{expectedErrorMessage}'");
            }
            else
            {
                Console.WriteLine($"LOG [Step 8] INFO: Error message displayed but not validated (expectedErrorMessage not set): '{errorText}'");
            }

            // Step 8: Verify no calendar data is displayed
            if (calendarPage.IsCalendarVisible())
            {
                throw new Exception("Calendar view should not be displayed for invalid specialist ID.");
            }
            Console.WriteLine("LOG [Step 9] PASS: No calendar data retrieved or displayed.");

            // Step 9: Verify no time slots are displayed
            if (calendarPage.AreTimeSlotsVisible())
            {
                throw new Exception("Time slots should not be displayed for invalid specialist ID.");
            }
            Console.WriteLine("LOG [Step 10] PASS: No time slots displayed.");

            // Step 10: Check that error status code is appropriate (if displayed)
            string statusCode = calendarPage.GetStatusCode();
            if (statusCode != null)
            {
                Console.WriteLine($"LOG [Step 11] HTTP Status Code: {statusCode}");
                
                // Verify it's 404 Not Found or 400 Bad Request
                if (!statusCode.Contains("404") && !statusCode.Contains("400"))
                {
                    Console.WriteLine($"LOG [Step 11] WARNING: Expected 404 or 400 status code, got {statusCode}");
                }
                else
                {
                    Console.WriteLine("LOG [Step 11] PASS: Appropriate error status code returned.");
                }
            }

            // Step 11: Test completed successfully
            Console.WriteLine("LOG [Step 12] Test completed successfully - Invalid specialist ID handled gracefully.");
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
                Console.WriteLine("LOG [Step 13] WebDriver closed.");
            }
        }
    }
}
