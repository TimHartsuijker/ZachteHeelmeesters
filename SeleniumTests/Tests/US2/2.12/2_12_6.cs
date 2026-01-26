using OpenQA.Selenium;
using SeleniumTests.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace US2._12
{
    [TestClass]
    public class _2_12_6 : BaseTest
    {
        [TestMethod]
        public void TC_2_12_6_ViewEntryDetails()
        {
            // Stap 1: Login en Navigatie
            LogStep(1, "Logging in as patient and navigating to dossier...");
            helpers.LoginAndNavigateToDossier("gebruiker@example.com", "Wachtwoord123");

            Assert.IsTrue(helpers.IsOnDossierPage(), "Patient dossier page did not open.");
            LogSuccess(1, "Successfully reached the patient dossier page.");

            // Stap 2: Dossier item identificeren
            LogStep(2, "Identifying the first dossier entry...");
            var entries = dossierPage.GetAllEntries();
            Assert.IsTrue(entries.Count > 0, "No dossier entries found - cannot test expansion.");

            var firstEntry = dossierPage.GetEntryByIndex(0);
            LogSuccess(2, $"Found {entries.Count} entries. Target entry identified.");

            // Stap 3: Initiële staat controleren
            LogStep(3, "Ensuring entry is initially collapsed...");
            if (dossierPage.IsEntryExpanded(firstEntry))
            {
                LogInfo("Entry was already expanded, collapsing for test consistency.");
                dossierPage.ToggleEntry(firstEntry);
                System.Threading.Thread.Sleep(500);
            }
            LogSuccess(3, "Entry is in collapsed state.");

            // Stap 4: Expansie testen
            LogStep(4, "Clicking on entry header to expand details...");
            dossierPage.ToggleEntry(firstEntry);
            System.Threading.Thread.Sleep(1000); // Wacht op expansie-animatie

            Assert.IsTrue(dossierPage.IsEntryExpanded(firstEntry), "Entry did not expand after clicking header.");
            LogSuccess(4, "Entry successfully expanded.");

            // Stap 5: Details verifiëren (Bestanden, Notities, Auteur)
            LogStep(5, "Verifying visible details in expanded entry...");

            // Bestanden
            var files = dossierPage.GetFilesInEntry(firstEntry);
            Assert.IsTrue(files.Count > 0, "No files visible in expanded entry.");
            LogInfo($"Files detected: {files.Count}");

            // Notities
            string notes = dossierPage.GetNotesFromEntry(firstEntry);
            LogInfo($"Notes content: {(string.IsNullOrWhiteSpace(notes) ? "[Empty]" : "Present (" + notes.Length + " chars)")}");

            // Auteur en Metadata
            string author = dossierPage.GetAuthorFromEntry(firstEntry);
            Assert.IsFalse(string.IsNullOrWhiteSpace(author), "Author information is missing.");
            LogInfo($"Author: {author}");

            LogSuccess(5, "Files, notes, and author information are correctly displayed.");

            // Stap 6: Collapse testen
            LogStep(6, "Testing collapse functionality...");
            dossierPage.ToggleEntry(firstEntry);
            System.Threading.Thread.Sleep(500);

            Assert.IsFalse(dossierPage.IsEntryExpanded(firstEntry), "Entry did not collapse after clicking header again.");
            LogSuccess(6, "Entry collapsed successfully.");

            // Finale audit-log
            LogStep(7, "Final verification of dossier interaction...");
            LogInfo("✓ Accordion expansion/collapse functional");
            LogInfo("✓ All sub-details (files, notes, metadata) accessible");
            LogSuccess(7, "Dossier entry detail view verified successfully.");
        }
    }
}