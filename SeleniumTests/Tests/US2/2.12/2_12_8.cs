using OpenQA.Selenium;
using SeleniumTests.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace US2._12
{
    [TestClass]
    public class _2_12_8 : BaseTest
    {
        [TestMethod]
        public void TC_2_12_8_ReadOnlyAccessEnforcement()
        {
            // Stap 1: Login en Navigatie
            LogStep(1, "Logging in as patient and navigating to dossier...");
            helpers.LoginAndNavigateToDossier("gebruiker@example.com", "Wachtwoord123");

            Assert.IsTrue(helpers.IsOnDossierPage(), "Patient dossier page did not open.");
            LogSuccess(1, "Successfully reached the patient dossier page.");

            // Stap 2: Controleer op actieknoppen (Upload/Add)
            LogStep(2, "Verifying absence of upload and add buttons...");
            var uploadSelectors = "button[type='submit'], button.upload, input[type='file'], .upload-button, .btn-upload";
            var addSelectors = "button[title*='add'], button[title*='Add'], .btn-add, .add-button";

            var forbiddenButtons = driver.FindElements(By.CssSelector($"{uploadSelectors}, {addSelectors}"))
                                         .Where(b => b.Displayed && b.Enabled).ToList();

            Assert.IsFalse(forbiddenButtons.Any(), $"Action buttons found for patient: {string.Join(", ", forbiddenButtons.Select(b => b.Text))}");
            LogSuccess(2, "No upload or add buttons visible to patient.");

            // Stap 3: Controleer op bewerkingsfunctionaliteit (Edit/Delete)
            LogStep(3, "Verifying absence of edit and delete buttons...");
            var modificationSelectors = "button.delete, .btn-delete, button.edit, .btn-edit, [aria-label*='delete'], [aria-label*='edit']";

            var visibleModButtons = driver.FindElements(By.CssSelector(modificationSelectors))
                                          .Where(b => b.Displayed && b.Enabled).ToList();

            Assert.IsFalse(visibleModButtons.Any(), "Modification buttons (edit/delete) are visible to patient.");
            LogSuccess(3, "No edit or delete buttons visible to patient.");

            // Stap 4: Controleer op invoervelden (Forms)
            LogStep(4, "Scanning for unauthorized modification inputs...");
            var inputs = driver.FindElements(By.CssSelector("input[type='text']:not([readonly]), textarea:not([readonly])"))
                               .Where(i => i.Displayed && i.Enabled)
                               .Where(i => {
                                   var cls = i.GetAttribute("class")?.ToLower() ?? "";
                                   var ph = i.GetAttribute("placeholder")?.ToLower() ?? "";
                                   return !cls.Contains("search") && !cls.Contains("filter") && !ph.Contains("zoek");
                               }).ToList();

            Assert.IsFalse(inputs.Any(), "Editable input fields found (excluding search/filters).");
            LogSuccess(4, "No modification form inputs visible to patient.");

            // Stap 5: Navigatie-opties controleren
            LogStep(5, "Checking for doctor-specific navigation options...");
            var restrictedLinks = driver.FindElements(By.CssSelector("nav a, header a"))
                                        .Where(l => l.Displayed)
                                        .Where(l => {
                                            var href = l.GetAttribute("href")?.ToLower() ?? "";
                                            var text = l.Text.ToLower();
                                            return href.Contains("/doctor/") || text.Contains("upload");
                                        }).ToList();

            Assert.IsFalse(restrictedLinks.Any(), "Doctor-specific links found in navigation.");
            LogSuccess(5, "No privileged navigation options visible.");

            // Finale audit-log
            LogStep(6, "Final verification of read-only enforcement...");
            LogInfo("✓ No upload/add capabilities");
            LogInfo("✓ No edit/delete capabilities");
            LogInfo("✓ No data modification inputs");
            LogInfo("✓ UI strictly limited to patient scope");
            LogSuccess(6, "Read-only access properly enforced for patient role.");
        }
    }
}