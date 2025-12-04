using OpenQA.Selenium;
using OpenQA.Selenium.BiDi.BrowsingContext;
using OpenQA.Selenium.Chrome;
using SeleniumTests.Pages;

namespace SeleniumTests.Tests.US1
{
    [TestClass]
    public class LogoutTests : IDisposable
    {
        private IWebDriver _driver;
        private LoginPage _loginPage;

        private const string VALID_EMAIL = "gebruiker@example.com";
        private const string VALID_PASSWORD = "Wachtwoord123";

        [TestInitialize]
        public void Setup()
        {
            _driver = new ChromeDriver();
            _driver.Manage().Window.Maximize();
            _loginPage = new LoginPage(_driver);
            _loginPage.Navigate();
        }

        [TestMethod]
        public void TC_1_20_1_LogoutButtonIsAvailable()
        {
            // Login
            DashboardPage dashboardPage = _loginPage.PerformLogin(VALID_EMAIL, VALID_PASSWORD);

            // ASSERT 1: Successful login
            Assert.IsTrue(dashboardPage.IsDashboardDisplayed(), "ERROR: Dashboard is not displayed");

            // ASSERT 2: Logout button available
            Assert.IsTrue(dashboardPage.IsLogoutButtonDisplayed(), "ERROR: The logout button is not available on the Dashboardpage");
        }

        [TestMethod]
        public void TC_1_20_2_NoAccessViaRouteAfterLoggingOut()
        {
            // Login
            DashboardPage dashboardPage = _loginPage.PerformLogin(VALID_EMAIL, VALID_PASSWORD);

            // ASSERT 1: Dashboard displayed
            Assert.IsTrue(dashboardPage.IsDashboardDisplayed(), "ERROR: Dashboard is not displayed");

            // Logout
            dashboardPage.ClickLogout();

            var testDashboard = new DashboardPage(_driver);
            testDashboard.NavigateToDashboard();

            // ASSERT 3: No access to Dashboard via routing
            Assert.IsFalse(testDashboard.IsDashboardDisplayed(), "ERROR: Dashboard was accessible via routing");
        }

        [TestMethod]
        public void TC_1_20_3_NoAccessNavigatingBackAfterLoggingOut()
        {
            // Login
            DashboardPage dashboardPage = _loginPage.PerformLogin(VALID_EMAIL, VALID_PASSWORD);

            // ASSERT 1: Dashboard displayed
            Assert.IsTrue(dashboardPage.IsDashboardDisplayed(), "ERROR: Dashboard is not displayed");

            // Logout
            dashboardPage.ClickLogout();

            // Navigate back
            _driver.Navigate().Back();

            var currentDashboardState = new DashboardPage(_driver);

            // ASSERT 2: Dashboard is not displayed after navigating back
            Assert.IsFalse(currentDashboardState.IsDashboardDisplayed(),
                "ERROR: Dashboard was accessible via navigating back");
        }

        [TestMethod]
        public void TC_1_20_4_UserRedirectedToLoginPageAfterLoggingOut()
        {
            // Login
            DashboardPage dashboardPage = _loginPage.PerformLogin(VALID_EMAIL, VALID_PASSWORD);

            // ASSERT 1: Dashboard displayed
            Assert.IsTrue(dashboardPage.IsDashboardDisplayed(), "ERROR: Dashboard is not displayed");

            // Logout
            dashboardPage.ClickLogout();

            // ASSERT 2: User redirected to Loginpage
            Assert.AreEqual(_loginPage.Url, _driver.Url, "ERROR: After login out the user is not redirected to the Loginpage");
        }

        [TestCleanup]
        public void Teardown()
        {
            _driver.Quit();
        }

        public void Dispose()
        {
            Teardown();
        }
    }
}