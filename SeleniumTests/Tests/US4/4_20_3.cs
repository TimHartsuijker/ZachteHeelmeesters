using OpenQA.Selenium;
using SeleniumTests.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace US4._20
{
    [TestClass]
    public class _4_20_3 : BaseTest
    {
        [TestMethod]
        public void TC_4_20_3_UpdateAvailability_SetUnavailableWithReason_PersistsAfterReload()
        {
            string userEmail = "gebruiker@example.com";
            string userPassword = "Wachtwoord123";
            string userName = "Test Gebruiker";
            string periodReason = "Test Meeting";

            // Stap 1: Login
            LogStep(1, "Navigating to login page...");
            loginPage.Navigate();
            LogSuccess(1, "Login page loaded.");

            LogStep(2, $"Logging in as: {userEmail}");
            loginPage.PerformLogin(userEmail, userPassword);
            LogSuccess(2, "Credentials submitted.");

            // Stap 3: Redirect naar Agenda via POM
            LogStep(3, "Navigating to the agenda page...");
            dashboardPage.ClickAppointments();

            // Gebruik de wait uit BaseTest om URL te controleren
            wait.Until(d => calendarPage.IsCalendarVisible());
            LogSuccess(3, $"Successfully reached agenda: {driver.Url}");

            LogStep(4, "Verifying calendar and user information...");
            wait.Until(d => d.FindElement(By.ClassName("user-info-inline")).Displayed);
            string displayedInfo = driver.FindElement(By.ClassName("user-info-inline")).Text;
            Assert.IsTrue(displayedInfo.Contains(userName), $"Wrong user: {displayedInfo}");
            LogSuccess(4, $"User verified: {displayedInfo}");

            // Stap 3: Initiële staat vastleggen
            LogStep(5, "Recording initial availability state...");
            wait.Until(d => d.FindElements(By.ClassName("time-slot")).Count > 0);
            int initialUnavailableCount = driver.FindElements(By.CssSelector(".time-slot.unavailable")).Count;
            LogInfo($"Initial unavailable slots: {initialUnavailableCount}");
            LogSuccess(5, "Initial state recorded.");

            // Stap 4: Modal openen
            LogStep(6, "Opening 'Periode onbeschikbaar maken' modal...");
            var unavailableButton = driver.FindElement(By.CssSelector(".quick-actions button.btn-primary"));
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", unavailableButton);
            js.ExecuteScript("arguments[0].click();", unavailableButton);

            wait.Until(d => d.FindElements(By.ClassName("modal")).Count > 0 && d.FindElement(By.ClassName("modal")).Displayed);
            LogSuccess(6, "Unavailability modal opened.");

            // Stap 5: Tijd en reden invoeren
            LogStep(7, "Selecting time slot and entering reason...");
            var availableSlots = driver.FindElements(By.CssSelector(".time-slot:not(.unavailable)"));
            Assert.IsTrue(availableSlots.Count > 0, "No available slots to mark.");

            string slotDate = availableSlots[0].GetAttribute("data-date") ?? DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
            string startDateTime = $"{slotDate}T11:00";

            var startInput = driver.FindElement(By.Id("unavail-start"));
            js.ExecuteScript("arguments[0].value = arguments[1]; arguments[0].dispatchEvent(new Event('input'));", startInput, startDateTime);

            var periodReasonInput = driver.FindElement(By.Id("unavail-reason"));
            periodReasonInput.SendKeys(periodReason);
            LogInfo($"Marking {startDateTime} as unavailable. Reason: {periodReason}");
            LogSuccess(7, "Unavailability details entered.");

            // Stap 6: Opslaan
            LogStep(8, "Submitting the form...");
            driver.FindElement(By.CssSelector(".modal button[type='submit']")).Click();
            wait.Until(d => d.FindElements(By.ClassName("modal")).Count == 0 || !d.FindElement(By.ClassName("modal")).Displayed);
            LogSuccess(8, "Form submitted and modal closed.");

            // Stap 7: Verificatie voor reload
            LogStep(9, "Verifying UI update before reload...");
            System.Threading.Thread.Sleep(2000); // Wacht op verwerking
            int updatedUnavailableCount = driver.FindElements(By.CssSelector(".time-slot.unavailable")).Count;
            Assert.IsTrue(updatedUnavailableCount > initialUnavailableCount, "Unavailable count did not increase.");
            LogSuccess(9, $"Count increased to {updatedUnavailableCount}.");

            // Stap 8: Persistentie na reload
            LogStep(10, "Refreshing page to verify persistence...");
            driver.Navigate().Refresh();
            wait.Until(d => d.FindElement(By.ClassName("user-info-inline")).Displayed);
            wait.Until(d => d.FindElements(By.ClassName("time-slot")).Count > 0);
            LogSuccess(10, "Page reloaded successfully.");

            LogStep(11, "Final verification of persisted data...");
            int reloadedCount = driver.FindElements(By.CssSelector(".time-slot.unavailable")).Count;
            Assert.AreEqual(updatedUnavailableCount, reloadedCount, "Data not persisted after reload.");

            bool reasonFound = false;
            var slots = driver.FindElements(By.CssSelector(".time-slot.unavailable"));
            foreach (var slot in slots)
            {
                try
                {
                    if (slot.FindElement(By.ClassName("unavailable-reason")).Text.Contains(periodReason))
                    {
                        reasonFound = true; break;
                    }
                }
                catch { /* skip */ }
            }
            Assert.IsTrue(reasonFound, "Reason not found on any slot after reload.");
            LogSuccess(11, "Persistence verified: Slot remains unavailable with correct reason.");
        }
    }
}