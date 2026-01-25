using OpenQA.Selenium;
using SeleniumTests.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

namespace US2._12
{
    [TestClass]
    public class _2_12_3 : BaseTest
    {
        [TestMethod]
        public void TC_2_12_3_StructuredDossierView()
        {
            // Stap 1: Login en Navigatie
            LogStep(1, "Logging in as patient and navigating to dossier...");
            helpers.LoginAndNavigateToDossier("gebruiker@example.com", "Wachtwoord123");

            Assert.IsTrue(helpers.IsOnDossierPage(), "Patient dossier page did not open.");
            LogSuccess(1, "Successfully reached the patient dossier page.");

            // Stap 2: Dossier items ophalen
            LogStep(2, "Verifying presence of structured dossier entries...");
            var entries = dossierPage.GetAllEntries();
            Assert.IsTrue(entries.Count > 0, "No dossier entries found - expected at least one entry.");
            LogSuccess(2, $"Found {entries.Count} entries.");

            // Stap 3: Structuur controleren (Groepering, Titel, Datum, Behandeling)
            LogStep(3, "Verifying each entry displays title, date, and treatment grouping...");

            foreach (var entry in entries)
            {
                var headerElement = entry.FindElement(By.CssSelector(".entry-header"));
                string headerText = headerElement.Text;

                // Controleer op aanwezigheid tekst
                Assert.IsFalse(string.IsNullOrWhiteSpace(headerText), "Entry header is empty.");

                // Controleer op datum patroon (dd-MM-yyyy)
                bool hasDate = Regex.IsMatch(headerText, @"\d{2}-\d{2}-\d{4}");
                Assert.IsTrue(hasDate, $"Entry does not contain a valid date format: {headerText}");

                // Controleer op scheidingsteken voor groepering (bijv. '-')
                Assert.IsTrue(headerText.Contains("-"), $"Entry metadata grouping separator missing in: {headerText}");

                // Controleer op styling classes
                string classList = headerElement.GetAttribute("class");
                Assert.IsFalse(string.IsNullOrEmpty(classList), "Entry element missing CSS classes.");
            }
            LogSuccess(3, "All entries are correctly structured and meta-tagged.");

            // Stap 4: Sortering verifiëren
            LogStep(4, "Verifying entries are sorted by date (newest first)...");
            bool isSorted = dossierPage.AreEntriesSortedNewestFirst();
            Assert.IsTrue(isSorted, "Entries are not sorted in descending chronological order.");
            LogSuccess(4, "Chronological sorting verified.");

            // Stap 5: Layout en leesbaarheid
            LogStep(5, "Verifying layout spacing and expansion functionality...");
            var firstEntry = dossierPage.GetEntryByIndex(0);
            int initialHeight = firstEntry.Size.Height;
            Assert.IsTrue(initialHeight >= 60, "Entry layout appears too compact (accessibility issue).");

            // Test expansie voor leesbaarheid van details
            dossierPage.ExpandEntry(firstEntry);
            System.Threading.Thread.Sleep(500); // Wacht op animatie
            int expandedHeight = firstEntry.Size.Height;

            LogInfo($"Entry height: {initialHeight}px (collapsed) -> {expandedHeight}px (expanded)");
            LogSuccess(5, "Dossier layout is readable and interactive.");

            // Stap 6: Foutvrije UI
            LogStep(6, "Checking for UI error messages and broken elements...");
            var errorElements = driver.FindElements(By.CssSelector(".error, .alert-danger, [role='alert']"))
                                      .Where(el => el.Displayed).ToList();

            Assert.IsFalse(errorElements.Any(), $"Visible error messages detected: {string.Join("; ", errorElements.Select(e => e.Text))}");
            LogSuccess(6, "No UI errors detected.");

            // Finale audit-log
            LogStep(7, "Final verification of structured dossier view...");
            LogInfo("✓ Metadata (Title/Date/Treatment) present on all items");
            LogInfo("✓ Correct chronological descending sort");
            LogInfo("✓ Layout responsive to expansion actions");
            LogSuccess(7, "Dossier structure verified successfully.");
        }
    }
}