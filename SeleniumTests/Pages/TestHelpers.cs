using OpenQA.Selenium;
using System.Threading;

namespace SeleniumTests.Pages
{
    /// <summary>
    /// Helper class for common test operations
    /// </summary>
    public class TestHelpers(IWebDriver driver, LoginPage loginPage, DossierPage dossierPage)
    {
        private readonly IWebDriver driver = driver;
        private readonly LoginPage loginPage = loginPage;
        private readonly DossierPage dossierPage = dossierPage;

        /// <summary>
        /// Complete patient login flow: Navigate -> Login -> Go to Dossier
        /// </summary>
        public void LoginAndNavigateToDossier(string email, string password)
        {
            // Navigate to login page
            loginPage.Navigate();

            // Login
            loginPage.PerformLogin(email, password);

            // Wait for redirect after login
            Thread.Sleep(1000);

            // Navigate to dossier
            dossierPage.Navigate();

            // Wait for dossier to load
            Thread.Sleep(2000);
        }

        /// <summary>
        /// Verify that the current page is the dossier page
        /// </summary>
        public bool IsOnDossierPage()
        {
            return driver.Url.Contains("/dossier");
        }

        /// <summary>
        /// Check if loading spinner is present
        /// </summary>
        public bool IsLoadingSpinnerPresent()
        {
            var spinners = driver.FindElements(By.CssSelector(".animate-spin"));
            return spinners.Count > 0;
        }

        /// <summary>
        /// Wait for loading spinner to disappear
        /// </summary>
        public void WaitForLoadingToComplete(int timeoutSeconds = 10)
        {
            var maxWait = timeoutSeconds * 10; // Check every 100ms
            var waited = 0;

            while (IsLoadingSpinnerPresent() && waited < maxWait)
            {
                Thread.Sleep(100);
                waited++;
            }
        }

        /// <summary>
        /// Get the current user's name from sessionStorage (via JS)
        /// </summary>
        public string GetSessionUserId()
        {
            var js = (IJavaScriptExecutor)driver;
            var userId = js.ExecuteScript("return sessionStorage.getItem('userId');");
            return userId?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Scroll to bottom of page
        /// </summary>
        public void ScrollToBottom()
        {
            var js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
            Thread.Sleep(500); // Wait for scroll animation
        }

        /// <summary>
        /// Scroll to top of page
        /// </summary>
        public void ScrollToTop()
        {
            var js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("window.scrollTo(0, 0);");
            Thread.Sleep(500);
        }
    }
}
