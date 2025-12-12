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

            // Step 2: Navigate directly to Gebruikers page
            driver.Navigate().GoToUrl("http://localhost:5173/Gebruikers.html");
            Console.WriteLine("LOG [Step 2] Navigated to Gebruikers page.");

            // Wait for page to load
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(d => d.FindElements(By.ClassName("user-row")).Count > 0);
            Console.WriteLine("LOG [Step 3] Page loaded with users.");

            // Step 3: Find the first user's row in the overview
            var userRows = driver.FindElements(By.ClassName("user-row"));
            if (userRows.Count == 0)
            {
                throw new Exception("No users found on the page.");
            }
            var firstUserRow = userRows[0];
            Console.WriteLine("LOG [Step 4] Found first user row.");

            // Step 4: Change User role (select input in the user's row)
            var roleSelect = firstUserRow.FindElement(By.ClassName("role-select"));
            var selectElement = new SelectElement(roleSelect);
            string currentRole = selectElement.SelectedOption.Text;
            
            // Get all available roles and pick one different from current
            var availableRoles = selectElement.Options.Select(o => o.Text).ToList();
            string newRole = availableRoles.First(r => r != currentRole);
            selectElement.SelectByText(newRole);
            Console.WriteLine($"LOG [Step 5] Changed user role from '{currentRole}' to '{newRole}'.");

            // Step 5: Click on "Save"
            var saveButton = firstUserRow.FindElement(By.ClassName("save-btn"));
            saveButton.Click();
            Console.WriteLine("LOG [Step 6] Clicked 'Save' button for user.");

            // Wait for save to complete (alert appears)
            Thread.Sleep(1000);
            var alert = driver.SwitchTo().Alert();
            string alertText = alert.Text;
            alert.Accept();
            Console.WriteLine($"LOG [Step 7] Alert message: '{alertText}'.");

            // Step 6: Refresh the page to ensure data is loaded from the database
            driver.Navigate().Refresh();
            wait.Until(d => d.FindElements(By.ClassName("user-row")).Count > 0);
            Console.WriteLine("LOG [Step 8] Page refreshed to reload data from database.");

            // Step 7: Find the first user's row again and check the role value
            var userRowsAfter = driver.FindElements(By.ClassName("user-row"));
            var firstUserRowAfter = userRowsAfter[0];
            var roleSelectAfter = firstUserRowAfter.FindElement(By.ClassName("role-select"));
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

            // Step 8: Test completed
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
