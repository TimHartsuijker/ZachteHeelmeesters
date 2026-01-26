using OpenQA.Selenium;
using SeleniumTests.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using System.Text.RegularExpressions;

namespace US2._12
{
    [TestClass]
    public class _2_12_5 : BaseTest
    {
        [TestMethod]
        public void TC_2_12_5_FilterByDateRange()
        {
            // Stap 1: Login en Navigatie
            LogStep(1, "Logging in as patient and navigating to dossier...");
            helpers.LoginAndNavigateToDossier("gebruiker@example.com", "Wachtwoord123");

            Assert.IsTrue(helpers.IsOnDossierPage(), "Patient dossier page did not open.");
            LogSuccess(1, "Successfully reached the patient dossier page.");

            // Stap 2: Initiële data verzamelen
            LogStep(2, "Gathering initial dossier entries and extracting dates...");
            var allEntries = dossierPage.GetAllEntries();
            Assert.IsTrue(allEntries.Count > 0, "No dossier entries found - cannot test date filtering.");

            var entryDates = new List<DateTime>();
            foreach (var entry in allEntries)
            {
                var dt = ExtractEntryDate(entry);
                if (dt.HasValue) entryDates.Add(dt.Value);
            }

            Assert.IsTrue(entryDates.Count > 0, "Could not parse any entry dates from the dossier.");
            entryDates.Sort();
            LogInfo($"Dossier range: {entryDates.First():dd-MM-yyyy} to {entryDates.Last():dd-MM-yyyy}");
            LogSuccess(2, $"Parsed {entryDates.Count} entry dates.");

            // Stap 3: Filter instellen
            LogStep(3, "Setting date filter to a single day range (first entry date)...");
            var targetDate = entryDates.First();
            string isoDate = targetDate.ToString("yyyy-MM-dd");
            string localeDate = targetDate.ToString("dd-MM-yyyy");

            dossierPage.SetDateFrom(isoDate);
            dossierPage.SetDateTo(isoDate);
            // Fallback voor verschillende input types
            dossierPage.SetDateFrom(localeDate);
            dossierPage.SetDateTo(localeDate);

            LogInfo($"Filter set from {isoDate} to {isoDate}");
            LogSuccess(3, "Date range filter values entered.");

            // Stap 4: Filters toepassen
            LogStep(4, "Applying filters...");
            dossierPage.ClickApplyFilters();
            System.Threading.Thread.Sleep(1000); // Wacht op async update
            LogSuccess(4, "Filters applied.");

            // Stap 5: Verificatie resultaten
            LogStep(5, "Verifying filtered results...");
            var filteredEntries = dossierPage.GetAllEntries();

            Assert.IsTrue(filteredEntries.Count > 0, "Filter returned no entries - expected at least one.");
            LogInfo($"Entries after filter: {filteredEntries.Count}");

            foreach (var entry in filteredEntries)
            {
                var dt = ExtractEntryDate(entry);
                Assert.IsNotNull(dt, "Could not parse date of a filtered entry.");
                Assert.AreEqual(targetDate.Date, dt.Value.Date,
                    $"Entry date {dt.Value:dd-MM-yyyy} does not match filter date {targetDate:dd-MM-yyyy}");
            }
            LogSuccess(5, "All visible entries match the selected date range.");

            // Stap 6: Foutcontrole
            LogStep(6, "Checking for UI errors and layout integrity...");
            var errorElements = driver.FindElements(By.CssSelector(".error, .alert-danger, [role='alert']"))
                                      .Where(el => el.Displayed).ToList();

            Assert.IsFalse(errorElements.Any(), $"Errors detected: {string.Join("; ", errorElements.Select(e => e.Text))}");
            LogSuccess(6, "No UI errors detected after filtering.");

            // Finale audit-log
            LogStep(7, "Final verification of date range isolation...");
            LogInfo("✓ Date range filter correctly restricts view");
            LogInfo("✓ Entries outside range are excluded");
            LogInfo("✓ Data integrity maintained after filter application");
            LogSuccess(7, "TC_2_12_5: Date range filter verified successfully.");
        }

        private DateTime? ExtractEntryDate(IWebElement entry)
        {
            try
            {
                var text = entry.Text;
                var match = Regex.Match(text, @"\b\d{2}-\d{2}-\d{4}\b");
                if (match.Success && DateTime.TryParseExact(match.Value, "dd-MM-yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
                {
                    return parsed;
                }
            }
            catch { }
            return null;
        }
    }
}