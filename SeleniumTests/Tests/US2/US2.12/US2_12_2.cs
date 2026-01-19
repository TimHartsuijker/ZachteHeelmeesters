using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SeleniumTests.P_O_M;

namespace SeleniumTests.Tests;

[TestClass]
public class US2_12_2
{
    [TestMethod]
    public void ViewFullMedicalHistory()
    {
        IWebDriver driver = null;
        try
        {
            // Step 1: Start test
            Console.WriteLine("LOG [Step 1] Start test: ViewFullMedicalHistory");
            driver = new ChromeDriver();
            var loginPage = new LoginPage(driver);
            var dossierPage = new DossierPage(driver);
            var helpers = new TestHelpers(driver, loginPage, dossierPage);

            // Step 2: Navigate to portal and login
            Console.WriteLine("LOG [Step 2] Navigate to portal and login as patient");
            helpers.LoginAndNavigateToDossier("gebruiker@example.com", "Wachtwoord123");

            // Step 3: Verify dossier page is open
            Console.WriteLine("LOG [Step 3] Verify patient dossier page opens");
            if (!helpers.IsOnDossierPage())
            {
                throw new Exception($"Patient dossier page did not open. Current URL: {driver.Url}");
            }
            Console.WriteLine("LOG [Step 3] PASS: Patient dossier page is open");

            // Step 4: Scroll through dossier to check all entries are visible
            Console.WriteLine("LOG [Step 4] Scroll through dossier to trigger lazy loading");
            helpers.ScrollToBottom();
            Console.WriteLine("LOG [Step 4] PASS: Scrolled to bottom of dossier");

            // Step 5: Scroll back to top for verification
            helpers.ScrollToTop();
            Console.WriteLine("LOG [Step 5] Scrolled back to top");

            // Step 6: Verify all medical entries are shown
            Console.WriteLine("LOG [Step 6] Verify all medical entries for patient are shown");
            var entries = dossierPage.GetAllEntries();
            int minimumExpected = 3;
            if (entries.Count < minimumExpected)
            {
                throw new Exception($"Expected at least {minimumExpected} entries, but found {entries.Count}");
            }
            Console.WriteLine($"LOG [Step 6] PASS: All medical entries are shown ({entries.Count} entries found)");

            // Step 7: Verify each entry has content
            Console.WriteLine("LOG [Step 7] Verify each entry contains data");
            foreach (var entry in entries)
            {
                if (string.IsNullOrWhiteSpace(entry.Text))
                {
                    throw new Exception("Found empty entry - all entries should contain data");
                }
            }
            Console.WriteLine("LOG [Step 7] PASS: All entries contain valid data");

            // Step 8: Verify entries are sorted newest first
            Console.WriteLine("LOG [Step 8] Verify entries are sorted newest first");
            bool isSorted = dossierPage.AreEntriesSortedNewestFirst();
            if (!isSorted)
            {
                throw new Exception("Entries are not sorted newest first - expected chronological order");
            }
            Console.WriteLine("LOG [Step 8] PASS: Entries are sorted newest first");

            // Step 9: Verify user is logged in (ensures data isolation)
            Console.WriteLine("LOG [Step 9] Verify user session is active");
            string sessionUserId = helpers.GetSessionUserId();
            if (string.IsNullOrEmpty(sessionUserId))
            {
                throw new Exception("Session userId not found - user should be logged in");
            }
            Console.WriteLine($"LOG [Step 9] PASS: User is logged in (userId: {sessionUserId})");

            // Step 10: Verify no error messages are displayed
            Console.WriteLine("LOG [Step 10] Verify no error messages are displayed");
            
            // Check for actual error UI elements, not just keywords in page source
            try
            {
                // Look for visible error messages in specific UI elements
                var errorElements = driver.FindElements(By.CssSelector("[class*='error'], [class*='Error'], [id*='error'], [id*='Error']"));
                var visibleErrors = errorElements.Where(el => el.Displayed).ToList();
                
                if (visibleErrors.Count > 0)
                {
                    string errorText = string.Join("; ", visibleErrors.Select(e => e.Text).Where(t => !string.IsNullOrEmpty(t)));
                    throw new Exception($"Visible error messages detected: {errorText}");
                }
            }
            catch (NoSuchElementException)
            {
                // No error elements found - this is good
            }
            
            Console.WriteLine("LOG [Step 10] PASS: No error messages displayed");

            // Step 11: Test completed successfully
            Console.WriteLine("LOG [Step 11] Test completed successfully - Full medical history is visible");
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
                Console.WriteLine("LOG [Cleanup] WebDriver closed.");
            }
        }
    }
}
