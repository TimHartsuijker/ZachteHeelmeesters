using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumTests.P_O_M;

namespace SeleniumTests;

[TestClass]
public class _4_20_1
{
    [TestMethod]
    public void RetrieveAvailableTimeSlots_ByUserID()
    {
        // Arrange
        IWebDriver? driver = null;
        WebDriverWait? wait = null;
        
        try
        {
            // Step 1: Start test
            Console.WriteLine("LOG [Step 1] Start test: RetrieveAvailableTimeSlots_ByUserID");
            driver = new ChromeDriver();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            // Step 2: Login
            var loginPage = new LoginPage(driver);
            loginPage.Navigate();
            Console.WriteLine("LOG [Step 2] Navigated to login page.");

            string testEmail = "gebruiker@example.com";
            string testPassword = "Wachtwoord123";
            string expectedUserName = "Test Gebruiker";

            loginPage.EnterEmail(testEmail);
            loginPage.EnterPassword(testPassword);
            loginPage.ClickLogin();
            Console.WriteLine("LOG [Step 3] Login credentials entered and submitted.");

            // Step 4: Wait for redirect to agenda page after login
            wait.Until(d => d.Url.Contains("/agenda"));
            Console.WriteLine("LOG [Step 4] Successfully redirected to agenda page.");

            // Step 5: Wait for the calendar to load - check for the user-info element
            wait.Until(d =>
            {
                try
                {
                    var userInfoElement = d.FindElement(By.ClassName("user-info-inline"));
                    return userInfoElement.Displayed;
                }
                catch (NoSuchElementException)
                {
                    return false;
                }
            });
            Console.WriteLine("LOG [Step 5] Calendar page loaded with user information.");

            // Step 6: Verify the displayed user matches the logged-in user
            var userInfo = driver.FindElement(By.ClassName("user-info-inline"));
            string displayedUserInfo = userInfo.Text;
            
            if (!displayedUserInfo.Contains(expectedUserName))
            {
                throw new Exception($"Wrong user displayed. Expected '{expectedUserName}' but got '{displayedUserInfo}'");
            }
            
            if (!displayedUserInfo.Contains(testEmail))
            {
                throw new Exception($"Wrong email displayed. Expected '{testEmail}' but got '{displayedUserInfo}'");
            }
            
            Console.WriteLine($"LOG [Step 6] PASS: Correct user information displayed - {displayedUserInfo}");

            // Step 7: Verify calendar time slots are displayed
            wait.Until(d =>
            {
                try
                {
                    var timeSlots = d.FindElements(By.ClassName("time-slot"));
                    return timeSlots.Count > 0;
                }
                catch
                {
                    return false;
                }
            });
            
            var timeSlotsElements = driver.FindElements(By.ClassName("time-slot"));
            int totalSlots = timeSlotsElements.Count;
            
            Console.WriteLine($"LOG [Step 7] Time slots found: {totalSlots}");

            // Step 8: Verify time slots are present for the user
            if (totalSlots == 0)
            {
                throw new Exception("No time slots found for the logged-in user.");
            }
            
            Console.WriteLine($"LOG [Step 8] PASS: Calendar time slots retrieved successfully for user.");

            // Step 9: Verify no errors are displayed on the page
            var errorElements = driver.FindElements(By.CssSelector(".status-message.error"));
            if (errorElements.Count > 0 && errorElements[0].Displayed)
            {
                throw new Exception($"Error message displayed on page: {errorElements[0].Text}");
            }
            
            Console.WriteLine("LOG [Step 9] PASS: No errors displayed on the calendar page.");

            // Step 10: Test completed successfully
            Console.WriteLine("LOG [Step 10] Test completed successfully - Available time slots retrieved for logged-in user.");
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
                Console.WriteLine("LOG [Step 11] WebDriver closed.");
            }
        }
    }
}
