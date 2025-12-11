using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace SeleniumTests;

[TestClass]
public class _3_7a_1
{
    [TestMethod]
    public void RoleChange_UserRoleIsChanged()
    {
        // Arrange
        IWebDriver driver = null;
        try
        {
            // Step 1: Start test
            Console.WriteLine("LOG [Step 1] Start test: RoleChange_UserRoleIsChanged");
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
            // WHEN TEST: Change selector (if needed)
            driver.FindElement(By.LinkText("Gebruikers")).Click();
            Console.WriteLine("LOG [Step 4] Navigated to 'Gebruikers' page.");

            // Step 5: Find the user's row in the overview and change the role directly
            // Find the row for the user by email
            // user rows are divs with IDs like "user-email@example.com", use that for selection
            var userDiv = driver.FindElement(By.Id("user-email@example.com"));
            Console.WriteLine("LOG [Step 5] Found user div for 'email@example.com'.");

            // Step 6: Change User role (select input in the user's row)
            var roleSelect = userDiv.FindElement(By.Name("role"));
            var selectElement = new SelectElement(roleSelect);
            string[] roles = { "Patiënt", "Huisarts", "Specialist", "Admin" };
            string currentRole = selectElement.SelectedOption.Text;
            string newRole = roles.First(r => r != currentRole);
            selectElement.SelectByText(newRole);
            Console.WriteLine($"LOG [Step 6] Changed user role from '{currentRole}' to '{newRole}'.");

            // Step 7: Click on "Save"
            // WHEN TEST: Replace with selector for save button!!
            // Only click the save button for this user, not other users
            var saveButton = userDiv.FindElement(By.XPath(".//button[contains(text(),'Save')]"));
            saveButton.Click();
            Console.WriteLine("LOG [Step 7] Clicked 'Save' button for user.");

            // Step 8: Refresh the page to ensure data is loaded from the database
            driver.Navigate().Refresh();
            Console.WriteLine("LOG [Step 8] Page refreshed to reload data from database.");

            // Step 9: Find the user's div again and check the role value
            var userDivAfter = driver.FindElement(By.Id("user-email@example.com"));
            var roleSelectAfter = userDivAfter.FindElement(By.Name("role"));
            var selectElementAfter = new SelectElement(roleSelectAfter);
            string selectedRoleAfter = selectElementAfter.SelectedOption.Text;
            if (selectedRoleAfter == newRole)
            {
                Console.WriteLine($"LOG [Step 9] PASS: Role in UI after refresh matches expected: '{newRole}'.");
            }
            else
            {
                Console.WriteLine($"LOG [Step 9] FAIL: Role in UI after refresh is '{selectedRoleAfter}', expected '{newRole}'.");
                throw new Exception("Role not updated in database/UI after refresh.");
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
                Console.WriteLine("LOG [Step 9] WebDriver closed.");
            }
        }
    }
}
