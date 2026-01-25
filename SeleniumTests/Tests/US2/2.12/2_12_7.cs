using OpenQA.Selenium;
using SeleniumTests.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace US2._12
{
    [TestClass]
    public class _2_12_7 : BaseTest
    {
        public _2_12_7()
        {
            useDownloadConfig = true;
        }

        [TestMethod]
        public void TC_2_12_7_DownloadMedicalFile()
        {
            const string USER_EMAIL = "gebruiker@example.com";
            const string USER_PASS = "Wachtwoord123";

            // Stap 1: Login en Navigatie
            LogStep(1, $"Logging in as patient ({USER_EMAIL}) and navigating to dossier...");
            helpers.LoginAndNavigateToDossier(USER_EMAIL, USER_PASS);

            Assert.IsTrue(helpers.IsOnDossierPage(), "Patient dossier page did not open.");
            LogSuccess(1, "Successfully reached the patient dossier page.");

            // Stap 2: Selecteer Dossier Entry
            LogStep(2, "Selecting the first dossier entry and expanding details...");
            var entries = dossierPage.GetAllEntries();
            Assert.IsTrue(entries.Count > 0, "No dossier entries found - cannot test file download.");

            var firstEntry = dossierPage.GetEntryByIndex(0);
            if (!dossierPage.IsEntryExpanded(firstEntry))
            {
                dossierPage.ToggleEntry(firstEntry);
                System.Threading.Thread.Sleep(1000); // Wacht op UI-animatie
            }
            LogSuccess(2, $"First entry expanded. Found {entries.Count} total entries.");

            // Stap 3: Bestand identificeren
            LogStep(3, "Locating files within the expanded entry...");
            var files = dossierPage.GetFilesInEntry(dossierPage.GetEntryByIndex(0));
            Assert.IsTrue(files.Count > 0, "No files found in expanded entry.");

            var firstFile = files.First();
            LogInfo($"Target file identified: {firstFile.Text}");
            LogSuccess(3, "Downloadable file found.");

            // Stap 4: Download uitvoeren
            LogStep(4, "Initiating file download...");
            int filesBefore = Directory.Exists(downloadDirectory) ? Directory.GetFiles(downloadDirectory).Length : 0;

            firstFile.Click();
            LogInfo("Download link clicked. Waiting for filesystem update...");

            // Stap 5: Wachten op voltooiing
            LogStep(5, "Waiting for download to complete (timeout: 10s)...");
            bool downloadCompleted = false;
            for (int i = 0; i < 20; i++) // 20 * 500ms = 10 sec
            {
                System.Threading.Thread.Sleep(500);
                var currentFiles = Directory.GetFiles(downloadDirectory)
                    .Where(f => !f.EndsWith(".crdownload") && !f.EndsWith(".tmp")).ToArray();

                if (currentFiles.Length > filesBefore)
                {
                    downloadCompleted = true;
                    LogInfo($"File appeared in directory after {(i + 1) * 500}ms.");
                    break;
                }
            }

            Assert.IsTrue(downloadCompleted, "Download did not complete within the timeout period.");
            LogSuccess(5, "File successfully detected on disk.");

            // Stap 6: Bestandsintegriteit controleren
            LogStep(6, "Verifying downloaded file integrity...");
            var downloadedFile = Directory.GetFiles(downloadDirectory)
                .OrderByDescending(f => File.GetCreationTime(f)).First();

            FileInfo fileInfo = new FileInfo(downloadedFile);
            LogInfo($"File name: {fileInfo.Name}");
            LogInfo($"File size: {fileInfo.Length} bytes");

            Assert.IsTrue(fileInfo.Length > 0, "Downloaded file is empty (0 bytes).");
            LogSuccess(6, "File integrity verified: Content is present.");

            // Stap 7: Foutcontrole op pagina
            LogStep(7, "Verifying no error alerts appeared during download...");
            var errors = driver.FindElements(By.CssSelector(".error, .alert-danger, [role='alert']"))
                               .Where(e => e.Displayed && !string.IsNullOrWhiteSpace(e.Text)).ToList();

            Assert.IsFalse(errors.Any(), $"UI errors detected: {string.Join(", ", errors.Select(e => e.Text))}");
            LogSuccess(7, "No UI errors detected during file acquisition.");

            // Finale audit-log
            LogStep(8, "Finalizing download test...");
            LogInfo("✓ File successfully acquired from server");
            LogInfo("✓ Local filesystem persistence verified");
            LogInfo("✓ No breaking UI states detected");
            LogSuccess(8, "File download functionality verified successfully.");
        }
    }
}