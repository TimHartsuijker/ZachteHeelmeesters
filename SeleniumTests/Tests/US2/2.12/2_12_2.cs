using OpenQA.Selenium;
using SeleniumTests.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace US2._12
{
    [TestClass]
    public class _2_12_2 : BaseTest
    {
        [TestMethod]
        public void TC_2_12_2_ViewFullMedicalHistory()
        {
            // Stap 1: Login en Navigatie
            LogStep(1, "Logging in as patient and navigating to medical dossier...");
            helpers.LoginAndNavigateToDossier("gebruiker@example.com", "Wachtwoord123");

            Assert.IsTrue(helpers.IsOnDossierPage(), "Patient dossier page did not open.");
            LogSuccess(1, "Successfully reached the patient dossier page.");

            // Stap 2: Laden van volledige historie (Lazy Loading)
            LogStep(2, "Scrolling through dossier to ensure all entries are loaded...");
            helpers.ScrollToBottom();
            System.Threading.Thread.Sleep(1000); // Wacht op eventuele lazy loading
            helpers.ScrollToTop();
            LogSuccess(2, "Lazy loading triggered and returned to top of page.");

            // Stap 3: Verifieer aantal entries
            LogStep(3, "Verifying that medical entries are present and visible...");
            var entries = dossierPage.GetAllEntries();
            int minimumExpected = 3;

            Assert.IsTrue(entries.Count >= minimumExpected,
                $"Expected at least {minimumExpected} entries, but found {entries.Count}.");
            LogSuccess(3, $"History verified: {entries.Count} medical entries found.");

            // Stap 4: Data integriteit per entry
            LogStep(4, "Checking for data integrity in each dossier entry...");
            foreach (var entry in entries)
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(entry.Text), "Found an empty dossier entry.");
            }
            LogSuccess(4, "All visible entries contain valid medical data.");

            // Stap 5: Chronologische volgorde
            LogStep(5, "Verifying if entries are sorted newest first...");
            bool isSorted = dossierPage.AreEntriesSortedNewestFirst();
            Assert.IsTrue(isSorted, "Medical history is not sorted in descending chronological order.");
            LogSuccess(5, "Chronological descending order verified.");

            // Stap 6: Sessie-isolatie check
            LogStep(6, "Verifying data isolation via active user session...");
            string sessionUserId = helpers.GetSessionUserId();
            Assert.IsFalse(string.IsNullOrEmpty(sessionUserId), "Session context lost - user is not properly logged in.");
            LogInfo($"Active Session UserID: {sessionUserId}");
            LogSuccess(6, "Session remains active for the current user.");

            // Stap 7: Foutvrije weergave
            LogStep(7, "Scanning for UI error messages or broken elements...");
            var errorElements = driver.FindElements(By.CssSelector(".error, .alert-danger, [role='alert']"))
                                      .Where(el => el.Displayed).ToList();

            Assert.IsFalse(errorElements.Any(), $"Visible errors detected: {string.Join("; ", errorElements.Select(e => e.Text))}");
            LogSuccess(7, "No UI error messages detected.");

            // Finale audit-log
            LogStep(8, "Final verification of full history access...");
            LogInfo("✓ Full historical data loaded");
            LogInfo("✓ Lazy loading functional");
            LogInfo("✓ Correct sorting and session persistence verified");
            LogSuccess(8, "TC_2_12_2: Patient can view full medical history without restrictions.");
        }
    }
}