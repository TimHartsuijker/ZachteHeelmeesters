using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace SeleniumTests;

[TestClass]
public class _3_7a_3
{
    [TestMethod]
    public void RemoveSystemManagerPermission_UserPermissionIsRemoved()
    {
        // Arrange
        IWebDriver driver = null;
        try
        {
            // Step 1: Start test
            Console.WriteLine("LOG [Step 1] Start test: RemoveSystemManagerPermission_UserPermissionIsRemoved");
            driver = new ChromeDriver();
            var loginPage = new SeleniumTests.Pages.LoginPage(driver);

            // Step 2: Navigate to the portal
            loginPage.Navigate();
            Console.WriteLine("LOG [Step 2] Navigated to portal.");

            // Step 3: Log in as system manager
            loginPage.EnterEmail("email@example.com");
            loginPage.EnterPassword("Test123!");
            loginPage.ClickLogin();
            Console.WriteLine("LOG [Step 3] Logged in as system manager.");

            // Step 4: Go to Gebruikers (Users) page
            driver.FindElement(By.LinkText("Gebruikers")).Click();
            Console.WriteLine("LOG [Step 4] Navigated to 'Gebruikers' page.");

            // Step 5: Find the user's div by ID and toggle the System Manager permission OFF
            var userDiv = driver.FindElement(By.Id("user-email@example.com"));
            Console.WriteLine("LOG [Step 5] Found user div for 'email@example.com'.");

            // Find the permission checkbox (assume input name="systemManager" or similar)
            var permissionCheckbox = userDiv.FindElement(By.Name("systemManager"));
            bool isChecked = permissionCheckbox.Selected;
            if (isChecked)
            {
                permissionCheckbox.Click();
                Console.WriteLine("LOG [Step 6] System Manager permission disabled (checkbox unchecked).");
            }
            else
            {
                Console.WriteLine("LOG [Step 6] System Manager permission was already disabled.");
            }

            // Step 7: Click Save button for this user
            var saveButton = userDiv.FindElement(By.XPath(".//button[contains(text(),'Save')]"));
            saveButton.Click();
            Console.WriteLine("LOG [Step 7] Clicked 'Save' button for user.");

            // Step 8: Refresh the page to ensure data is loaded from the database
            driver.Navigate().Refresh();
            Console.WriteLine("LOG [Step 8] Page refreshed to reload data from database.");

            // Step 9: Find the user's div again and check the permission value
            var userDivAfter = driver.FindElement(By.Id("user-email@example.com"));
            var permissionCheckboxAfter = userDivAfter.FindElement(By.Name("systemManager"));
            bool isCheckedAfter = permissionCheckboxAfter.Selected;
            if (!isCheckedAfter)
            {
                Console.WriteLine("LOG [Step 9] PASS: System Manager permission is disabled after refresh.");
            }
            else
            {
                Console.WriteLine("LOG [Step 9] FAIL: System Manager permission is STILL enabled after refresh.");
                throw new Exception("Permission not removed in database/UI after refresh.");
            }

            // Step 10: Test completed
            Console.WriteLine("LOG [Step 10] Test completed successfully.");
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