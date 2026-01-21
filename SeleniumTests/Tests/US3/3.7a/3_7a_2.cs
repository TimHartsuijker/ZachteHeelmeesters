using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace SeleniumTests;

[TestClass]
public class _3_7a_2
{
    [TestMethod]
    public void SaveWithoutChanges_ShowsNoChangesAlert()
    {
        // Arrange
        IWebDriver driver = null;
        try
        {
            Console.WriteLine("LOG [Step 1] Start test: SaveWithoutChanges_ShowsNoChangesAlert");
            driver = new ChromeDriver();

            // Step 2: Navigate directly to Gebruikers page
            driver.Navigate().GoToUrl("http://localhost/admin/users");
            Console.WriteLine("LOG [Step 2] Navigated to Gebruikers page.");

            // Wait for page to load
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(d => d.FindElements(By.ClassName("user-row")).Count > 0);
            Console.WriteLine("LOG [Step 3] Page loaded with users.");

            // Step 3: Find the first user's row
            var userRows = driver.FindElements(By.ClassName("user-row"));
            if (userRows.Count == 0)
            {
                throw new Exception("No users found on the page.");
            }
            var firstUserRow = userRows[0];
            Console.WriteLine("LOG [Step 4] Found first user row.");

            // Step 4: Click Save button without making changes
            var saveButton = firstUserRow.FindElement(By.ClassName("save-btn"));
            saveButton.Click();
            Console.WriteLine("LOG [Step 5] Clicked 'Save' button without changing any fields.");

            // Step 5: Wait for alert and verify message
            try
            {
                wait.Until(d => {
                    try
                    {
                        d.SwitchTo().Alert();
                        return true;
                    }
                    catch (NoAlertPresentException)
                    {
                        return false;
                    }
                });
                
                IAlert alert = driver.SwitchTo().Alert();
                string alertText = alert.Text;
                Console.WriteLine($"LOG [Step 6] Alert shown: '{alertText}'");
                
                if (alertText.Trim() == "Geen wijzigingen om op te slaan.")
                {
                    Console.WriteLine("LOG [Step 7] PASS: Correct alert message shown.");
                }
                else
                {
                    Console.WriteLine("LOG [Step 7] FAIL: Incorrect alert message.");
                    alert.Accept();
                    throw new Exception("Unexpected alert message: " + alertText);
                }
                alert.Accept();
                Console.WriteLine("LOG [Step 8] Alert accepted.");
            }
            catch (WebDriverTimeoutException)
            {
                Console.WriteLine("LOG [Step 6] FAIL: No alert appeared after clicking save without changes.");
                throw new Exception("Expected alert did not appear.");
            }

            // Step 6: Test completed
            Console.WriteLine("LOG [Step 9] Test completed successfully.");
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
                Console.WriteLine("LOG [Step 10] WebDriver closed.");
            }
        }
    }
}