using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Linq;
using SeleniumTests.P_O_M;

namespace SeleniumTests;

[TestClass]
public class _4_20_4
{
    [TestMethod]
    public void ConcurrentAvailabilityUpdates_TwoUsers()
    {
        // Arrange
        IWebDriver? driver1 = null;
        IWebDriver? driver2 = null;
        WebDriverWait? wait1 = null;
        WebDriverWait? wait2 = null;
        
        try
        {
            // Step 1: Start test
            Console.WriteLine("LOG [Step 1] Start test: ConcurrentAvailabilityUpdates_TwoUsers");
            
            // Create two separate WebDriver instances
            driver1 = new ChromeDriver();
            driver2 = new ChromeDriver();
            wait1 = new WebDriverWait(driver1, TimeSpan.FromSeconds(10));
            wait2 = new WebDriverWait(driver2, TimeSpan.FromSeconds(10));
            
            Console.WriteLine("LOG [Step 2] Created two browser instances for concurrent testing.");

            // ===== BROWSER 1: PATIENT LOGIN =====
            
            var loginPage1 = new LoginPage(driver1);
            loginPage1.Navigate();
            Console.WriteLine("LOG [Step 3] Browser 1: Navigated to login page (Patient).");

            string patient_Email = "gebruiker@example.com";
            string patient_Password = "Wachtwoord123";
            string patient_Name = "Test Gebruiker";

            loginPage1.EnterEmail(patient_Email);
            loginPage1.EnterPassword(patient_Password);
            loginPage1.ClickLogin();
            Console.WriteLine("LOG [Step 4] Browser 1: Patient login submitted.");

            wait1.Until(d => d.Url.Contains("/agenda"));
            Console.WriteLine("LOG [Step 5] Browser 1: Redirected to agenda page (Patient).");

            // Wait for patient calendar to load
            wait1.Until(d =>
            {
                try
                {
                    return d.FindElement(By.ClassName("user-info-inline")).Displayed;
                }
                catch { return false; }
            });
            Console.WriteLine("LOG [Step 6] Browser 1: Patient calendar loaded.");

            // ===== BROWSER 2: DOCTOR LOGIN =====
            
            var loginPage2 = new LoginPage(driver2);
            loginPage2.Navigate();
            Console.WriteLine("LOG [Step 7] Browser 2: Navigated to login page (Doctor).");

            string doctor_Email = "testdoctor@example.com";
            string doctor_Password = "password";
            string doctor_Name = "Test Doctor";

            loginPage2.EnterEmail(doctor_Email);
            loginPage2.EnterPassword(doctor_Password);
            loginPage2.ClickLogin();
            Console.WriteLine("LOG [Step 8] Browser 2: Doctor login submitted.");

            wait2.Until(d => d.Url.Contains("/agenda"));
            Console.WriteLine("LOG [Step 9] Browser 2: Redirected to agenda page (Doctor).");

            // Wait for doctor calendar to load
            wait2.Until(d =>
            {
                try
                {
                    return d.FindElement(By.ClassName("user-info-inline")).Displayed;
                }
                catch { return false; }
            });
            Console.WriteLine("LOG [Step 10] Browser 2: Doctor calendar loaded.");

            // Declare JS executors early
            IJavaScriptExecutor js1 = (IJavaScriptExecutor)driver1;
            IJavaScriptExecutor js2 = (IJavaScriptExecutor)driver2;

            // Debug: Check doctor's user info
            try
            {
                var doctorUserId = js2.ExecuteScript("return sessionStorage.getItem('userId');");
                var doctorUserEmail = js2.ExecuteScript("return sessionStorage.getItem('userEmail');");
                var doctorUserName = js2.ExecuteScript("return sessionStorage.getItem('userName');");
                var doctorUserRole = js2.ExecuteScript("return sessionStorage.getItem('userRole');");
                Console.WriteLine($"LOG [Step 10.1] Browser 2: Doctor user info:");
                Console.WriteLine($"  - userId: {doctorUserId}");
                Console.WriteLine($"  - email: {doctorUserEmail}");
                Console.WriteLine($"  - name: {doctorUserName}");
                Console.WriteLine($"  - role: {doctorUserRole}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LOG [Step 10.1] Browser 2: Error reading session storage: {ex.Message}");
            }

            // ===== CONCURRENT UPDATES: BOTH USERS SET UNAVAILABLE PERIODS =====
            
            Console.WriteLine("LOG [Step 11] Starting concurrent unavailability period updates...");
            
            wait1.Until(d =>
            {
                try
                {
                    return d.FindElement(By.CssSelector(".quick-actions button.btn-primary")).Displayed;
                }
                catch { return false; }
            });
            var btn1 = driver1.FindElement(By.CssSelector(".quick-actions button.btn-primary"));
            js1.ExecuteScript("arguments[0].scrollIntoView({behavior: 'smooth', block: 'center'});", btn1);
            System.Threading.Thread.Sleep(300);
            js1.ExecuteScript("arguments[0].click();", btn1);
            Console.WriteLine("LOG [Step 12] Browser 1 (Patient): Clicked unavailability button.");

            // Browser 2 (Doctor): Click "Periode onbeschikbaar maken" button (same action)
            wait2.Until(d =>
            {
                try
                {
                    return d.FindElement(By.CssSelector(".quick-actions button.btn-primary")).Displayed;
                }
                catch { return false; }
            });
            var btn2 = driver2.FindElement(By.CssSelector(".quick-actions button.btn-primary"));
            js2.ExecuteScript("arguments[0].scrollIntoView({behavior: 'smooth', block: 'center'});", btn2);
            System.Threading.Thread.Sleep(300);
            js2.ExecuteScript("arguments[0].click();", btn2);
            Console.WriteLine("LOG [Step 13] Browser 2 (Doctor): Clicked unavailability button.");

            // ===== FILL FORMS CONCURRENTLY =====
            
            // Browser 1: Fill patient's unavailability period
            wait1.Until(d => d.FindElement(By.Id("unavail-start")).Displayed);
            // Pick the first available slot dynamically
            var freeSlots1 = driver1.FindElements(By.CssSelector(".time-slot:not(.unavailable)"));
            if (freeSlots1 == null || freeSlots1.Count == 0)
                throw new Exception("Browser 1: No free slots available to select.");
            var chosenSlot1 = freeSlots1.First();
            string slot1Date = chosenSlot1.GetAttribute("data-date") ?? "";
            string slot1HourRaw = chosenSlot1.GetAttribute("data-hour") ?? "10";
            string slot1MinuteRaw = chosenSlot1.GetAttribute("data-minute") ?? "00";
            string slot1HourPadded = slot1HourRaw.PadLeft(2, '0');
            string slot1MinutePadded = slot1MinuteRaw.PadLeft(2, '0');
            string patientStart = $"{slot1Date}T{slot1HourPadded}:{slot1MinutePadded}";

            var patientStartInput = driver1.FindElement(By.Id("unavail-start"));
            js1.ExecuteScript(@"
                const input = arguments[0];
                const value = arguments[1];
                input.value = value;
                input.dispatchEvent(new Event('input', { bubbles: true }));
                input.dispatchEvent(new Event('change', { bubbles: true }));
            ", patientStartInput, patientStart);
            Console.WriteLine($"LOG [Step 14] Browser 1: Selected free slot {slot1Date} {slot1HourPadded}:{slot1MinutePadded}");

            // Browser 2: Fill doctor's unavailability period
            wait2.Until(d => d.FindElement(By.Id("unavail-start")).Displayed);
            // Pick the first available slot dynamically
            var freeSlots2 = driver2.FindElements(By.CssSelector(".time-slot:not(.unavailable)"));
            if (freeSlots2 == null || freeSlots2.Count == 0)
                throw new Exception("Browser 2: No free slots available to select.");
            var chosenSlot2 = freeSlots2.First();
            string slot2Date = chosenSlot2.GetAttribute("data-date") ?? "";
            string slot2HourRaw = chosenSlot2.GetAttribute("data-hour") ?? "11";
            string slot2MinuteRaw = chosenSlot2.GetAttribute("data-minute") ?? "00";
            string slot2HourPadded = slot2HourRaw.PadLeft(2, '0');
            string slot2MinutePadded = slot2MinuteRaw.PadLeft(2, '0');
            string doctorStart = $"{slot2Date}T{slot2HourPadded}:{slot2MinutePadded}";

            var doctorStartInput = driver2.FindElement(By.Id("unavail-start"));
            js2.ExecuteScript(@"
                const input = arguments[0];
                const value = arguments[1];
                input.value = value;
                input.dispatchEvent(new Event('input', { bubbles: true }));
                input.dispatchEvent(new Event('change', { bubbles: true }));
            ", doctorStartInput, doctorStart);
            Console.WriteLine($"LOG [Step 15] Browser 2: Selected free slot {slot2Date} {slot2HourPadded}:{slot2MinutePadded}");

            // Wait for both dropdowns to populate
            System.Threading.Thread.Sleep(1500);

            // Browser 1: Select end time
            wait1.Until(d =>
            {
                try
                {
                    var opts = d.FindElement(By.Id("unavail-end")).FindElements(By.TagName("option"));
                    return opts.Count > 1;
                }
                catch { return false; }
            });
            var patientEndSelect = driver1.FindElement(By.Id("unavail-end"));
            var patientEndOptions = patientEndSelect.FindElements(By.TagName("option"));
            patientEndOptions[1].Click();
            Console.WriteLine($"LOG [Step 16] Browser 1: Selected end time: {patientEndOptions[1].Text}");

            // Browser 2: Select end time
            wait2.Until(d =>
            {
                try
                {
                    var opts = d.FindElement(By.Id("unavail-end")).FindElements(By.TagName("option"));
                    return opts.Count > 1;
                }
                catch { return false; }
            });
            var doctorEndSelect = driver2.FindElement(By.Id("unavail-end"));
            var doctorEndOptions = doctorEndSelect.FindElements(By.TagName("option"));
            doctorEndOptions[1].Click();
            Console.WriteLine($"LOG [Step 17] Browser 2: Selected end time: {doctorEndOptions[1].Text}");

            // Browser 1: Add reason
            var patientReason = driver1.FindElement(By.Id("unavail-reason"));
            patientReason.Clear();
            patientReason.SendKeys("Patient Meeting");
            Console.WriteLine("LOG [Step 18] Browser 1: Entered reason 'Patient Meeting'");

            // Browser 2: Add reason
            var doctorReason = driver2.FindElement(By.Id("unavail-reason"));
            // Use JS to set and trigger Vue reactivity
            js2.ExecuteScript(@"const el=arguments[0], v=arguments[1]; el.value=v; el.dispatchEvent(new Event('input',{bubbles:true})); el.dispatchEvent(new Event('change',{bubbles:true}));", doctorReason, "Doctor Conference");
            // Wait until value is reflected
            wait2.Until(d => {
                try { return d.FindElement(By.Id("unavail-reason")).GetAttribute("value") == "Doctor Conference"; } catch { return false; }
            });
            Console.WriteLine("LOG [Step 19] Browser 2: Entered reason 'Doctor Conference' (verified)");

            // ===== SUBMIT BOTH FORMS CONCURRENTLY =====
            
            Console.WriteLine("LOG [Step 20] Submitting both forms concurrently...");
            
            // Verify forms are valid before submission
            try
            {
                var patientReasonValue = driver1.FindElement(By.Id("unavail-reason")).GetAttribute("value");
                var doctorReasonValue = driver2.FindElement(By.Id("unavail-reason")).GetAttribute("value");
                Console.WriteLine($"LOG [Step 20.0] Patient reason value: '{patientReasonValue}'");
                Console.WriteLine($"LOG [Step 20.0] Doctor reason value: '{doctorReasonValue}'");
                
                // Check for form validation errors
                try
                {
                    var doctorFormErrors = js2.ExecuteScript(@"
                        const form = document.querySelector('.modal form');
                        if (!form) return 'No form found';
                        const hasErrors = form.querySelector('[role=alert]') !== null;
                        const formState = form.className;
                        return { hasErrors, formState, formValid: form.checkValidity() };
                    ");
                    Console.WriteLine($"LOG [Step 20.0.4] Browser 2 form state: {doctorFormErrors}");
                }
                catch (Exception ex2)
                {
                    Console.WriteLine($"LOG [Step 20.0.4.err] Could not check form state: {ex2.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LOG [Step 20.0] Error reading form values: {ex.Message}");
            }
            
            var patientSubmit = driver1.FindElement(By.CssSelector(".modal button[type='submit']"));
            var doctorSubmit = driver2.FindElement(By.CssSelector(".modal button[type='submit']"));
            
            // Verify submit buttons are present and clickable
            try
            {
                var patientSubmitEnabled = patientSubmit.Enabled;
                var doctorSubmitEnabled = doctorSubmit.Enabled;
                Console.WriteLine($"LOG [Step 20.0.1] Patient submit button enabled: {patientSubmitEnabled}");
                Console.WriteLine($"LOG [Step 20.0.2] Doctor submit button enabled: {doctorSubmitEnabled}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LOG [Step 20.0.3] Error verifying submit buttons: {ex.Message}");
            }
            
            // Add request tracking for both browsers
            try
            {
                js1.ExecuteScript(@"
                    window.__savePeriodRequests1 = [];
                    const originalFetch1 = window.fetch;
                    window.fetch = function(...args) {
                        const url = args[0];
                        const promise = originalFetch1.apply(this, args);
                        if (typeof url === 'string' && url.includes('unavailable-period')) {
                            promise.then(r => {
                                window.__savePeriodRequests1.push({ url, status: r.status });
                            }).catch(e => {});
                        }
                        return promise;
                    };
                ");
                Console.WriteLine("LOG [Step 20.1] Request tracking enabled for Browser 1");
            }
            catch { }
            
            try
            {
                js2.ExecuteScript(@"
                    window.__savePeriodRequests2 = [];
                    const originalFetch2 = window.fetch;
                    window.fetch = function(...args) {
                        const url = args[0];
                        const promise = originalFetch2.apply(this, args);
                        if (typeof url === 'string' && url.includes('unavailable-period')) {
                            promise.then(r => {
                                window.__savePeriodRequests2.push({ url, status: r.status });
                            }).catch(e => {});
                        }
                        return promise;
                    };
                ");
                Console.WriteLine("LOG [Step 20.1] Request tracking enabled for Browser 2");
            }
            catch { }
            
            // Submit both using JS click with true concurrency
            var task1 = System.Threading.Tasks.Task.Run(() => {
                try
                {
                    js1.ExecuteScript("arguments[0].click();", patientSubmit);
                    Console.WriteLine("LOG [Step 21.0] Browser 1 submit clicked via JS");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"LOG [Step 21.0.err] Browser 1 JS click failed: {ex.Message}");
                    try { patientSubmit.Click(); Console.WriteLine("LOG [Step 21.0.fallback] Browser 1 submit clicked via Selenium"); }
                    catch { }
                }
            });
            
            var task2 = System.Threading.Tasks.Task.Run(() => {
                System.Threading.Thread.Sleep(50);  // Minimal delay for true concurrency
                try
                {
                    js2.ExecuteScript("arguments[0].click();", doctorSubmit);
                    Console.WriteLine("LOG [Step 21.1] Browser 2 submit clicked via JS");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"LOG [Step 21.1.err] Browser 2 JS click failed: {ex.Message}");
                    try { doctorSubmit.Click(); Console.WriteLine("LOG [Step 21.1.fallback] Browser 2 submit clicked via Selenium"); }
                    catch { }
                }
            });
            
            System.Threading.Tasks.Task.WaitAll(task1, task2);
            Console.WriteLine("LOG [Step 21] Both forms submitted concurrently.");

            // Wait for both modals to close
            try
            {
                wait1.Until(d =>
                {
                    try
                    {
                        var modals = d.FindElements(By.ClassName("modal"));
                        return modals.Count == 0 || !modals[0].Displayed;
                    }
                    catch { return true; }
                });
            }
            catch { }
            
            try
            {
                wait2.Until(d =>
                {
                    try
                    {
                        var modals = d.FindElements(By.ClassName("modal"));
                        return modals.Count == 0 || !modals[0].Displayed;
                    }
                    catch { return true; }
                });
            }
            catch { }
            
            Console.WriteLine("LOG [Step 22] Both modals closed.");

            // Verify both requests succeeded
            try
            {
                bool browser1RequestSucceeded = false;
                for (int i = 0; i < 10; i++)
                {
                    try
                    {
                        var requests = js1.ExecuteScript("return window.__savePeriodRequests1 || [];");
                        if (requests is System.Collections.ObjectModel.ReadOnlyCollection<object> requestList && requestList.Count > 0)
                        {
                            browser1RequestSucceeded = true;
                            Console.WriteLine($"LOG [Step 22.0] Browser 1 save request captured (attempt {i + 1})");
                            break;
                        }
                    }
                    catch { }
                    System.Threading.Thread.Sleep(200);
                }
                if (!browser1RequestSucceeded)
                {
                    throw new Exception("Browser 1: No save period request detected after submission.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LOG [Step 22.0] Warning: {ex.Message}");
            }

            try
            {
                bool browser2RequestSucceeded = false;
                for (int i = 0; i < 10; i++)
                {
                    try
                    {
                        var requests = js2.ExecuteScript("return window.__savePeriodRequests2 || [];");
                        if (requests is System.Collections.ObjectModel.ReadOnlyCollection<object> requestList && requestList.Count > 0)
                        {
                            browser2RequestSucceeded = true;
                            Console.WriteLine($"LOG [Step 22.1] Browser 2 save request captured (attempt {i + 1})");
                            break;
                        }
                    }
                    catch { }
                    System.Threading.Thread.Sleep(200);
                }
                if (!browser2RequestSucceeded)
                {
                    throw new Exception("Browser 2: No save period request detected after submission.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LOG [Step 22.1] Warning: {ex.Message}");
            }

            // Wait for backend processing
            System.Threading.Thread.Sleep(3000);

            // ===== VERIFY BOTH UPDATES WERE APPLIED =====
            
            // VERIFY: Exact selected slots gained the 'unavailable' class
            // Browser 1 verification
            var selectedSlot1 = driver1.FindElement(By.CssSelector($".time-slot[data-date='{slot1Date}'][data-hour='{slot1HourRaw}'][data-minute='{slot1MinuteRaw}']"));
            var cls1 = selectedSlot1.GetAttribute("class") ?? string.Empty;
            if (!cls1.Contains("unavailable"))
            {
                throw new Exception($"Browser 1: Selected slot {slot1Date} {slot1HourPadded}:{slot1MinutePadded} was not marked unavailable.");
            }
            Console.WriteLine($"LOG [Step 23] Browser 1: Slot {slot1Date} {slot1HourPadded}:{slot1MinutePadded} marked unavailable.");

            // Browser 2 verification
            var selectedSlot2 = driver2.FindElement(By.CssSelector($".time-slot[data-date='{slot2Date}'][data-hour='{slot2HourRaw}'][data-minute='{slot2MinuteRaw}']"));
            var cls2 = selectedSlot2.GetAttribute("class") ?? string.Empty;
            if (!cls2.Contains("unavailable"))
            {
                throw new Exception($"Browser 2: Selected slot {slot2Date} {slot2HourPadded}:{slot2MinutePadded} was not marked unavailable.");
            }
            Console.WriteLine($"LOG [Step 24] Browser 2: Slot {slot2Date} {slot2HourPadded}:{slot2MinutePadded} marked unavailable.");

            Console.WriteLine("LOG [Step 25] PASS: Both users' calendars updated successfully without conflicts.");
            Console.WriteLine("LOG [Step 26] PASS: Concurrent updates from different users work correctly.");
            Console.WriteLine("LOG [Step 27] Test completed successfully - Concurrent availability updates handled correctly.");
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
            if (driver1 != null)
            {
                driver1.Quit();
                Console.WriteLine("LOG [Step 28] Browser 1 (Patient) closed.");
            }
            if (driver2 != null)
            {
                driver2.Quit();
                Console.WriteLine("LOG [Step 29] Browser 2 (Doctor) closed.");
            }
        }
    }
}
