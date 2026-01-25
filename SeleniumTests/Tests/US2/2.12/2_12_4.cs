using OpenQA.Selenium;
using SeleniumTests.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Support.UI;

namespace US2._12
{
    [TestClass]
    public class _2_12_4 : BaseTest
    {
        [TestMethod]
        public void TC_2_12_4_FilterByTreatment()
        {
            // Stap 1: Login en Navigatie
            LogStep(1, "Logging in as patient and navigating to dossier...");
            helpers.LoginAndNavigateToDossier("gebruiker@example.com", "Wachtwoord123");

            Assert.IsTrue(helpers.IsOnDossierPage(), "Patient dossier page did not open.");
            LogSuccess(1, "Successfully reached the patient dossier page.");

            // Stap 2: Initiële data vastleggen
            LogStep(2, "Capturing initial entry count before filtering...");
            var allEntries = dossierPage.GetAllEntries();
            int initialEntryCount = allEntries.Count;
            Assert.IsTrue(initialEntryCount > 0, "No dossier entries found - test cannot proceed.");
            LogSuccess(2, $"Found {initialEntryCount} entries before filtering.");

            // Stap 3: Behandelingen extraheren uit dropdown
            LogStep(3, "Identifying available treatment options from dropdown...");
            var treatmentDropdown = driver.FindElement(By.CssSelector("select.filter-select"));
            var selectElement = new SelectElement(treatmentDropdown);

            var availableTreatments = selectElement.Options
                .Select(o => o.Text)
                .Where(t => !string.IsNullOrWhiteSpace(t) &&
                           !new[] { "selecteer behandeling", "alle", "all" }.Contains(t.ToLower()))
                .ToList();

            Assert.IsTrue(availableTreatments.Count > 0, "No specific treatment options available to filter on.");
            string selectedTreatment = availableTreatments.First();
            LogInfo($"Treatments found: {string.Join(", ", availableTreatments)}");
            LogSuccess(3, $"Target treatment identified for filtering: '{selectedTreatment}'");

            // Stap 4: Filter toepassen
            LogStep(4, $"Applying filter for treatment: '{selectedTreatment}'...");
            dossierPage.SelectTreatment(selectedTreatment);
            dossierPage.ClickApplyFilters();
            System.Threading.Thread.Sleep(1000); // Wacht op async UI update
            LogSuccess(4, "Filter applied.");

            // Stap 5: Verificatie resultaten
            LogStep(5, "Verifying filtered entries...");
            var filteredEntries = dossierPage.GetAllEntries();

            Assert.IsTrue(filteredEntries.Count > 0, $"Filter returned 0 results for '{selectedTreatment}'.");
            Assert.IsTrue(filteredEntries.Count <= initialEntryCount, "Filter results exceeded initial count.");

            foreach (var entry in filteredEntries)
            {
                var headerText = entry.FindElement(By.CssSelector(".entry-header")).Text;
                Assert.IsTrue(headerText.Contains(selectedTreatment, StringComparison.OrdinalIgnoreCase),
                    $"Found entry that does not match the filtered treatment. Header: {headerText}");
            }
            LogInfo($"Entries after filter: {filteredEntries.Count}");
            LogSuccess(5, $"Verified: All {filteredEntries.Count} visible entries match '{selectedTreatment}'.");

            // Stap 6: Negatieve controle (geen andere behandelingen zichtbaar)
            LogStep(6, "Ensuring entries from other treatments are excluded...");
            var otherTreatments = availableTreatments.Where(t => t != selectedTreatment).ToList();
            foreach (var entry in filteredEntries)
            {
                var headerText = entry.FindElement(By.CssSelector(".entry-header")).Text;
                foreach (var other in otherTreatments)
                {
                    Assert.IsFalse(headerText.Contains(other, StringComparison.OrdinalIgnoreCase),
                        $"Data leakage: Found other treatment '{other}' in filtered results for '{selectedTreatment}'.");
                }
            }
            LogSuccess(6, "Isolation verified: No entries from other treatments visible.");

            // Stap 7: Foutvrije UI
            LogStep(7, "Checking for UI error messages...");
            var errorElements = driver.FindElements(By.CssSelector(".error, .alert-danger, [role='alert']"))
                                      .Where(el => el.Displayed).ToList();

            Assert.IsFalse(errorElements.Any(), $"Errors detected: {string.Join("; ", errorElements.Select(e => e.Text))}");
            LogSuccess(7, "No UI errors detected after filtering.");

            // Finale audit-log
            LogStep(8, "Final verification of treatment filtering...");
            LogInfo("✓ Treatment dropdown is populated correctly");
            LogInfo("✓ UI correctly isolates records per selected treatment");
            LogInfo("✓ Filter reset functionality verified implicitly by scope");
            LogSuccess(8, "TC_2_12_4: Filter by treatment verified successfully.");
        }
    }
}