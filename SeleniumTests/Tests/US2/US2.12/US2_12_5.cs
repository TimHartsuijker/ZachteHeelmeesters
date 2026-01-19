using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SeleniumTests.P_O_M;

namespace SeleniumTests.Tests;

[TestClass]
public class US2_12_5
{
    [TestMethod]
    public void FilterByDateRange()
    {
        IWebDriver driver = null;
        try
        {
            // Step 1: Start test
            Console.WriteLine("LOG [Step 1] Start test: FilterByDateRange");
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

            // Step 4: Gather initial entries and extract dates
            Console.WriteLine("LOG [Step 4] Gather initial dossier entries and extract dates");
            var allEntries = dossierPage.GetAllEntries();
            if (allEntries.Count == 0)
            {
                throw new Exception("No dossier entries found - cannot test date filtering");
            }

            // Helper to extract a date (with optional time) from an entry by expanding and scanning text
            DateTime? ExtractEntryDate(IWebElement entry, List<string> samples)
            {
                try
                {
                    dossierPage.ExpandEntry(entry); // make sure details are visible
                    System.Threading.Thread.Sleep(100);
                }
                catch { }

                // First try header helper
                var headerDateText = dossierPage.GetEntryDateText(entry);
                if (!string.IsNullOrWhiteSpace(headerDateText)) samples.Add(headerDateText);

                // Patterns: dd-MM-yyyy or dd-MM-yyyy HH:mm
                var patterns = new[] { "dd-MM-yyyy", "dd-MM-yyyy HH:mm" };

                foreach (var candidate in new[] { headerDateText, entry.Text })
                {
                    if (string.IsNullOrWhiteSpace(candidate)) continue;
                    samples.Add(candidate);

                    foreach (var pat in patterns)
                    {
                        if (DateTime.TryParseExact(candidate.Trim(), pat,
                            System.Globalization.CultureInfo.InvariantCulture,
                            System.Globalization.DateTimeStyles.None,
                            out var parsed))
                        {
                            return parsed;
                        }
                    }

                    // Regex search within the text
                    var match = System.Text.RegularExpressions.Regex.Match(candidate, @"\b\d{2}-\d{2}-\d{4}(\s\d{2}:\d{2})?\b");
                    if (match.Success)
                    {
                        var val = match.Value;
                        foreach (var pat in patterns)
                        {
                            if (DateTime.TryParseExact(val, pat,
                                System.Globalization.CultureInfo.InvariantCulture,
                                System.Globalization.DateTimeStyles.None,
                                out var parsed))
                            {
                                return parsed;
                            }
                        }
                    }
                }

                return null;
            }

            var entryDates = new List<DateTime>();
            var headerSamples = new List<string>();
            foreach (var entry in allEntries)
            {
                var dt = ExtractEntryDate(entry, headerSamples);
                if (dt.HasValue) entryDates.Add(dt.Value);
            }

            if (entryDates.Count == 0)
            {
                var sampleText = string.Join(" | ", headerSamples.Take(5));
                throw new Exception($"Could not parse any entry dates - samples: {sampleText}");
            }

            entryDates.Sort(); // oldest -> newest
            Console.WriteLine($"LOG [Step 4] PASS: Parsed {entryDates.Count} entry dates. Range {entryDates.First():dd-MM-yyyy} to {entryDates.Last():dd-MM-yyyy}");

            // Step 5: Use the exact date of the first entry so only that entry should remain
            var firstEntryDate = entryDates.First();
            DateTime fromDate = firstEntryDate;
            DateTime toDate = firstEntryDate;

            string fromStr = fromDate.ToString("yyyy-MM-dd");
            string toStr = toDate.ToString("yyyy-MM-dd");
            Console.WriteLine($"LOG [Step 5] Using single-day range on first entry date: {fromStr}");

            // Step 6: Set date range in filters (try both ISO and locale dd-MM-yyyy)
            Console.WriteLine("LOG [Step 6] Set From and To dates in filter");
            var fromDdMm = fromDate.ToString("dd-MM-yyyy");
            var toDdMm = toDate.ToString("dd-MM-yyyy");

            // Primary: ISO for input type="date"
            dossierPage.SetDateFrom(fromStr);
            dossierPage.SetDateTo(toStr);
            System.Threading.Thread.Sleep(200);

            // Secondary: send locale format in case the control expects dd-MM-yyyy
            dossierPage.SetDateFrom(fromDdMm);
            dossierPage.SetDateTo(toDdMm);
            System.Threading.Thread.Sleep(300);

            // Step 7: Click Apply filters
            Console.WriteLine("LOG [Step 7] Click 'Toepassen' button to apply date filter");
            dossierPage.ClickApplyFilters();
            System.Threading.Thread.Sleep(1000);

            // Step 8: Fetch filtered entries
            Console.WriteLine("LOG [Step 8] Fetch filtered entries after applying date range");
            var filteredEntries = dossierPage.GetAllEntries();
            if (filteredEntries.Count == 0)
            {
                throw new Exception("Filter returned no entries - expected the first entry to remain");
            }
            if (filteredEntries.Count != 1)
            {
                throw new Exception($"Filter returned {filteredEntries.Count} entries - expected exactly 1 (the first entry)");
            }

            Console.WriteLine("LOG [Step 8] PASS: Exactly 1 entry shown after filtering");

            // Step 9: Verify the single filtered entry is exactly the chosen date
            Console.WriteLine("LOG [Step 9] Verify filtered entry matches the selected date");
            {
                var entry = filteredEntries.First();
                var dt = ExtractEntryDate(entry, headerSamples);
                if (!dt.HasValue)
                {
                    throw new Exception("Could not parse entry date after filtering - entry text: " + entry.Text);
                }

                if (dt.Value.Date != fromDate.Date)
                {
                    throw new Exception($"Filtered entry date {dt.Value:dd-MM-yyyy} does not match selected date {fromDate:dd-MM-yyyy}");
                }
            }
            Console.WriteLine("LOG [Step 9] PASS: Filtered entry date matches selected date");

            // Step 10: Verify entries outside the range are excluded when possible
            Console.WriteLine("LOG [Step 10] Verify entries outside range are excluded (if initial set had wider range)");
            if (entryDates.First() < fromDate || entryDates.Last() > toDate)
            {
                if (filteredEntries.Count == allEntries.Count)
                {
                    throw new Exception("Filter did not reduce entries even though range was narrowed");
                }
                Console.WriteLine("LOG [Step 10] PASS: Entries outside the range are excluded");
            }
            else
            {
                Console.WriteLine("LOG [Step 10] INFO: Entire dataset already within selected range - exclusion not applicable");
            }

            // Step 11: Verify no error messages are displayed
            Console.WriteLine("LOG [Step 11] Verify no error messages are displayed");
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
            Console.WriteLine("LOG [Step 11] PASS: No error messages displayed");

            // Step 12: Test completed successfully
            Console.WriteLine("LOG [Step 12] Test completed successfully - Date range filter works correctly");
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
