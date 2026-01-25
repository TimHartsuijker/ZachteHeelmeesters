using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SeleniumTests.P_O_M;

namespace SeleniumTests.Tests;

[TestClass]
public class US2_12_1
{
    [TestMethod]
    public void OpenMedicalDossier()
    {
        IWebDriver driver = null;
        try
        {
            // Step 1: Start test
            Console.WriteLine("LOG [Step 1] Start test: OpenMedicalDossier");
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
            Console.WriteLine($"LOG [Step 3] PASS: Patient dossier page is open (URL: {driver.Url})");

            // Step 4: Verify dossier entries are loaded
            Console.WriteLine("LOG [Step 4] Verify dossier entries are loaded for logged-in patient");
            var entries = dossierPage.GetAllEntries();
            if (entries.Count == 0)
            {
                throw new Exception("No dossier entries found - expected at least one entry");
            }
            Console.WriteLine($"LOG [Step 4] PASS: Dossier entries loaded ({entries.Count} entries found)");

            // Step 5: Verify loading spinner disappeared
            Console.WriteLine("LOG [Step 5] Verify loading spinner has disappeared");
            if (helpers.IsLoadingSpinnerPresent())
            {
                throw new Exception("Loading spinner is still present - expected to have disappeared");
            }
            Console.WriteLine("LOG [Step 5] PASS: Loading spinner disappeared after data load");

            // Step 6: Test completed successfully
            Console.WriteLine("LOG [Step 6] Test completed successfully - Medical dossier opened and entries loaded");
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
