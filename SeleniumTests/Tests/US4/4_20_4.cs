using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumTests.Pages;
using SeleniumTests.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace US4._20
{
    [TestClass]
    public class _4_20_4 : BaseTest
    {
        [TestMethod]
        public void TC_4_20_4_ConcurrentAvailabilityUpdates_TwoUsers()
        {
            // Arrange: We gebruiken handmatige drivers voor echte concurrency
            IWebDriver? driver1 = null;
            IWebDriver? driver2 = null;

            // Stap 1: Setup browsers
            LogStep(1, "Creating two separate browser instances for concurrent testing...");
            driver1 = new ChromeDriver();
            driver2 = new ChromeDriver();
            var wait1 = new WebDriverWait(driver1, TimeSpan.FromSeconds(10));
            var wait2 = new WebDriverWait(driver2, TimeSpan.FromSeconds(10));
            LogSuccess(1, "Browsers initialized.");

            // Stap 2: Browser 1 - Patient Login
            LogStep(2, "Browser 1: Logging in as Patient (gebruiker@example.com)...");
            var loginPage1 = new LoginPage(driver1);
            loginPage1.Navigate();
            loginPage1.PerformLogin("gebruiker@example.com", "Wachtwoord123");
            wait1.Until(d => d.Url.Contains("/agenda"));
            LogSuccess(2, "Browser 1 reached agenda.");

            // Stap 3: Browser 2 - Doctor Login
            LogStep(3, "Browser 2: Logging in as Doctor (testdoctor@example.com)...");
            var loginPage2 = new LoginPage(driver2);
            loginPage2.Navigate();
            loginPage2.PerformLogin("testdoctor@example.com", "password");
            dashboardPage.ClickAppointments();
            wait.Until(d => calendarPage.IsCalendarVisible());
            LogSuccess(3, "Browser 2 reached agenda.");

            // Stap 4: Concurrency voorbereiden
            LogStep(4, "Preparing unavailability modals in both browsers...");
            IJavaScriptExecutor js1 = (IJavaScriptExecutor)driver1;
            IJavaScriptExecutor js2 = (IJavaScriptExecutor)driver2;

            var btn1 = driver1.FindElement(By.CssSelector(".quick-actions button.btn-primary"));
            var btn2 = driver2.FindElement(By.CssSelector(".quick-actions button.btn-primary"));

            js1.ExecuteScript("arguments[0].click();", btn1);
            js2.ExecuteScript("arguments[0].click();", btn2);
            LogSuccess(4, "Both modals opened concurrently.");

            // Stap 5: Data invoeren (Patient)
            LogStep(5, "Browser 1: Entering unavailability data...");
            wait1.Until(d => d.FindElement(By.Id("unavail-start")).Displayed);
            var slot1 = driver1.FindElements(By.CssSelector(".time-slot:not(.unavailable)")).First();
            string start1 = $"{slot1.GetAttribute("data-date")}T{slot1.GetAttribute("data-hour").PadLeft(2, '0')}:00";

            js1.ExecuteScript("arguments[0].value = arguments[1]; arguments[0].dispatchEvent(new Event('input'));",
                driver1.FindElement(By.Id("unavail-start")), start1);
            driver1.FindElement(By.Id("unavail-reason")).SendKeys("Patient Concurrency Test");
            LogInfo($"Patient selected slot: {start1}");

            // Stap 6: Data invoeren (Doctor)
            LogStep(6, "Browser 2: Entering unavailability data...");
            wait2.Until(d => d.FindElement(By.Id("unavail-start")).Displayed);
            var slot2 = driver2.FindElements(By.CssSelector(".time-slot:not(.unavailable)")).First();
            string start2 = $"{slot2.GetAttribute("data-date")}T{slot2.GetAttribute("data-hour").PadLeft(2, '0')}:00";

            js2.ExecuteScript("arguments[0].value = arguments[1]; arguments[0].dispatchEvent(new Event('input'));",
                driver2.FindElement(By.Id("unavail-start")), start2);
            driver2.FindElement(By.Id("unavail-reason")).SendKeys("Doctor Concurrency Test");
            LogInfo($"Doctor selected slot: {start2}");

            // Stap 7: Gelijktijdige submit
            LogStep(7, "Executing concurrent submission...");
            var submit1 = driver1.FindElement(By.CssSelector(".modal button[type='submit']"));
            var submit2 = driver2.FindElement(By.CssSelector(".modal button[type='submit']"));

            var task1 = System.Threading.Tasks.Task.Run(() => js1.ExecuteScript("arguments[0].click();", submit1));
            var task2 = System.Threading.Tasks.Task.Run(() => js2.ExecuteScript("arguments[0].click();", submit2));
            System.Threading.Tasks.Task.WaitAll(task1, task2);
            LogSuccess(7, "Both requests sent to server simultaneously.");

            // Stap 8: Verificatie
            LogStep(8, "Verifying persistence for both users...");
            System.Threading.Thread.Sleep(3000); // Wait for sync

            var finalSlot1 = driver1.FindElement(By.CssSelector($".time-slot[data-date='{slot1.GetAttribute("data-date")}']"));
            var finalSlot2 = driver2.FindElement(By.CssSelector($".time-slot[data-date='{slot2.GetAttribute("data-date")}']"));

            Assert.IsTrue(finalSlot1.GetAttribute("class").Contains("unavailable"), "Patient update failed.");
            Assert.IsTrue(finalSlot2.GetAttribute("class").Contains("unavailable"), "Doctor update failed.");

            LogInfo("✓ Patient calendar updated correctly.");
            LogInfo("✓ Doctor calendar updated correctly.");
            LogSuccess(8, "No race conditions detected. Both updates persisted.");
        }
    }
}