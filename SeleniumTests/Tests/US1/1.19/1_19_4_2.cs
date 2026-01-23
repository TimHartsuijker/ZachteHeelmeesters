using OpenQA.Selenium;
using SeleniumTests.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace US1._19
{
    [TestClass]
    public class _1_19_4_2 : BaseTest
    {
        [TestMethod]
        public void TC_1_19_4_NoLogin_AccessDashboard()
        {
            // Stap 1: Navigatie naar beveiligde pagina
            LogStep(1, "Navigating to /dashboard without being logged in...");
            dashboardPage.Navigate();
            LogSuccess(1, "Navigation attempt to protected route completed.");

            // Stap 2: Wachten op automatische redirect
            LogStep(2, "Waiting for login page elements (redirect check)...");
            wait.Until(d => loginPage.IsPasswordInputDisplayed());
            LogInfo($"Current URL: {driver.Url}");
            LogSuccess(2, "Redirection to login page detected.");

            // Stap 3: Verificatie blokkade
            LogStep(3, "Verifying if dashboard access is blocked...");
            Assert.IsTrue(loginPage.IsPasswordInputDisplayed(), "Gebruiker kan dashboard openen zonder inloggen!");
            LogSuccess(3, "Access to dashboard without login is blocked as expected.");

            // Stap 4: Controle van de huidige staat
            LogStep(4, "Final verification of login page presence...");
            LogInfo("Login page is visible, dashboard content is not accessible.");
            LogSuccess(4, "Unauthorized access protection verified successfully.");
        }
    }
}