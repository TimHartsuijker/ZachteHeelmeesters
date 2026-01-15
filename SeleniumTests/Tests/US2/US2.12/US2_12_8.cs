using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SeleniumTests.P_O_M;

namespace SeleniumTests.Tests;

[TestClass]
public class US2_12_8
{
    [TestMethod]
    public void ReadOnlyAccessEnforcement()
    {
        IWebDriver driver = null;
        try
        {
            // Step 1: Start test
            Console.WriteLine("LOG [Step 1] Start test: ReadOnlyAccessEnforcement");
            driver = new ChromeDriver();
            var loginPage = new LoginPage(driver);
            var dossierPage = new DossierPage(driver);
            var helpers = new TestHelpers(driver, loginPage, dossierPage);

            // Step 2: Navigate to portal and login as patient
            Console.WriteLine("LOG [Step 2] Navigate to portal and login as patient");
            helpers.LoginAndNavigateToDossier("gebruiker@example.com", "Wachtwoord123");

            // Step 3: Verify dossier page is open
            Console.WriteLine("LOG [Step 3] Verify patient dossier page opens");
            if (!helpers.IsOnDossierPage())
            {
                throw new Exception($"Patient dossier page did not open. Current URL: {driver.Url}");
            }
            Console.WriteLine($"LOG [Step 3] PASS: Patient dossier page is open");

            // Step 4: Check for upload/add buttons (should not exist for patients)
            Console.WriteLine("LOG [Step 4] Check for upload/add buttons");
            var uploadButtons = driver.FindElements(By.CssSelector("button[type='submit'], button.upload, input[type='file'], .upload-button, .btn-upload"));
            var visibleUploadButtons = uploadButtons.Where(b => b.Displayed && b.Enabled).ToList();
            
            if (visibleUploadButtons.Any())
            {
                string buttonText = string.Join(", ", visibleUploadButtons.Select(b => b.Text));
                throw new Exception($"Upload buttons found for patient (should be hidden): {buttonText}");
            }
            Console.WriteLine("LOG [Step 4] PASS: No upload buttons visible to patient");

            // Step 5: Check for delete buttons (should not exist for patients)
            Console.WriteLine("LOG [Step 5] Check for delete buttons");
            var deleteButtons = driver.FindElements(By.CssSelector("button.delete, .btn-delete, button[title*='delete'], button[title*='Delete'], button[aria-label*='delete']"));
            var visibleDeleteButtons = deleteButtons.Where(b => b.Displayed && b.Enabled).ToList();
            
            if (visibleDeleteButtons.Any())
            {
                string buttonText = string.Join(", ", visibleDeleteButtons.Select(b => b.Text));
                throw new Exception($"Delete buttons found for patient (should be hidden): {buttonText}");
            }
            Console.WriteLine("LOG [Step 5] PASS: No delete buttons visible to patient");

            // Step 6: Check for edit buttons (should not exist for patients)
            Console.WriteLine("LOG [Step 6] Check for edit buttons");
            var editButtons = driver.FindElements(By.CssSelector("button.edit, .btn-edit, button[title*='edit'], button[title*='Edit'], button[aria-label*='edit']"));
            var visibleEditButtons = editButtons.Where(b => b.Displayed && b.Enabled).ToList();
            
            if (visibleEditButtons.Any())
            {
                string buttonText = string.Join(", ", visibleEditButtons.Select(b => b.Text));
                throw new Exception($"Edit buttons found for patient (should be hidden): {buttonText}");
            }
            Console.WriteLine("LOG [Step 6] PASS: No edit buttons visible to patient");

            // Step 7: Check for plus/add icon buttons
            Console.WriteLine("LOG [Step 7] Check for plus/add icon buttons");
            var addButtons = driver.FindElements(By.CssSelector("button[title*='add'], button[title*='Add'], button[aria-label*='add'], .btn-add, .add-button, button:has(svg[data-icon='plus'])"));
            var visibleAddButtons = addButtons.Where(b => b.Displayed && b.Enabled).ToList();
            
            if (visibleAddButtons.Any())
            {
                string buttonText = string.Join(", ", visibleAddButtons.Select(b => b.Text));
                throw new Exception($"Add buttons found for patient (should be hidden): {buttonText}");
            }
            Console.WriteLine("LOG [Step 7] PASS: No add buttons visible to patient");

            // Step 8: Check for any form inputs that would allow modification
            Console.WriteLine("LOG [Step 8] Check for modification form inputs");
            var formInputs = driver.FindElements(By.CssSelector("input[type='text']:not([readonly]), textarea:not([readonly]), input[type='file']"));
            var visibleFormInputs = formInputs.Where(input => 
            {
                try
                {
                    return input.Displayed && input.Enabled;
                }
                catch
                {
                    return false;
                }
            }).ToList();
            
            // Filter out search/filter inputs (these are allowed)
            var modificationInputs = visibleFormInputs.Where(input =>
            {
                try
                {
                    var inputClass = input.GetAttribute("class") ?? "";
                    var inputPlaceholder = input.GetAttribute("placeholder") ?? "";
                    
                    // Allow filter/search inputs
                    bool isFilterInput = inputClass.Contains("filter") || 
                                       inputClass.Contains("search") ||
                                       inputPlaceholder.ToLower().Contains("zoek") ||
                                       inputPlaceholder.ToLower().Contains("filter");
                    
                    return !isFilterInput;
                }
                catch
                {
                    return true; // Include if we can't check
                }
            }).ToList();
            
            if (modificationInputs.Any())
            {
                string inputInfo = string.Join(", ", modificationInputs.Select(i => 
                {
                    try { return i.GetAttribute("name") ?? i.GetAttribute("id") ?? "unknown"; }
                    catch { return "unknown"; }
                }));
                throw new Exception($"Modification form inputs found for patient (should be hidden): {inputInfo}");
            }
            Console.WriteLine("LOG [Step 8] PASS: No modification form inputs visible to patient");

            // Step 9: Verify navigation bar doesn't have doctor-specific options
            Console.WriteLine("LOG [Step 9] Check for doctor-specific navigation options");
            var doctorNavLinks = driver.FindElements(By.CssSelector("a[href*='upload'], a[href*='doctor'], nav a"));
            var doctorSpecificLinks = doctorNavLinks.Where(link =>
            {
                try
                {
                    if (!link.Displayed) return false;
                    
                    string linkText = link.Text.ToLower();
                    string href = link.GetAttribute("href")?.ToLower() ?? "";
                    
                    // Check if it's a doctor-specific link
                    return href.Contains("/doctor/") || 
                           linkText.Contains("upload") ||
                           linkText.Contains("uploaden");
                }
                catch
                {
                    return false;
                }
            }).ToList();
            
            if (doctorSpecificLinks.Any())
            {
                string linkText = string.Join(", ", doctorSpecificLinks.Select(l => l.Text));
                throw new Exception($"Doctor-specific navigation links found for patient (should be hidden): {linkText}");
            }
            Console.WriteLine("LOG [Step 9] PASS: No doctor-specific navigation options visible");

            // Step 10: Summary of read-only enforcement
            Console.WriteLine("LOG [Step 10] PASS: Patient UI is read-only");
            Console.WriteLine("LOG [Step 10.1] - No upload buttons");
            Console.WriteLine("LOG [Step 10.2] - No delete buttons");
            Console.WriteLine("LOG [Step 10.3] - No edit buttons");
            Console.WriteLine("LOG [Step 10.4] - No add buttons");
            Console.WriteLine("LOG [Step 10.5] - No modification inputs");
            Console.WriteLine("LOG [Step 10.6] - No doctor-specific navigation");

            Console.WriteLine("LOG [Step 11] Test completed successfully - Read-only access properly enforced for patient");
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
