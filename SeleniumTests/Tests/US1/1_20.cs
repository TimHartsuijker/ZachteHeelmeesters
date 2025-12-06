using OpenQA.Selenium;
using OpenQA.Selenium.BiDi.BrowsingContext;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Chrome;
using SeleniumTests.P_O_M;

namespace SeleniumTests.Tests.US1
{
    [TestClass]
    public class LogoutTests
    {
        private IWebDriver _driver;
        private LoginPage _loginPage;
        private WebDriverWait wait;

        private const string VALID_EMAIL = "gebruiker@example.com";
        private const string VALID_PASSWORD = "Wachtwoord123";

        [TestInitialize]
        public void Setup()
        {
            _driver = new ChromeDriver();
            _driver.Manage().Window.Maximize();
            _loginPage = new LoginPage(_driver);
            _loginPage.Navigate();

            wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        }

        [TestMethod]
        public void TC_1_20_1_LogoutButtonIsAvailable()
        {
            // Login
            DashboardPage dashboardPage = _loginPage.PerformLogin(VALID_EMAIL, VALID_PASSWORD);

            wait.Until(d => dashboardPage.IsDashboardDisplayed());

            // ASSERT: Logout button available
            Assert.IsTrue(dashboardPage.IsLogoutButtonDisplayed(), "ERROR: The logout button is not available on the Dashboardpage");
        }

        [TestMethod]
        public void TC_1_20_2_NoAccessViaRouteAfterLoggingOut()
        {
            // Login
            DashboardPage dashboardPage = _loginPage.PerformLogin(VALID_EMAIL, VALID_PASSWORD);

            wait.Until(d => dashboardPage.IsDashboardDisplayed());

            // Logout
            Console.WriteLine("LOG: Clicking the logout button");
            dashboardPage.ClickLogout();

            Console.WriteLine("LOG: Trying to navigate to the dashboard via routing");
            var testDashboard = new DashboardPage(_driver);
            testDashboard.NavigateToDashboard();

            // ASSERT: No access to Dashboard via routing
            Assert.IsFalse(testDashboard.IsDashboardDisplayed(), "ERROR: Dashboard was accessible via routing");
        }

        [TestMethod]
        public void TC_1_20_3_NoAccessNavigatingBackAfterLoggingOut()
        {
            // Login
            DashboardPage dashboardPage = _loginPage.PerformLogin(VALID_EMAIL, VALID_PASSWORD);

            wait.Until(d => dashboardPage.IsDashboardDisplayed());

            // Logout
            Console.WriteLine("LOG: Clicking the logout button");
            dashboardPage.ClickLogout();

            // Navigate back
            Console.WriteLine("LOG: Trying to navigate back");
            _driver.Navigate().Back();

            var currentDashboardState = new DashboardPage(_driver);

            // ASSERT: Dashboard is not displayed after navigating back
            Assert.IsFalse(currentDashboardState.IsDashboardDisplayed(),
                "ERROR: Dashboard was accessible via navigating back");
        }

        [TestMethod]
        public void TC_1_20_4_UserRedirectedToLoginPageAfterLoggingOut()
        {
            // Login
            DashboardPage dashboardPage = _loginPage.PerformLogin(VALID_EMAIL, VALID_PASSWORD);

            wait.Until(d => dashboardPage.IsDashboardDisplayed());

            // Logout
            Console.WriteLine("LOG: Clicking the logout button");
            dashboardPage.ClickLogout();

            // ASSERT: User redirected to Loginpage
            Assert.AreEqual(LoginPage.Url, _driver.Url, "ERROR: After logging out the user is not redirected to the Login page");
        }

        [TestCleanup]
        public void Teardown()
        {
            try
            {
                _driver?.Quit();
                _driver?.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during teardown: {ex}");
            }
            finally
            {
                _driver = null!;
            }
        }
    }
}