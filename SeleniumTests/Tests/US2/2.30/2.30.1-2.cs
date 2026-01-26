using OpenQA.Selenium;
using SeleniumTests.Base;

namespace US2._30
{
    [TestClass]
    public class _2_30_1_2: BaseTest
    {
        [TestMethod]
        public void TC_2_30_1_2_Dashboard_PersonalizedWelcomeMessage()
        {
            Console.WriteLine("Test started: TC_2_30_1_2_Dashboard_PersonalizedWelcomeMessage");

            // Step 1: Navigate to login page
            Console.WriteLine("Step 1: Navigating to login page...");
            driver.Navigate().GoToUrl($"{baseUrl}/login");
            Console.WriteLine("Navigation completed!");

            // Step 2: Enter valid login credentials (Test Gebruiker from DbSeederTest)
            Console.WriteLine("Step 2: Entering login credentials...");
            var emailInput = driver.FindElement(By.Id("email"));
            emailInput.SendKeys("gebruiker@example.com");

            var passwordInput = driver.FindElement(By.Id("wachtwoord"));
            passwordInput.SendKeys("Wachtwoord123");
            Console.WriteLine("Login credentials entered!");

            // Step 3: Click login
            Console.WriteLine("Step 3: Clicking login...");
            var loginButton = driver.FindElement(By.Id("login-btn"));
            loginButton.Click();
            Console.WriteLine("Login submitted!");

            // Step 4: Wait until dashboard is loaded
            Console.WriteLine("Step 4: Waiting for dashboard...");
            wait.Until(d => d.FindElement(By.CssSelector("[data-test='welcome-message']")));
            Console.WriteLine("Dashboard successfully loaded!");

            // Step 5: Verify personalized welcome message
            Console.WriteLine("Step 5: Verifying welcome message...");

            var welcomeMessage = wait.Until(d =>
                d.FindElement(By.CssSelector("[data-test='welcome-message']")));

            string expectedName = "Test Gebruiker";

            Assert.IsTrue(welcomeMessage.Text.Contains(expectedName),
                $"Welcome message does not contain the name '{expectedName}'. " +
                $"Welcome message text: '{welcomeMessage.Text}'");

            Console.WriteLine($"Welcome message contains correct name! Text: {welcomeMessage.Text}");
            Console.WriteLine("Test completed successfully.");
        }
    }
}
