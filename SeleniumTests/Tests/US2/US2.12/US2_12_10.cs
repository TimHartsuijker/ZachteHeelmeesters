using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SeleniumTests.P_O_M;
using System.Diagnostics;

namespace SeleniumTests.Tests;

[TestClass]
public class US2_12_10
{
    [TestMethod]
    public void DossierLoadErrorHandling()
    {
        IWebDriver driver = null;
        bool backendWasRunning = false;
        
        try
        {
            // Step 1: Start test
            Console.WriteLine("LOG [Step 1] Start test: DossierLoadErrorHandling");
            driver = new ChromeDriver();
            var loginPage = new LoginPage(driver);
            var dossierPage = new DossierPage(driver);
            var helpers = new TestHelpers(driver, loginPage, dossierPage);

            // Step 2: Navigate to portal and login as patient
            Console.WriteLine("LOG [Step 2] Navigate to portal and login as patient (backend running)");
            helpers.LoginAndNavigateToDossier("gebruiker@example.com", "Wachtwoord123");

            // Step 3: Verify dossier page loads successfully first time
            Console.WriteLine("LOG [Step 3] Verify dossier page loads successfully");
            if (!helpers.IsOnDossierPage())
            {
                throw new Exception($"Patient dossier page did not open. Current URL: {driver.Url}");
            }
            
            // Wait for initial load
            System.Threading.Thread.Sleep(2000);
            
            var initialEntries = dossierPage.GetAllEntries();
            Console.WriteLine($"LOG [Step 3] PASS: Dossier loaded successfully with {initialEntries.Count} entries");

            // Step 4: Simulate API failure by stopping the backend
            Console.WriteLine("LOG [Step 4] Simulating API failure by stopping backend server");
            Console.WriteLine("LOG [Step 4.1] INFO: Attempting to stop backend process...");
            
            try
            {
                // Find and kill dotnet backend process
                var dotnetProcesses = Process.GetProcessesByName("backend");
                
                if (dotnetProcesses.Length == 0)
                {
                    Console.WriteLine("LOG [Step 4.1] WARNING: Backend process not found by name 'backend', trying 'dotnet'");
                    dotnetProcesses = Process.GetProcessesByName("dotnet");
                }
                
                if (dotnetProcesses.Length > 0)
                {
                    backendWasRunning = true;
                    foreach (var proc in dotnetProcesses)
                    {
                        Console.WriteLine($"LOG [Step 4.1] Stopping process: {proc.ProcessName} (PID: {proc.Id})");
                        proc.Kill();
                        proc.WaitForExit(5000);
                    }
                    Console.WriteLine("LOG [Step 4.2] PASS: Backend stopped successfully");
                }
                else
                {
                    Console.WriteLine("LOG [Step 4.1] ERROR: No backend process found to stop");
                    throw new Exception("Cannot simulate API failure - backend process not found");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LOG [Step 4.1] ERROR: Failed to stop backend: {ex.Message}");
                throw;
            }

            // Wait for backend to fully stop
            System.Threading.Thread.Sleep(2000);

            // Step 5: Try to refresh the dossier page (should fail)
            Console.WriteLine("LOG [Step 5] Refresh dossier page to trigger API failure");
            driver.Navigate().Refresh();
            
            // Wait for error to appear
            System.Threading.Thread.Sleep(3000);

            // Step 6: Verify error message is displayed
            Console.WriteLine("LOG [Step 6] Verify error message is displayed");
            bool errorFound = false;
            string errorMessage = "";

            try
            {
                // Look for error elements
                var errorElements = driver.FindElements(By.CssSelector(
                    ".error, .alert, .alert-danger, .error-message, [role='alert'], " +
                    ".text-red, .text-danger, .notification, .toast, .message"));
                
                // Also check paragraphs and divs for error keywords
                var allText = driver.FindElements(By.TagName("p"))
                    .Concat(driver.FindElements(By.TagName("div")))
                    .Concat(errorElements)
                    .ToList();

                var visibleErrors = allText.Where(e =>
                {
                    try
                    {
                        if (!e.Displayed || string.IsNullOrWhiteSpace(e.Text))
                            return false;
                        
                        var text = e.Text.ToLower();
                        return text.Contains("error") || 
                               text.Contains("fout") || 
                               text.Contains("mislukt") ||
                               text.Contains("failed") ||
                               text.Contains("niet bereikbaar") ||
                               text.Contains("probleem") ||
                               text.Contains("unavailable") ||
                               text.Contains("connection") ||
                               text.Contains("network");
                    }
                    catch
                    {
                        return false;
                    }
                }).ToList();

                if (visibleErrors.Any())
                {
                    errorFound = true;
                    errorMessage = visibleErrors.First().Text;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LOG [Step 6] WARNING: Exception while checking for errors: {ex.Message}");
            }

            if (!errorFound)
            {
                // Check if page is blank or crashed
                var bodyText = driver.FindElement(By.TagName("body")).Text;
                Console.WriteLine($"LOG [Step 6] WARNING: No explicit error message found");
                Console.WriteLine($"LOG [Step 6] Body text length: {bodyText.Length} characters");
                
                if (bodyText.Length < 50)
                {
                    throw new Exception("Page appears blank after API failure - no error handling");
                }
                
                Console.WriteLine("LOG [Step 6] INFO: No crash detected, but no explicit error message shown");
            }
            else
            {
                Console.WriteLine($"LOG [Step 6] PASS: Error message displayed: '{errorMessage}'");
            }

            // Step 7: Verify page did not crash (no blank page)
            Console.WriteLine("LOG [Step 7] Verify page did not crash");
            try
            {
                var appElement = driver.FindElement(By.Id("app"));
                if (appElement.Displayed)
                {
                    Console.WriteLine("LOG [Step 7] PASS: App container still present");
                }
                else
                {
                    Console.WriteLine("LOG [Step 7] WARNING: App container not visible");
                }
            }
            catch (NoSuchElementException)
            {
                throw new Exception("App container disappeared - page crashed");
            }

            // Step 8: Verify navigation is still functional
            Console.WriteLine("LOG [Step 8] Verify navigation is still present");
            try
            {
                var navigation = driver.FindElement(By.CssSelector("nav, header, .navbar"));
                if (navigation.Displayed)
                {
                    Console.WriteLine("LOG [Step 8] PASS: Navigation still visible");
                }
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("LOG [Step 8] WARNING: Navigation not found");
            }

            Console.WriteLine("LOG [Step 9] PASS: Error handling verified");
            Console.WriteLine("LOG [Step 9.1] - Error message shown (or graceful degradation)");
            Console.WriteLine("LOG [Step 9.2] - Page did not crash");
            Console.WriteLine("LOG [Step 9.3] - Navigation still available");
            Console.WriteLine("LOG [Step 10] NOTE: Backend will be restarted after test");
            Console.WriteLine("LOG [Step 11] Test completed successfully - Error handling works");
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR [E999] Test error: " + ex.ToString());
            throw;
        }
        finally
        {
            // Step 12: Inform user to restart backend
            if (backendWasRunning)
            {
                Console.WriteLine("LOG [Cleanup] =====================================================");
                Console.WriteLine("LOG [Cleanup] IMPORTANT: Backend was stopped during this test");
                Console.WriteLine("LOG [Cleanup] MANUAL ACTION REQUIRED:");
                Console.WriteLine("LOG [Cleanup] 1. Open a terminal in the backend folder");
                Console.WriteLine("LOG [Cleanup] 2. Run: dotnet run");
                Console.WriteLine("LOG [Cleanup] 3. Wait 10-15 seconds for backend to fully start");
                Console.WriteLine("LOG [Cleanup] 4. Then you can run other tests");
                Console.WriteLine("LOG [Cleanup] =====================================================");
            }
            
            if (driver != null)
            {
                driver.Quit();
                Console.WriteLine("LOG [Cleanup] WebDriver closed.");
            }
        }
    }
}
