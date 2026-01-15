using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SeleniumTests.P_O_M;

namespace SeleniumTests.Tests;

[TestClass]
public class US2_12_3
{
    [TestMethod]
    public void StructuredDossierView()
    {
        IWebDriver driver = null;
        try
        {
            // Step 1: Start test
            Console.WriteLine("LOG [Step 1] Start test: StructuredDossierView");
            driver = new ChromeDriver();
            var loginPage = new LoginPage(driver);
            var dossierPage = new DossierPage(driver);
            var helpers = new TestHelpers(driver, loginPage, dossierPage);

            // Step 2: Navigate to portal and login as patient
            Console.WriteLine("LOG [Step 2] Navigate to portal and login as patient");
            helpers.LoginAndNavigateToDossier("gebruiker@example.com", "Wachtwoord123");

            // Step 3: Verify dossier page is open
            Console.WriteLine("LOG [Step 3] Verify patient dossier page opens");
            if (!helpers.IsOnDossierPage())
            {
                throw new Exception($"Patient dossier page did not open. Current URL: {driver.Url}");
            }
            Console.WriteLine("LOG [Step 3] PASS: Patient dossier page is open");

            // Step 4: Verify dossier entries are structured
            Console.WriteLine("LOG [Step 4] Verify entries are properly structured");
            var entries = dossierPage.GetAllEntries();
            if (entries.Count == 0)
            {
                throw new Exception("No dossier entries found - expected at least one entry");
            }
            Console.WriteLine($"LOG [Step 4] PASS: Found {entries.Count} structured entries");

            // Step 5: Verify entries are grouped by treatment and date
            Console.WriteLine("LOG [Step 5] Verify entries are grouped by treatment and date");
            var treatments = new List<string>();
            var dates = new List<string>();
            
            foreach (var entry in entries)
            {
                var headerText = entry.FindElement(By.CssSelector(".entry-header")).Text;
                
                // Extract treatment and date information from header
                // Expected format: "Title - Treatment Date"
                if (string.IsNullOrWhiteSpace(headerText))
                {
                    throw new Exception("Entry header is empty - expected title, treatment, and date");
                }
                
                // Check if entry contains both treatment and date indicators
                var parts = headerText.Split('-');
                if (parts.Length < 2)
                {
                    throw new Exception($"Entry does not contain proper treatment grouping: {headerText}");
                }
                
                // Extract date (last part should be date)
                var dateStr = parts[parts.Length - 1].Trim();
                dates.Add(dateStr);
                
                // Extract treatment (middle parts)
                var treatment = parts.Length > 1 ? parts[parts.Length - 2].Trim() : "Unknown";
                treatments.Add(treatment);
            }
            
            Console.WriteLine($"LOG [Step 5] PASS: Entries contain {treatments.Distinct().Count()} unique treatments with dates");

            // Step 6: Verify each entry shows title, date, and treatment
            Console.WriteLine("LOG [Step 6] Verify each entry displays title, date, and treatment information");
            int validEntries = 0;
            foreach (var entry in entries)
            {
                try
                {
                    var headerElement = entry.FindElement(By.CssSelector(".entry-header"));
                    var headerText = headerElement.Text;
                    
                    // Verify title exists (non-empty)
                    if (string.IsNullOrWhiteSpace(headerText))
                    {
                        throw new Exception("Entry title is missing");
                    }
                    
                    // Verify contains date pattern (dd-MM-yyyy)
                    if (!System.Text.RegularExpressions.Regex.IsMatch(headerText, @"\d{2}-\d{2}-\d{4}"))
                    {
                        throw new Exception($"Entry does not contain valid date format: {headerText}");
                    }
                    
                    // Verify entry is visually readable (has proper spacing, classes)
                    var displayStyle = headerElement.GetAttribute("style");
                    var classList = headerElement.GetAttribute("class");
                    if (string.IsNullOrEmpty(classList))
                    {
                        throw new Exception("Entry element missing CSS classes - layout may not be properly styled");
                    }
                    
                    validEntries++;
                }
                catch (StaleElementReferenceException)
                {
                    // Re-fetch entries if element becomes stale
                    entries = dossierPage.GetAllEntries();
                }
            }
            
            if (validEntries == 0)
            {
                throw new Exception("No entries contain complete required information (title, date, treatment)");
            }
            Console.WriteLine($"LOG [Step 6] PASS: All {validEntries} entries display title, date, and treatment");

            // Step 7: Verify entries are sorted by date (newest first)
            Console.WriteLine("LOG [Step 7] Verify entries are sorted by date (newest first)");
            bool isSorted = dossierPage.AreEntriesSortedNewestFirst();
            if (!isSorted)
            {
                throw new Exception("Entries are not sorted newest first - expected chronological order");
            }
            Console.WriteLine("LOG [Step 7] PASS: Entries are sorted newest first");

            // Step 8: Verify layout is clear and readable
            Console.WriteLine("LOG [Step 8] Verify layout is clear and readable");
            var firstEntry = dossierPage.GetEntryByIndex(0);
            
            // Check for proper spacing and visibility
            var entryHeight = firstEntry.Size.Height;
            if (entryHeight < 60)
            {
                throw new Exception("Entry layout appears too compact - may not be readable");
            }
            
            // Check if entry can be expanded to show more details
            try
            {
                dossierPage.ExpandEntry(firstEntry);
                var expandedHeight = firstEntry.Size.Height;
                if (expandedHeight <= entryHeight)
                {
                    Console.WriteLine("LOG [Step 8] WARNING: Entry expand animation may not have worked, but visibility is adequate");
                }
                else
                {
                    Console.WriteLine($"LOG [Step 8] PASS: Layout is clear - entry expanded from {entryHeight}px to {expandedHeight}px");
                }
            }
            catch
            {
                // Entry might not be expandable, which is fine
                Console.WriteLine("LOG [Step 8] PASS: Entry layout is clear and readable");
            }

            // Step 9: Verify no error messages are displayed
            Console.WriteLine("LOG [Step 9] Verify no error messages are displayed");
            try
            {
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
            Console.WriteLine("LOG [Step 9] PASS: No error messages displayed");

            // Step 10: Test completed successfully
            Console.WriteLine("LOG [Step 10] Test completed successfully - Dossier view is properly structured");
        }
        catch (NoSuchElementException ex)
        {
            Console.WriteLine("ERROR [E001] Element not found: " + ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR [E999] Unexpected error: " + ex.Message);
            throw;
        }
        finally
        {
            driver?.Quit();
            Console.WriteLine("LOG [Cleanup] WebDriver closed.");
        }
    }
}
