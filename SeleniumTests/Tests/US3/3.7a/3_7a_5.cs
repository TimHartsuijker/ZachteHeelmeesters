using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace SeleniumTests;

[TestClass]
public class _3_7a_5
{
    [TestMethod]
    public void UnauthorizedAccess_NonAdminCannotModifyRolesOrPermissions()
    {
        // Arrange
        IWebDriver driver = null;
        try
        {
            Console.WriteLine("LOG [Step 1] Start test: UnauthorizedAccess_NonAdminCannotModifyRolesOrPermissions");
            driver = new ChromeDriver();
            var loginPage = new SeleniumTests.Pages.LoginPage(driver);

            // Step 2: Navigate to the portal
            loginPage.Navigate();
            Console.WriteLine("LOG [Step 2] Navigated to portal.");

            // Step 3: Log in as non-admin user
            string nonAdminEmail = "user-email@example.com";
            loginPage.EnterEmail(nonAdminEmail);
            loginPage.EnterPassword("Test123!");
            loginPage.ClickLogin();
            Console.WriteLine("LOG [Step 3] Logged in as non-admin user.");

            // Step 4: Try to access "Gebruikers" page
            try
            {
                driver.FindElement(By.LinkText("Gebruikers")).Click();
                Console.WriteLine("LOG [Step 4] Attempted to navigate to 'Gebruikers' page.");
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("LOG [Step 4] 'Gebruikers' link not visible for non-admin user.");
            }

            // Step 4b: Try to access the user management page directly via URL
            try
            {
                driver.Navigate().GoToUrl("http://localhost:5173/gebruikers");
                Console.WriteLine("LOG [Step 4b] Attempted to access 'Gebruikers' page via direct URL.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("LOG [Step 4b] Error navigating directly to 'Gebruikers' page: " + ex.Message);
            }

            // Step 5: Check for access denied message or redirect
            bool accessDenied = false;
            bool redirectedToUserPage = false;
            try
            {
                var deniedMsg = driver.FindElement(By.XPath("//*[contains(text(),'Geen toegang')]"));
                if (deniedMsg.Displayed)
                {
                    accessDenied = true;
                    Console.WriteLine("LOG [Step 5] PASS: 'Geen toegang' message is shown.");
                }
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("LOG [Step 5] 'Geen toegang' message not found, checking for redirect.");
                // Check if redirected to user page (e.g. URL contains "/user" or similar)
                if (driver.Url.Contains("/user"))
                {
                    redirectedToUserPage = true;
                    Console.WriteLine("LOG [Step 5] PASS: Redirected to user page instead of accessing 'Gebruikers'.");
                }
            }

            // Step 6: Try to modify a role or permission (should not be possible)
            bool modificationPossible = false;
            try
            {
                var userDiv = driver.FindElement(By.Id("user-" + nonAdminEmail));
                var saveButton = userDiv.FindElement(By.XPath(".//button[contains(text(),'Save')]"));
                if (saveButton.Enabled)
                {
                    modificationPossible = true;
                    Console.WriteLine("LOG [Step 6] FAIL: Save button is enabled for non-admin user.");
                }
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("LOG [Step 6] Save button or user div not found for non-admin user (expected).");
            }

            // Step 7: Assert test outcome
            if ((accessDenied || redirectedToUserPage) && !modificationPossible)
            {
                Console.WriteLine("LOG [Step 7] PASS: Non-admin user cannot modify roles or permissions.");
            }
            else
            {
                Console.WriteLine("LOG [Step 7] FAIL: Unauthorized modification possible or no access denied/redirect.");
                throw new Exception("Unauthorized access not properly restricted.");
            }

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
                Console.WriteLine("LOG [Step 9] WebDriver closed.");
            }
        }
    }
}