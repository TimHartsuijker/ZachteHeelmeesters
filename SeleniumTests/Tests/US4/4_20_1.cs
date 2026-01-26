using OpenQA.Selenium;
using SeleniumTests.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace US4._20
{
    [TestClass]
    public class _4_20_1 : BaseTest
    {
        [TestMethod]
        public void TC_4_20_1_RetrieveAvailableTimeSlots_ByUserID()
        {
            string testEmail = "gebruiker@example.com";
            string testPassword = "Wachtwoord123";
            string expectedUserName = "Test Gebruiker";

            // Stap 1: Navigatie naar Login
            LogStep(1, "Navigating to login page...");
            loginPage.Navigate();
            LogSuccess(1, "Login page loaded successfully.");

            // Stap 2: Inloggen
            LogStep(2, $"Logging in with email: {testEmail}");
            loginPage.PerformLogin(testEmail, testPassword);
            LogSuccess(2, "Login credentials submitted.");

            // Stap 3: Redirect naar Agenda via POM
            LogStep(3, "Navigating to the agenda page...");
            dashboardPage.ClickAppointments();

            // Gebruik de wait uit BaseTest om URL te controleren
            wait.Until(d => calendarPage.IsCalendarVisible());
            LogSuccess(3, $"Successfully reached agenda: {driver.Url}");

            // Stap 4: Kalender en Gebruikersinformatie laden via POM
            LogStep(4, "Waiting for calendar container to load...");
            calendarPage.WaitForCalendarLoad();
            LogSuccess(4, "Calendar page content is visible.");

            // Stap 5: Gebruikersverificatie via POM
            LogStep(5, "Verifying displayed user information...");
            string displayedUserInfo = calendarPage.GetUserInfo();

            // We gebruiken StringAssert voor betere foutmeldingen
            StringAssert.Contains(displayedUserInfo, expectedUserName,
                $"Wrong user name displayed. Got: '{displayedUserInfo}'");
            StringAssert.Contains(displayedUserInfo, testEmail,
                $"Wrong email displayed. Got: '{displayedUserInfo}'");

            LogInfo($"Display verified: {displayedUserInfo}");
            LogSuccess(5, "Correct user information (ID context) is displayed.");

            // Stap 6: Beschikbare tijdsloten ophalen via POM
            LogStep(6, "Retrieving available time slots from the calendar grid...");

            RetryVerification(() =>
            {
                int totalSlots = calendarPage.GetTotalTimeSlotsCount();
                Assert.IsTrue(totalSlots > 0, "No time slots found in the calendar grid.");
                LogInfo($"Number of time slots found: {totalSlots}");
            });

            LogSuccess(6, "Calendar time slots retrieved successfully.");

            // Stap 7: Controleer of de juiste dokter-ID is gekoppeld (Context check)
            LogStep(7, "Verifying doctor context via data-attributes...");
            string doctorId = calendarPage.GetDoctorId();
            Assert.IsFalse(string.IsNullOrEmpty(doctorId), "Doctor ID attribute is missing from calendar.");
            LogInfo($"Calendar is bound to Doctor ID: {doctorId}");
            LogSuccess(7, "Doctor context verified.");

            // Stap 8: Finale conclusie
            LogStep(8, "Final verification of retrieved data...");
            LogInfo("✓ User context is correctly applied to the agenda");
            LogInfo("✓ Available time slots are loaded and visible in the grid");
        }
    }
}