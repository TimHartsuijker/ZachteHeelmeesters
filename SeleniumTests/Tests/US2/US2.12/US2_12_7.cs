using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SeleniumTests.P_O_M;

namespace SeleniumTests.Tests;

[TestClass]
public class US2_12_7
{
    private static string downloadDirectory = Path.Combine(Path.GetTempPath(), "SeleniumDownloads_" + Guid.NewGuid().ToString());

    [TestMethod]
    public void DownloadMedicalFile()
    {
        IWebDriver driver = null;
        try
        {
            // Step 1: Start test and create download directory
            Console.WriteLine("LOG [Step 1] Start test: DownloadMedicalFile");
            Directory.CreateDirectory(downloadDirectory);
            Console.WriteLine($"LOG [Step 1.1] Download directory created: {downloadDirectory}");

            // Step 2: Configure Chrome to download files automatically
            Console.WriteLine("LOG [Step 2] Configure Chrome for automatic downloads");
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddUserProfilePreference("download.default_directory", downloadDirectory);
            chromeOptions.AddUserProfilePreference("download.prompt_for_download", false);
            chromeOptions.AddUserProfilePreference("download.directory_upgrade", true);
            chromeOptions.AddUserProfilePreference("safebrowsing.enabled", true);
            
            driver = new ChromeDriver(chromeOptions);
            var loginPage = new LoginPage(driver);
            var dossierPage = new DossierPage(driver);
            var helpers = new TestHelpers(driver, loginPage, dossierPage);

            // Step 3: Navigate to portal and login
            Console.WriteLine("LOG [Step 3] Navigate to portal and login as patient");
            helpers.LoginAndNavigateToDossier("gebruiker@example.com", "Wachtwoord123");

            // Step 4: Verify dossier page is open
            Console.WriteLine("LOG [Step 4] Verify patient dossier page opens");
            if (!helpers.IsOnDossierPage())
            {
                throw new Exception($"Patient dossier page did not open. Current URL: {driver.Url}");
            }
            Console.WriteLine($"LOG [Step 4] PASS: Patient dossier page is open");

            // Step 5: Get first entry and expand it
            Console.WriteLine("LOG [Step 5] Get first dossier entry");
            var entries = dossierPage.GetAllEntries();
            if (entries.Count == 0)
            {
                throw new Exception("No dossier entries found - cannot test file download");
            }
            var firstEntry = dossierPage.GetEntryByIndex(0);
            Console.WriteLine($"LOG [Step 5] PASS: Found {entries.Count} entries");

            // Step 6: Expand entry if not already expanded
            Console.WriteLine("LOG [Step 6] Expand dossier entry");
            bool isExpanded = dossierPage.IsEntryExpanded(firstEntry);
            if (!isExpanded)
            {
                dossierPage.ToggleEntry(firstEntry);
                System.Threading.Thread.Sleep(1000); // Wait for expansion
                firstEntry = dossierPage.GetEntryByIndex(0); // Refresh reference
            }
            Console.WriteLine("LOG [Step 6] PASS: Entry is expanded");

            // Step 7: Get files in entry
            Console.WriteLine("LOG [Step 7] Get files in expanded entry");
            var files = dossierPage.GetFilesInEntry(firstEntry);
            if (files.Count == 0)
            {
                throw new Exception("No files found in expanded entry - cannot test download");
            }
            Console.WriteLine($"LOG [Step 7] PASS: Found {files.Count} file(s) in entry");

            // Step 8: Get file information before download
            Console.WriteLine("LOG [Step 8] Get file information");
            var firstFile = files.First();
            string fileText = firstFile.Text;
            Console.WriteLine($"LOG [Step 8] File card text: {fileText}");

            // Step 9: Count files before download
            int filesBeforeDownload = Directory.GetFiles(downloadDirectory).Length;
            Console.WriteLine($"LOG [Step 9] Files in download directory before: {filesBeforeDownload}");

            // Step 10: Click on file card to download
            Console.WriteLine("LOG [Step 10] Click on file card to download");
            firstFile.Click();
            Console.WriteLine("LOG [Step 10] Download initiated");

            // Step 11: Wait for download to complete (max 10 seconds)
            Console.WriteLine("LOG [Step 11] Wait for download to complete");
            bool downloadCompleted = false;
            int maxAttempts = 20; // 20 * 500ms = 10 seconds
            int attempt = 0;
            
            while (attempt < maxAttempts && !downloadCompleted)
            {
                System.Threading.Thread.Sleep(500);
                int currentFileCount = Directory.GetFiles(downloadDirectory).Length;
                
                // Check if a new file appeared (not a .crdownload temp file)
                var currentFiles = Directory.GetFiles(downloadDirectory)
                    .Where(f => !f.EndsWith(".crdownload") && !f.EndsWith(".tmp"))
                    .ToArray();
                
                if (currentFiles.Length > filesBeforeDownload)
                {
                    downloadCompleted = true;
                    Console.WriteLine($"LOG [Step 11] Download completed after {(attempt + 1) * 500}ms");
                }
                
                attempt++;
            }

            if (!downloadCompleted)
            {
                throw new Exception($"Download did not complete within 10 seconds. Files before: {filesBeforeDownload}, Files now: {Directory.GetFiles(downloadDirectory).Length}");
            }

            // Step 12: Verify file was downloaded
            Console.WriteLine("LOG [Step 12] Verify downloaded file");
            var downloadedFiles = Directory.GetFiles(downloadDirectory)
                .Where(f => !f.EndsWith(".crdownload") && !f.EndsWith(".tmp"))
                .ToArray();
            
            if (downloadedFiles.Length == 0)
            {
                throw new Exception("No file was downloaded");
            }

            string downloadedFilePath = downloadedFiles[0];
            string downloadedFileName = Path.GetFileName(downloadedFilePath);
            long fileSize = new FileInfo(downloadedFilePath).Length;
            
            Console.WriteLine($"LOG [Step 12] PASS: File downloaded: {downloadedFileName}");
            Console.WriteLine($"LOG [Step 12.1] File size: {fileSize} bytes");

            // Step 13: Verify file has content
            Console.WriteLine("LOG [Step 13] Verify file has content");
            if (fileSize == 0)
            {
                throw new Exception("Downloaded file is empty (0 bytes)");
            }
            Console.WriteLine($"LOG [Step 13] PASS: File has content ({fileSize} bytes)");

            // Step 14: Verify no error shown on page
            Console.WriteLine("LOG [Step 14] Verify no error shown on page");
            try
            {
                var errorElements = driver.FindElements(By.CssSelector(".error, .alert-danger, [role='alert']"));
                var visibleErrors = errorElements.Where(e => e.Displayed && !string.IsNullOrWhiteSpace(e.Text)).ToList();
                
                if (visibleErrors.Any())
                {
                    string errorText = string.Join(", ", visibleErrors.Select(e => e.Text));
                    throw new Exception($"Error shown on page: {errorText}");
                }
                Console.WriteLine("LOG [Step 14] PASS: No errors shown on page");
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("LOG [Step 14] PASS: No error elements found");
            }

            Console.WriteLine("LOG [Step 15] PASS: File download successful");
            Console.WriteLine($"LOG [Step 16] Downloaded file: {downloadedFileName} ({fileSize} bytes)");
            Console.WriteLine("LOG [Step 17] Test completed successfully - File download works correctly");
        }
        catch (NoSuchElementException ex)
        {
            Console.WriteLine("ERROR [E001] Element not found: " + ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR [E999] Unexpected error: " + ex.ToString());
            throw;
        }
        finally
        {
            if (driver != null)
            {
                driver.Quit();
                Console.WriteLine("LOG [Cleanup] WebDriver closed.");
            }

            // Cleanup: Delete download directory
            try
            {
                if (Directory.Exists(downloadDirectory))
                {
                    Directory.Delete(downloadDirectory, true);
                    Console.WriteLine("LOG [Cleanup] Download directory deleted.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LOG [Cleanup] Warning: Could not delete download directory: {ex.Message}");
            }
        }
    }
}
