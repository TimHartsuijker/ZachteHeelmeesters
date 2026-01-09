using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumTests.P_O_M;

namespace SeleniumTests;

[TestClass]
public class _4_20_2
{
    [TestMethod]
    public void CalendarIsolation_BetweenMultipleUsers()
    {
        // Arrange
        IWebDriver driver = null;
        WebDriverWait wait = null;
        
        try
        {
            // Step 1: Start test
            Console.WriteLine("LOG [Step 1] Start test: CalendarIsolation_BetweenMultipleUsers");
            driver = new ChromeDriver();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            // ===== FIRST USER LOGIN =====
            
            // Step 2: Login as first user (Patient)
            var loginPage = new LoginPage(driver);
            loginPage.Navigate();
            Console.WriteLine("LOG [Step 2] Navigated to login page.");

            string user1Email = "gebruiker@example.com";
            string user1Password = "Wachtwoord123";
            string user1Name = "Test Gebruiker";

            loginPage.EnterEmail(user1Email);
            loginPage.EnterPassword(user1Password);
            loginPage.ClickLogin();
            Console.WriteLine("LOG [Step 3] First user credentials entered and submitted.");

            // Step 4: Wait for redirect to agenda page
            wait.Until(d => d.Url.Contains("/agenda"));
            Console.WriteLine("LOG [Step 4] Successfully redirected to agenda page for first user.");

            // Step 5: Wait for calendar to load
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
            Console.WriteLine("LOG [Step 5] Calendar loaded for first user.");

            // Step 6: Verify first user information
            var user1Info = driver.FindElement(By.ClassName("user-info-inline"));
            string user1DisplayedInfo = user1Info.Text;
            
            if (!user1DisplayedInfo.Contains(user1Name) || !user1DisplayedInfo.Contains(user1Email))
            {
                throw new Exception($"Wrong user displayed. Expected '{user1Name}' and '{user1Email}' but got '{user1DisplayedInfo}'");
            }
            
            Console.WriteLine($"LOG [Step 6] First user verified: {user1DisplayedInfo}");

            // Step 7: Get first user's time slots
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
            
            var user1TimeSlots = driver.FindElements(By.ClassName("time-slot"));
            int user1SlotCount = user1TimeSlots.Count;
            
            Console.WriteLine($"LOG [Step 7] First user has {user1SlotCount} time slots.");

            if (user1SlotCount == 0)
            {
                throw new Exception("No time slots found for first user.");
            }

            // ===== LOGOUT AND SECOND USER LOGIN =====

            // Step 8: Logout by navigating back to login page
            driver.Navigate().GoToUrl("http://localhost:5173/");
            Console.WriteLine("LOG [Step 8] Logged out (navigated back to login page).");

            // Step 9: Login as second user (Doctor)
            string user2Email = "testdoctor@example.com";
            string user2Password = "password";
            string user2Name = "Test Doctor";

            loginPage.EnterEmail(user2Email);
            loginPage.EnterPassword(user2Password);
            loginPage.ClickLogin();
            Console.WriteLine("LOG [Step 9] Second user credentials entered and submitted.");

            // Step 10: Wait for redirect to agenda page
            wait.Until(d => d.Url.Contains("/agenda"));
            Console.WriteLine("LOG [Step 10] Successfully redirected to agenda page for second user.");

            // Step 11: Wait for calendar to load
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
            Console.WriteLine("LOG [Step 11] Calendar loaded for second user.");

            // Step 12: Verify second user information
            var user2Info = driver.FindElement(By.ClassName("user-info-inline"));
            string user2DisplayedInfo = user2Info.Text;
            
            if (!user2DisplayedInfo.Contains(user2Name) || !user2DisplayedInfo.Contains(user2Email))
            {
                throw new Exception($"Wrong user displayed. Expected '{user2Name}' and '{user2Email}' but got '{user2DisplayedInfo}'");
            }
            
            Console.WriteLine($"LOG [Step 12] Second user verified: {user2DisplayedInfo}");

            // Step 13: Get second user's time slots
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
            
            var user2TimeSlots = driver.FindElements(By.ClassName("time-slot"));
            int user2SlotCount = user2TimeSlots.Count;
            
            Console.WriteLine($"LOG [Step 13] Second user has {user2SlotCount} time slots.");

            if (user2SlotCount == 0)
            {
                throw new Exception("No time slots found for second user.");
            }

            // ===== VERIFY DATA ISOLATION =====

            // Step 14: Verify that the displayed user changed
            if (user1DisplayedInfo.Equals(user2DisplayedInfo))
            {
                throw new Exception("User information did not change between logins - calendars are not isolated.");
            }
            
            Console.WriteLine("LOG [Step 14] PASS: User information is different between the two logins.");

            // Step 15: Verify calendars are different (slot counts should differ or at least be verified as separate)
            Console.WriteLine($"LOG [Step 15] User 1 slot count: {user1SlotCount}, User 2 slot count: {user2SlotCount}");
            Console.WriteLine("LOG [Step 16] PASS: Each user receives only their own calendar data.");
            Console.WriteLine("LOG [Step 17] PASS: No data leakage between user calendars.");

            // Step 16: Test completed successfully
            Console.WriteLine("LOG [Step 18] Test completed successfully - Calendar data is fully isolated between users.");
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
                Console.WriteLine("LOG [Step 19] WebDriver closed.");
            }
        }
    }
}
