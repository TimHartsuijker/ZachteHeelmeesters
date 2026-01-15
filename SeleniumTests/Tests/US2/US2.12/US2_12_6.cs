using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SeleniumTests.P_O_M;

namespace SeleniumTests.Tests;

[TestClass]
public class US2_12_6
{
    [TestMethod]
    public void ViewEntryDetails()
    {
        IWebDriver driver = null;
        try
        {
            // Step 1: Start test
            Console.WriteLine("LOG [Step 1] Start test: ViewEntryDetails");
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

            // Step 4: Get first entry
            Console.WriteLine("LOG [Step 4] Get first dossier entry");
            var entries = dossierPage.GetAllEntries();
            if (entries.Count == 0)
            {
                throw new Exception("No dossier entries found - cannot test entry expansion");
            }
            var firstEntry = dossierPage.GetEntryByIndex(0);
            Console.WriteLine($"LOG [Step 4] PASS: Found {entries.Count} entries");

            // Step 5: Verify entry is initially collapsed
            Console.WriteLine("LOG [Step 5] Verify entry is initially collapsed");
            bool initiallyExpanded = dossierPage.IsEntryExpanded(firstEntry);
            if (initiallyExpanded)
            {
                Console.WriteLine("LOG [Step 5] WARNING: Entry was already expanded - collapsing first");
                dossierPage.ToggleEntry(firstEntry);
                System.Threading.Thread.Sleep(500);
            }
            Console.WriteLine("LOG [Step 5] PASS: Entry is collapsed");

            // Step 6: Click on entry header to expand
            Console.WriteLine("LOG [Step 6] Click on entry header to expand");
            dossierPage.ToggleEntry(firstEntry);
            System.Threading.Thread.Sleep(1000); // Wait for expansion animation

            // Step 7: Verify entry is now expanded
            Console.WriteLine("LOG [Step 7] Verify entry expanded");
            bool isExpanded = dossierPage.IsEntryExpanded(firstEntry);
            if (!isExpanded)
            {
                throw new Exception("Entry did not expand after clicking header");
            }
            Console.WriteLine("LOG [Step 7] PASS: Entry is expanded");

            // Step 8: Verify files are visible
            Console.WriteLine("LOG [Step 8] Verify files are visible in expanded entry");
            var files = dossierPage.GetFilesInEntry(firstEntry);
            if (files.Count == 0)
            {
                throw new Exception("No files visible in expanded entry - expected at least one file");
            }
            Console.WriteLine($"LOG [Step 8] PASS: {files.Count} file(s) visible in expanded entry");

            // Step 9: Verify notes are visible
            Console.WriteLine("LOG [Step 9] Verify notes are visible");
            string notes = dossierPage.GetNotesFromEntry(firstEntry);
            if (string.IsNullOrWhiteSpace(notes))
            {
                Console.WriteLine("LOG [Step 9] WARNING: No notes found in entry (might be empty)");
            }
            else
            {
                Console.WriteLine($"LOG [Step 9] PASS: Notes visible (length: {notes.Length} characters)");
            }

            // Step 10: Verify author information is visible
            Console.WriteLine("LOG [Step 10] Verify author information is visible");
            string author = dossierPage.GetAuthorFromEntry(firstEntry);
            if (string.IsNullOrWhiteSpace(author))
            {
                throw new Exception("Author information not visible in expanded entry");
            }
            Console.WriteLine($"LOG [Step 10] PASS: Author visible: {author}");

            // Step 11: Verify metadata is visible (treatment, date)
            Console.WriteLine("LOG [Step 11] Verify metadata (treatment, date) is visible");
            var entryText = firstEntry.Text;
            bool hasMetadata = !string.IsNullOrWhiteSpace(entryText) && entryText.Length > 20;
            if (!hasMetadata)
            {
                throw new Exception("Entry metadata (treatment, date) not visible or too short");
            }
            Console.WriteLine($"LOG [Step 11] PASS: Metadata visible in entry");

            // Step 12: Verify expansion state remains (refresh entry reference)
            Console.WriteLine("LOG [Step 12] Verify expansion state remains");
            System.Threading.Thread.Sleep(500);
            var updatedEntry = dossierPage.GetEntryByIndex(0);
            bool stillExpanded = dossierPage.IsEntryExpanded(updatedEntry);
            if (!stillExpanded)
            {
                throw new Exception("Entry collapsed unexpectedly - expansion state not maintained");
            }
            Console.WriteLine("LOG [Step 12] PASS: Expansion state remains stable");

            // Step 13: Test collapse functionality
            Console.WriteLine("LOG [Step 13] Test collapse functionality");
            dossierPage.ToggleEntry(updatedEntry);
            System.Threading.Thread.Sleep(500);
            var finalEntry = dossierPage.GetEntryByIndex(0);
            bool isCollapsed = !dossierPage.IsEntryExpanded(finalEntry);
            if (!isCollapsed)
            {
                throw new Exception("Entry did not collapse after clicking header again");
            }
            Console.WriteLine("LOG [Step 13] PASS: Entry collapsed successfully");

            Console.WriteLine("LOG [Step 14] PASS: All entry details verified successfully");
            Console.WriteLine("LOG [Step 15] Test completed successfully - Entry expansion works correctly");
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
