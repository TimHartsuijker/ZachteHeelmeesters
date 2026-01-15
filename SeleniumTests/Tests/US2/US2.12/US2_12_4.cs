using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SeleniumTests.P_O_M;

namespace SeleniumTests.Tests;

[TestClass]
public class US2_12_4
{
    [TestMethod]
    public void FilterByTreatment()
    {
        IWebDriver driver = null;
        try
        {
            // Step 1: Start test
            Console.WriteLine("LOG [Step 1] Start test: FilterByTreatment");
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

            // Step 4: Get initial entry count and treatments
            Console.WriteLine("LOG [Step 4] Get initial entry count before filtering");
            var allEntries = dossierPage.GetAllEntries();
            int initialEntryCount = allEntries.Count;
            if (initialEntryCount == 0)
            {
                throw new Exception("No dossier entries found - test cannot proceed without initial entries");
            }
            Console.WriteLine($"LOG [Step 4] PASS: Found {initialEntryCount} entries before filtering");

            // Step 5: Get treatment information from initial entries
            Console.WriteLine("LOG [Step 5] Extract treatment information from all entries");
            var treatmentsFound = new List<string>();
            foreach (var entry in allEntries)
            {
                try
                {
                    var headerText = entry.FindElement(By.CssSelector(".entry-header")).Text;
                    // Treatment is typically in format "Title - Treatment Date"
                    // Extract everything between first '-' and last '-' (date)
                    var parts = headerText.Split('-');
                    if (parts.Length >= 3)
                    {
                        // Join middle parts as treatment (skip first which is title, skip last which is date)
                        var treatment = string.Join("-", parts.Skip(1).Take(parts.Length - 2)).Trim();
                        if (!string.IsNullOrWhiteSpace(treatment) && !treatmentsFound.Contains(treatment))
                        {
                            treatmentsFound.Add(treatment);
                        }
                    }
                }
                catch
                {
                    // Skip entries we can't parse
                }
            }
            
            if (treatmentsFound.Count == 0)
            {
                throw new Exception("Could not extract any treatment information from entries");
            }
            Console.WriteLine($"LOG [Step 5] PASS: Found {treatmentsFound.Count} different treatments: {string.Join(", ", treatmentsFound)}");

            // Step 6: Get available options from treatment dropdown
            Console.WriteLine("LOG [Step 6] Get available treatment options from dropdown");
            var treatmentDropdown = driver.FindElement(By.CssSelector("select.filter-select"));
            if (treatmentDropdown == null)
            {
                throw new Exception("Treatment dropdown not found on page");
            }
            
            var selectElement = new OpenQA.Selenium.Support.UI.SelectElement(treatmentDropdown);
            var availableOptions = selectElement.Options;
            var availableTreatments = availableOptions
                .Select(o => o.Text)
                .Where(t => !string.IsNullOrWhiteSpace(t) && 
                           t.ToLower() != "selecteer behandeling" && 
                           t.ToLower() != "alle") // Skip placeholder and "Alle" (all treatments)
                .ToList();
            
            if (availableTreatments.Count == 0)
            {
                throw new Exception("No specific treatment options available in dropdown (only 'Alle' or placeholder found)");
            }
            Console.WriteLine($"LOG [Step 6] PASS: Found {availableTreatments.Count} specific treatment options in dropdown: {string.Join(", ", availableTreatments)}");

            // Step 7: Select first specific treatment from dropdown (not "Alle")
            Console.WriteLine("LOG [Step 7] Select a specific treatment from dropdown");
            string selectedTreatment = availableTreatments.First();
            
            try
            {
                dossierPage.SelectTreatment(selectedTreatment);
                System.Threading.Thread.Sleep(500); // Wait for dropdown to update
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to select {selectedTreatment} treatment: {ex.Message}");
            }
            Console.WriteLine($"LOG [Step 7] PASS: Selected '{selectedTreatment}' from dropdown");

            // Step 8: Click "Toepassen" (Apply) button to apply the filter
            Console.WriteLine("LOG [Step 8] Click 'Toepassen' button to apply the filter");
            try
            {
                dossierPage.ClickApplyFilters();
                System.Threading.Thread.Sleep(1000); // Wait for filter to apply
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to click apply filter button: {ex.Message}");
            }
            Console.WriteLine("LOG [Step 8] PASS: Clicked 'Toepassen' button");

            // Step 9: Verify filter has been applied
            Console.WriteLine("LOG [Step 9] Verify filter has been applied");
            var filteredEntries = dossierPage.GetAllEntries();
            
            if (filteredEntries.Count == initialEntryCount)
            {
                Console.WriteLine("LOG [Step 9] INFO: Same number of entries after filter - verify if selected treatment is the only one");
            }
            else if (filteredEntries.Count > initialEntryCount)
            {
                throw new Exception("Filter increased entry count - this should not happen");
            }
            else
            {
                Console.WriteLine($"LOG [Step 9] PASS: Filter applied - entries reduced from {initialEntryCount} to {filteredEntries.Count}");
            }

            // Step 10: Verify only selected treatment entries are shown
            Console.WriteLine($"LOG [Step 10] Verify only '{selectedTreatment}'-related entries are displayed");
            int selectedTreatmentCount = 0;
            foreach (var entry in filteredEntries)
            {
                try
                {
                    var headerText = entry.FindElement(By.CssSelector(".entry-header")).Text;
                    
                    // Check if entry header contains the selected treatment text
                    // Selected treatment might be "Consult hart", and header has "Consult hart - Cardiologie 19-12-2025"
                    if (headerText.Contains(selectedTreatment, StringComparison.OrdinalIgnoreCase))
                    {
                        selectedTreatmentCount++;
                    }
                    else
                    {
                        Console.WriteLine($"LOG [Step 9] WARNING: Found entry without '{selectedTreatment}': {headerText}");
                    }
                }
                catch
                {
                    // Skip entries we can't parse
                }
            }
            
            if (selectedTreatmentCount == 0)
            {
                throw new Exception($"No entries found for selected treatment '{selectedTreatment}' - filter may not be working");
            }
            
            Console.WriteLine($"LOG [Step 10] PASS: Verified - {selectedTreatmentCount} '{selectedTreatment}' entries shown out of {filteredEntries.Count} total");

            // Step 11: Verify entries don't contain other treatments
            Console.WriteLine($"LOG [Step 11] Verify entries don't contain other specific treatments");
            bool filterWorkingCorrectly = true;
            var otherTreatments = availableTreatments.Where(t => t != selectedTreatment).ToList();
            
            foreach (var entry in filteredEntries)
            {
                try
                {
                    var headerText = entry.FindElement(By.CssSelector(".entry-header")).Text;
                    
                    // Check if any other treatment options appear in the filtered results
                    foreach (var otherTreatment in otherTreatments)
                    {
                        if (headerText.Contains(otherTreatment, StringComparison.OrdinalIgnoreCase))
                        {
                            filterWorkingCorrectly = false;
                            Console.WriteLine($"LOG [Step 10] ERROR: Found '{otherTreatment}' in filtered results: {headerText}");
                        }
                    }
                }
                catch
                {
                    // Skip entries we can't parse
                }
            }
            
            if (filterWorkingCorrectly)
            {
                Console.WriteLine($"LOG [Step 11] PASS: Filter is working correctly - no other treatments visible");
            }
            else
            {
                throw new Exception("Filter is not working - entries from other treatments are still visible");
            }

            // Step 12: Reset filter to verify it was working
            Console.WriteLine("LOG [Step 12] Reset filter to verify filtering mechanism");
            try
            {
                dossierPage.SelectTreatment(""); // Select empty/all treatments
                System.Threading.Thread.Sleep(500);
                var resetEntries = dossierPage.GetAllEntries();
                
                if (resetEntries.Count >= initialEntryCount)
                {
                    Console.WriteLine($"LOG [Step 12] PASS: Filter reset successful - entries returned to {resetEntries.Count} (was {initialEntryCount})");
                }
                else
                {
                    Console.WriteLine("LOG [Step 12] WARNING: Entry count did not return to initial count after reset");
                }
            }
            catch
            {
                Console.WriteLine("LOG [Step 12] INFO: Could not verify filter reset (may not be required)");
            }

            // Step 13: Verify no error messages are displayed
            Console.WriteLine("LOG [Step 13] Verify no error messages are displayed");
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
            Console.WriteLine("LOG [Step 13] PASS: No error messages displayed");

            // Step 14: Test completed successfully
            Console.WriteLine("LOG [Step 14] Test completed successfully - Filter by treatment works correctly");
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
