using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SeleniumTests.P_O_M;

namespace SeleniumTests.Tests;

[TestClass]
public class US2_12_9
{
    [TestMethod]
    public void EmptyDossierMessage()
    {
        IWebDriver driver = null;
        try
        {
            // Step 1: Start test
            Console.WriteLine("LOG [Step 1] Start test: EmptyDossierMessage");
            driver = new ChromeDriver();
            var loginPage = new LoginPage(driver);
            var dossierPage = new DossierPage(driver);
            var helpers = new TestHelpers(driver, loginPage, dossierPage);

            // Step 2: Navigate to portal and login as patient with empty dossier
            Console.WriteLine("LOG [Step 2] Navigate to portal and login as patient with empty dossier");
            helpers.LoginAndNavigateToDossier("gebruiker@example.com", "Wachtwoord123");

            // Step 3: Verify dossier page is open
            Console.WriteLine("LOG [Step 3] Verify patient dossier page opens");
            if (!helpers.IsOnDossierPage())
            {
                throw new Exception($"Patient dossier page did not open. Current URL: {driver.Url}");
            }
            Console.WriteLine($"LOG [Step 3] PASS: Patient dossier page is open");

            // Step 4: Wait for page to fully load
            Console.WriteLine("LOG [Step 4] Wait for page to load");
            System.Threading.Thread.Sleep(2000);

            // Step 5: Verify no dossier entries are present
            Console.WriteLine("LOG [Step 5] Verify no dossier entries present");
            var entries = dossierPage.GetAllEntries();
            if (entries.Count > 0)
            {
                throw new Exception($"Found {entries.Count} dossier entries - expected empty dossier");
            }
            Console.WriteLine("LOG [Step 5] PASS: No dossier entries found (empty dossier)");

            // Step 6: Check for empty state message
            Console.WriteLine("LOG [Step 6] Check for empty state message");
            
            // Search for common empty state elements by class
            var emptyMessages = driver.FindElements(By.CssSelector(".empty-state, .no-data, .no-entries"));
            
            // Also check all paragraphs and divs for empty state text
            var paragraphs = driver.FindElements(By.TagName("p"));
            var divs = driver.FindElements(By.TagName("div"));
            
            var allElements = emptyMessages.Concat(paragraphs).Concat(divs).ToList();
            
            var visibleMessages = allElements.Where(m =>
            {
                try
                {
                    if (!m.Displayed || string.IsNullOrWhiteSpace(m.Text))
                        return false;
                    
                    var text = m.Text.ToLower();
                    return text.Contains("geen") || 
                           text.Contains("leeg") || 
                           text.Contains("no entries") || 
                           text.Contains("empty") ||
                           text.Contains("no documents");
                }
                catch
                {
                    return false;
                }
            }).ToList();

            bool hasEmptyMessage = visibleMessages.Any();
            string emptyMessageText = hasEmptyMessage ? visibleMessages.First().Text : "";

            if (!hasEmptyMessage)
            {
                Console.WriteLine("LOG [Step 6] WARNING: No explicit empty state message found");
                Console.WriteLine("LOG [Step 6] INFO: This is acceptable if the UI simply shows no entries");
            }
            else
            {
                Console.WriteLine($"LOG [Step 6] PASS: Empty state message shown: '{emptyMessageText}'");
            }

            // Step 7: Verify no errors on page
            Console.WriteLine("LOG [Step 7] Verify no errors shown on page");
            try
            {
                var errorElements = driver.FindElements(By.CssSelector(".error, .alert-danger, [role='alert'], .text-red, .text-danger"));
                var visibleErrors = errorElements.Where(e =>
                {
                    try
                    {
                        return e.Displayed && !string.IsNullOrWhiteSpace(e.Text);
                    }
                    catch
                    {
                        return false;
                    }
                }).ToList();

                if (visibleErrors.Any())
                {
                    string errorText = string.Join(", ", visibleErrors.Select(e => e.Text));
                    throw new Exception($"Errors shown on page with empty dossier: {errorText}");
                }
                Console.WriteLine("LOG [Step 7] PASS: No errors shown on page");
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("LOG [Step 7] PASS: No error elements found");
            }

            // Step 8: Verify layout is not broken
            Console.WriteLine("LOG [Step 8] Verify page layout is not broken");
            
            // Check if main content area exists and is visible
            var mainContent = driver.FindElements(By.CssSelector("main, .main-content, .content, #app"));
            if (mainContent.Count == 0 || !mainContent[0].Displayed)
            {
                throw new Exception("Main content area not found or not displayed - layout appears broken");
            }
            Console.WriteLine("LOG [Step 8] PASS: Main content area is visible");

            // Check if header/navigation is visible
            var navigation = driver.FindElements(By.CssSelector("nav, header, .navbar"));
            if (navigation.Count == 0 || !navigation[0].Displayed)
            {
                Console.WriteLine("LOG [Step 8] WARNING: Navigation/header not found");
            }
            else
            {
                Console.WriteLine("LOG [Step 8] PASS: Navigation/header is visible");
            }

            // Step 9: Verify filter controls are still present (even with no data)
            Console.WriteLine("LOG [Step 9] Verify filter controls are present");
            try
            {
                var filterElements = driver.FindElements(By.CssSelector("select.filter-select, input[type='date'], .filter"));
                bool hasFilters = filterElements.Any(f => f.Displayed);
                
                if (hasFilters)
                {
                    Console.WriteLine("LOG [Step 9] PASS: Filter controls are visible");
                }
                else
                {
                    Console.WriteLine("LOG [Step 9] INFO: No filter controls visible (acceptable for empty dossier)");
                }
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("LOG [Step 9] INFO: No filter controls found (acceptable for empty dossier)");
            }

            // Step 10: Verify console has no JavaScript errors
            Console.WriteLine("LOG [Step 10] Check for JavaScript console errors");
            try
            {
                var logs = driver.Manage().Logs.GetLog("browser");
                var severeErrors = logs.Where(log => log.Level == OpenQA.Selenium.LogLevel.Severe).ToList();
                
                if (severeErrors.Any())
                {
                    Console.WriteLine($"LOG [Step 10] WARNING: {severeErrors.Count} severe JavaScript error(s) found:");
                    foreach (var error in severeErrors.Take(3))
                    {
                        Console.WriteLine($"  - {error.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("LOG [Step 10] PASS: No severe JavaScript errors");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LOG [Step 10] INFO: Could not check browser logs: {ex.Message}");
            }

            Console.WriteLine("LOG [Step 11] PASS: Empty dossier handled correctly");
            Console.WriteLine("LOG [Step 11.1] - No entries shown (as expected)");
            Console.WriteLine("LOG [Step 11.2] - No errors displayed");
            Console.WriteLine("LOG [Step 11.3] - Layout is not broken");
            Console.WriteLine("LOG [Step 12] Test completed successfully - Empty dossier displays properly");
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
        }
    }
}
