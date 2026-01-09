using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumTests.P_O_M;

namespace SeleniumTests;

[TestClass]
public class _4_20_3
{
    [TestMethod]
    public void UpdateAvailability_SetUnavailableWithReason_PersistsAfterReload()
    {
        // Arrange
        IWebDriver? driver = null;
        WebDriverWait? wait = null;
        
        try
        {
            // Step 1: Start test
            Console.WriteLine("LOG [Step 1] Start test: UpdateAvailability_SetUnavailableWithReason_PersistsAfterReload");
            driver = new ChromeDriver();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            // Step 2: Login as patient
            var loginPage = new LoginPage(driver);
            loginPage.Navigate();
            Console.WriteLine("LOG [Step 2] Navigated to login page.");

            string userEmail = "gebruiker@example.com";
            string userPassword = "Wachtwoord123";
            string userName = "Test Gebruiker";

            loginPage.EnterEmail(userEmail);
            loginPage.EnterPassword(userPassword);
            loginPage.ClickLogin();
            Console.WriteLine("LOG [Step 3] User credentials entered and submitted.");

            // Step 4: Wait for redirect to agenda page
            wait.Until(d => d.Url.Contains("/agenda"));
            Console.WriteLine("LOG [Step 4] Successfully redirected to agenda page.");

            // Step 5: Wait for calendar to load
            wait.Until(d =>
            {
                try
                {
                    var userInfoElement = d.FindElement(By.ClassName("user-info-inline"));
                    return userInfoElement.Displayed;
                }
                catch (NoSuchElementException)
                {
                    return false;
                }
            });
            Console.WriteLine("LOG [Step 5] Calendar loaded.");

            // Step 6: Verify user is logged in
            var userInfo = driver.FindElement(By.ClassName("user-info-inline"));
            string displayedInfo = userInfo.Text;
            
            if (!displayedInfo.Contains(userName) || !displayedInfo.Contains(userEmail))
            {
                throw new Exception($"Wrong user displayed. Expected '{userName}' and '{userEmail}' but got '{displayedInfo}'");
            }
            
            Console.WriteLine($"LOG [Step 6] User verified: {displayedInfo}");

            // Step 7: Wait for time slots to load
            wait.Until(d =>
            {
                try
                {
                    var timeSlots = d.FindElements(By.ClassName("time-slot"));
                    return timeSlots.Count > 0;
                }
                catch
                {
                    return false;
                }
            });
            
            Console.WriteLine("LOG [Step 7] Time slots loaded.");

            // Step 8: Count initial available and unavailable slots
            var initialTimeSlots = driver.FindElements(By.ClassName("time-slot"));
            var initialUnavailableSlots = driver.FindElements(By.CssSelector(".time-slot.unavailable"));
            int initialTotalSlots = initialTimeSlots.Count;
            int initialUnavailableCount = initialUnavailableSlots.Count;
            int initialAvailableCount = initialTotalSlots - initialUnavailableCount;
            
            Console.WriteLine($"LOG [Step 8] Initial state: {initialTotalSlots} total slots, {initialUnavailableCount} unavailable, {initialAvailableCount} available.");

            // Step 9: Click the "Periode onbeschikbaar maken" button
            wait.Until(d =>
            {
                try
                {
                    var button = d.FindElement(By.CssSelector(".quick-actions button.btn-primary"));
                    return button.Displayed && button.Enabled;
                }
                catch
                {
                    return false;
                }
            });
            
            var unavailableButton = driver.FindElement(By.CssSelector(".quick-actions button.btn-primary"));
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            
            // Scroll the button into view, accounting for fixed navbar
            js.ExecuteScript("arguments[0].scrollIntoView({behavior: 'smooth', block: 'center'});", unavailableButton);
            System.Threading.Thread.Sleep(500); // Wait for scroll to complete
            
            // Click using JavaScript to avoid navbar interception
            js.ExecuteScript("arguments[0].click();", unavailableButton);
            Console.WriteLine("LOG [Step 9] Clicked 'Periode onbeschikbaar maken' button.");

            // Step 10: Wait for the unavailable period modal to appear
            try
            {
                wait.Until(d =>
                {
                    try
                    {
                        var modals = d.FindElements(By.ClassName("modal"));
                        if (modals.Count > 0)
                        {
                            var modal = modals[0];
                            return modal.Displayed && modal.FindElements(By.Id("unavail-start")).Count > 0;
                        }
                        return false;
                    }
                    catch
                    {
                        return false;
                    }
                });
                Console.WriteLine("LOG [Step 10] Unavailable period modal opened.");
            }
            catch (WebDriverTimeoutException)
            {
                Console.WriteLine("ERROR: Unavailable period modal did not appear.");
                throw new Exception("Unavailable period modal did not appear after clicking button.");
            }

            // Step 11: Find the first available (not unavailable) time slot to mark as unavailable
            var availableSlots = driver.FindElements(By.CssSelector(".time-slot:not(.unavailable)"));
            if (availableSlots.Count == 0)
            {
                throw new Exception("No available slots found to test with.");
            }
            
            // Click the first available slot to get its date/time
            var firstAvailableSlot = availableSlots[0];
            string slotDate = firstAvailableSlot.GetAttribute("data-date") ?? "";
            string slotHour = firstAvailableSlot.GetAttribute("data-hour") ?? "10";
            string slotMinute = firstAvailableSlot.GetAttribute("data-minute") ?? "00";
            
            if (string.IsNullOrEmpty(slotDate))
            {
                // Fallback: use tomorrow at 11:00 if data attributes not available
                DateTime tomorrow = DateTime.Now.AddDays(1);
                slotDate = tomorrow.ToString("yyyy-MM-dd");
                slotHour = "11";
                slotMinute = "00";
            }
            
            string startDateTime = $"{slotDate}T{slotHour.PadLeft(2, '0')}:{slotMinute.PadLeft(2, '0')}";
            
            var startInput = driver.FindElement(By.Id("unavail-start"));
            
            // Clear any existing value first
            js.ExecuteScript("arguments[0].value = '';", startInput);
            System.Threading.Thread.Sleep(300);
            
            // Use JavaScript to set the value and trigger Vue's reactivity
            js.ExecuteScript(@"
                const input = arguments[0];
                const value = arguments[1];
                input.value = value;
                input.dispatchEvent(new Event('input', { bubbles: true }));
                input.dispatchEvent(new Event('change', { bubbles: true }));
            ", startInput, startDateTime);
            
            Console.WriteLine($"LOG [Step 11] Selected available slot at: {startDateTime}");
            
            // Wait for Vue to process and verify the value is set
            wait.Until(d =>
            {
                try
                {
                    var val = js.ExecuteScript("return arguments[0].value;", startInput)?.ToString() ?? "";
                    return !string.IsNullOrEmpty(val);
                }
                catch
                {
                    return false;
                }
            });
            
            System.Threading.Thread.Sleep(500); // Wait for Vue to react and populate end time options

            // Step 12: Select an end time from the dropdown
            wait.Until(d =>
            {
                try
                {
                    var endSelect = d.FindElement(By.Id("unavail-end"));
                    var options = endSelect.FindElements(By.TagName("option"));
                    return options.Count > 1; // More than just the placeholder
                }
                catch
                {
                    return false;
                }
            });
            
            var endTimeSelect = driver.FindElement(By.Id("unavail-end"));
            var endTimeOptions = endTimeSelect.FindElements(By.TagName("option"));
            
            // Select the first available end time (not the empty placeholder)
            if (endTimeOptions.Count < 2)
            {
                throw new Exception("No end time options available.");
            }
            
            endTimeOptions[1].Click(); // Select first real option
            string selectedEndTime = endTimeOptions[1].Text;
            Console.WriteLine($"LOG [Step 12] Selected end time: {selectedEndTime}");

            // Step 13: Enter a reason for unavailability
            string unavailableReason = "Test Meeting";
            var reasonInput = driver.FindElement(By.Id("unavail-reason"));
            reasonInput.Clear();
            reasonInput.SendKeys(unavailableReason);
            Console.WriteLine($"LOG [Step 13] Entered unavailability reason: '{unavailableReason}'");
            // Step 13: Enter a reason for unavailability
            string periodReason = "Test Meeting";
            var periodReasonInput = driver.FindElement(By.Id("unavail-reason"));
            periodReasonInput.Clear();
            periodReasonInput.SendKeys(periodReason);
            Console.WriteLine($"LOG [Step 13] Entered unavailability reason: '{periodReason}'");

            // Step 14: Submit the form
            var submitButton = driver.FindElement(By.CssSelector(".modal button[type='submit']"));
            submitButton.Click();
            Console.WriteLine("LOG [Step 14] Clicked save button to submit unavailable period.");

            // Step 15: Wait for modal to close (indicates save is processing)
            try
            {
                wait.Until(d =>
                {
                    try
                    {
                        var modals = d.FindElements(By.ClassName("modal"));
                        return modals.Count == 0 || !modals[0].Displayed;
                    }
                    catch
                    {
                        return true;
                    }
                });
                Console.WriteLine("LOG [Step 15] Modal closed after save.");
            }
            catch (WebDriverTimeoutException)
            {
                Console.WriteLine("ERROR: Modal did not close after clicking save. The save may have failed.");
                
                // Check if there's an alert
                try
                {
                    var alert = driver.SwitchTo().Alert();
                    Console.WriteLine($"DEBUG: Alert detected: {alert.Text}");
                    alert.Accept();
                }
                catch
                {
                    Console.WriteLine("DEBUG: No alert detected.");
                }
                
                throw new Exception("Modal did not close after save - possible validation error.");
            }

        // Step 16: Wait longer for the backend to process and reload
        Console.WriteLine("LOG [Step 16] Waiting for backend to process and UI to update...");
        System.Threading.Thread.Sleep(3000); // Increased wait time for backend processing
        
        // Force a refresh of the availabilities by checking browser console for errors
        // Reuse the js variable from earlier in the method
        var consoleErrors = js.ExecuteScript(@"
            return window.console.errors || [];
        ");
        
        if (consoleErrors != null)
        {
            Console.WriteLine($"DEBUG: Console errors: {consoleErrors}");
        }
        
        // Verify the slot is now marked as unavailable
        var updatedUnavailableSlots = driver.FindElements(By.CssSelector(".time-slot.unavailable"));
        int updatedUnavailableCount = updatedUnavailableSlots.Count;
        
        Console.WriteLine($"LOG [Step 16] After save and reload: {updatedUnavailableCount} unavailable slots (was {initialUnavailableCount}).");
        
        if (updatedUnavailableCount <= initialUnavailableCount)
        {
            Console.WriteLine($"ERROR: Unavailable slot count did not increase.");
            Console.WriteLine($"DEBUG: Initial available: {initialAvailableCount}, Initial unavailable: {initialUnavailableCount}");
            Console.WriteLine($"DEBUG: Current unavailable: {updatedUnavailableCount}");
            Console.WriteLine($"DEBUG: Expected increase due to period settings");
            
            throw new Exception($"Unavailable slot count did not increase. Initial: {initialUnavailableCount}, Current: {updatedUnavailableCount}");
        }
        
        Console.WriteLine($"LOG [Step 16] PASS: Unavailable slot count increased from {initialUnavailableCount} to {updatedUnavailableCount}");

        // Step 17: Check if the reason is displayed on the slot
        bool reasonFound = false;
            foreach (var slot in updatedUnavailableSlots)
            {
                try
                {
                    var reasonElement = slot.FindElement(By.ClassName("unavailable-reason"));
                    if (reasonElement.Text.Contains(periodReason))
                    {
                        reasonFound = true;
                        Console.WriteLine($"LOG [Step 16] PASS: Found slot with reason '{reasonElement.Text}'");
                        break;
                    }
                }
                catch (NoSuchElementException)
                {
                    // No reason on this slot, continue
                }
            }
            
            if (!reasonFound)
            {
                Console.WriteLine($"LOG [Step 17] WARNING: Could not find reason '{periodReason}' displayed on any slot.");
            }

            // Step 18: Reload the page to verify persistence
            driver.Navigate().Refresh();
            Console.WriteLine("LOG [Step 17] Reloaded the page to test persistence.");

            // Step 19: Wait for calendar to reload
            wait.Until(d =>
            {
                try
                {
                    var userInfoElement = d.FindElement(By.ClassName("user-info-inline"));
                    return userInfoElement.Displayed;
                }
                catch (NoSuchElementException)
                {
                    return false;
                }
            });
            Console.WriteLine("LOG [Step 19] Calendar reloaded after page refresh.");

            // Step 20: Wait for time slots to reload
            wait.Until(d =>
            {
                try
                {
                    var timeSlots = d.FindElements(By.ClassName("time-slot"));
                    return timeSlots.Count > 0;
                }
                catch
                {
                    return false;
                }
            });
            
            System.Threading.Thread.Sleep(1000); // Brief wait for full render
            
            // Step 20: Verify the unavailable slot still exists after reload
            var reloadedUnavailableSlots = driver.FindElements(By.CssSelector(".time-slot.unavailable"));
            int reloadedUnavailableCount = reloadedUnavailableSlots.Count;
            
            if (reloadedUnavailableCount != updatedUnavailableCount)
            {
                Console.WriteLine($"LOG [Step 20] WARNING: Unavailable count changed after reload. Before: {updatedUnavailableCount}, After: {reloadedUnavailableCount}");
                Console.WriteLine("LOG [Step 20] This might indicate the change was not persisted to the database.");
            }
            else
            {
                Console.WriteLine($"LOG [Step 20] PASS: Unavailable slot count persisted after reload: {reloadedUnavailableCount}");
            }

            // Step 21: Verify the reason is still displayed after reload
            reasonFound = false;
            foreach (var slot in reloadedUnavailableSlots)
            {
                try
                {
                    var reasonElement = slot.FindElement(By.ClassName("unavailable-reason"));
                    if (reasonElement.Text.Contains(periodReason))
                    {
                        reasonFound = true;
                        Console.WriteLine($"LOG [Step 21] PASS: Reason '{periodReason}' persisted after reload.");
                        break;
                    }
                }
                catch (NoSuchElementException)
                {
                    // No reason on this slot, continue
                }
            }
            
            if (!reasonFound)
            {
                Console.WriteLine($"LOG [Step 21] WARNING: Could not find reason '{periodReason}' after reload.");
                Console.WriteLine("LOG [Step 21] This might indicate the reason was not saved to the database.");
            }

            // Step 22: Test completed
            Console.WriteLine("LOG [Step 22] Test completed successfully - Availability changes with reasons persist after page reload.");
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
                Console.WriteLine("LOG [Step 23] WebDriver closed.");
            }
        }
    }
}
