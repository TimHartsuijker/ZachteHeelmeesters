using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace SeleniumTests;

[TestClass]
public class _3_7a_4
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
            var loginPage = new SeleniumTests.Pages.LoginPage(driver);

            // Step 2: Navigate to the portal
            loginPage.Navigate();
            Console.WriteLine("LOG [Step 2] Navigated to portal.");

            // Step 3: Log in as system manager
            string systemManagerEmail = "email@example.com";
            loginPage.EnterEmail(systemManagerEmail);
            loginPage.EnterPassword("Test123!");
            loginPage.ClickLogin();
            Console.WriteLine("LOG [Step 3] Logged in as system manager.");

            // Step 4: Go to Gebruikers (Users) page
            driver.FindElement(By.LinkText("Gebruikers")).Click();
            Console.WriteLine("LOG [Step 4] Navigated to 'Gebruikers' page.");

            // Step 5: Find the system manager's own user div by ID
            var userDiv = driver.FindElement(By.Id(systemManagerEmail));
            Console.WriteLine($"LOG [Step 5] Found user div for system manager '{systemManagerEmail}'.");

            // Step 6: Click Save button without making changes
            var saveButton = userDiv.FindElement(By.XPath(".//button[contains(text(),'Save')]"));
            saveButton.Click();
            Console.WriteLine("LOG [Step 6] Clicked 'Save' button without changing any fields.");

            // Step 7: Handle alert and verify message
            IAlert alert = driver.SwitchTo().Alert();
            string alertText = alert.Text;
            Console.WriteLine($"LOG [Step 7] Alert shown: '{alertText}'");
            if (alertText.Trim() == "Geen wijzigingen om op te slaan.")
            {
                Console.WriteLine("LOG [Step 8] PASS: Correct alert message shown.");
            }
            else
            {
                Console.WriteLine("LOG [Step 8] FAIL: Incorrect alert message.");
                throw new Exception("Unexpected alert message: " + alertText);
            }
            alert.Accept();
            Console.WriteLine("LOG [Step 9] Alert accepted.");

            // Step 10: Test completed
            Console.WriteLine("LOG [Step 10] Test completed successfully.");
        }
        catch (NoSuchElementException ex)
        {
            Console.WriteLine("ERROR [E001] Element not found: " + ex.Message);
            throw;
        }
        catch (NoAlertPresentException ex)
        {
            Console.WriteLine("ERROR [E002] No alert present: " + ex.Message);
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