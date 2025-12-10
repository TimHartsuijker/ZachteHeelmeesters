using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumTests.P_O_M;
using System;

namespace SeleniumTests
{
    [TestClass]
    public class _1_19_6
    {
        private IWebDriver driver;
        private WebDriverWait wait;
        private string baseUrl = "https://localhost:5173";
        private LoginPage loginPage;

        [TestInitialize]
        public void Setup()
        {
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            options.AddArgument("--ignore-certificate-errors");

            driver = new ChromeDriver(options);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            loginPage = new LoginPage(driver);
        }

        [TestCleanup]
        public void TearDown()
        {
            driver.Quit();
            driver.Dispose();
        }

        public void Login(string email, string password)
        {
            loginPage.EnterEmail(email);
            loginPage.EnterPassword(password);
            loginPage.ClickLogin();
        }

        public static async Task FixedWait15Minutes()
        {
            Console.WriteLine("Starting 15-minute non-blocking wait...");

            // Pass the duration as a TimeSpan
            await Task.Delay(TimeSpan.FromMinutes(15));

            Console.WriteLine("Non-blocking wait complete.");
        }

        // Try again after wrong input
        [TestMethod]
        public void TC_1_19_6_CanTryAgainAfterWrongInput()
        {
            string WRONG_EMAIL = "gebruikerexample.com";
            string RIGHT_EMAIL = "gebruiker@example.com";
            string PASSWORD = "Wachtwoord123";

            Console.WriteLine($"LOG Start Testcase 1_19_6_HappyFlow");

            // Navigate to login page
            driver.Navigate().GoToUrl($"{baseUrl}/");
            Console.WriteLine($"LOG [Step 1] Navigated to the login page");

            // Enter wrong credentials page
            Login(WRONG_EMAIL, PASSWORD);

            Console.WriteLine($"LOG [Step 2/3] Entered wrong credentials and clicked login");

            // Waiting until error message is displayed
            wait.Until(d => d.FindElement(By.Id("login-error")));

            // Enter right credentials and try again
            Login(RIGHT_EMAIL, PASSWORD);

            Console.WriteLine($"LOG [Step 4/5] Entered right credentials and clicked login");

            wait.Until(d => d.Url.Contains("/dashboard"));

            Assert.IsTrue(driver.Url.Contains("/dashboard"), "Gebruiker is niet doorgestuurd naar het dashboard.");
        }

        // User gets block message after three wrong attempts
        [TestMethod]
        public void TC_1_19_6_BlockMessageIsDisplayedAfterThreeAttempts()
        {
            string EMAIL = "verkeerd@example.com";
            string PASSWORD = "Fout123";

            Console.WriteLine($"LOG Start Testcase 1_19_6_Blockmessage");

            // Navigate to login page
            driver.Navigate().GoToUrl($"{baseUrl}/");
            Console.WriteLine($"LOG [Step 1] Navigated to the login page");

            // Enter wrong credentials 3 times
            Login(EMAIL, PASSWORD);
            Login(EMAIL, PASSWORD);
            Login(EMAIL, PASSWORD);
            Console.WriteLine($"LOG [Step 2/3/4] Tried to login with the wrong credentials 3 times");

            // Error message is displayed
            var error = wait.Until(d => d.FindElement(By.Id("login-error")));
            Assert.AreEqual("Uw account is 15 minuten geblokkeerd.", error.Text);
        }

        // User cannot login after three wrong attempts
        [TestMethod]
        public void TC_1_19_6_UserIsBlockedAfterThreeAttempts()
        {
            string WRONG_EMAIL = "verkeerd@example.com";
            string WRONG_PASSWORD = "Fout123";
            string RIGHT_EMAIL = "gebruiker@example.com";
            string RIGHT_PASSWORD = "Wachtwoord123";

            Console.WriteLine($"LOG Start Testcase 1_19_6_UserIsBlocked");

            // Navigate to login page
            driver.Navigate().GoToUrl($"{baseUrl}/");
            Console.WriteLine($"LOG [Step 1] Navigated to the login page");

            // Enter wrong credentials 3 times
            Login(WRONG_EMAIL, WRONG_PASSWORD);
            Login(WRONG_EMAIL, WRONG_PASSWORD);
            Login(WRONG_EMAIL, WRONG_PASSWORD);
            Console.WriteLine($"LOG [Step 2/3/4] Tried to login with the wrong credentials 3 times");

            // Enter right credentials after being blocked
            Login(RIGHT_EMAIL, RIGHT_PASSWORD);
            Console.WriteLine($"LOG [Step 5] Entered the right credentials and clicked login");

            // Verify user remains blocked: blocked message shown and no dashboard redirect
            var error = wait.Until(d => d.FindElement(By.Id("login-error")));
            Assert.IsFalse(driver.Url.Contains("/dashboard"), "Gebruiker is doorgestuurd naar het dashboard terwijl het account geblokkeerd zou moeten zijn.");
        }

        // User can login after being blocked for 15 minutes
        [TestMethod]
        public void TC_1_19_6_LoginAfterBlock()
        {
            string WRONG_EMAIL = "verkeerd@example.com";
            string WRONG_PASSWORD = "Fout123";
            string RIGHT_EMAIL = "gebruiker@example.com";
            string RIGHT_PASSWORD = "Wachtwoord123";

            Console.WriteLine($"LOG Start Testcase 1_19_6_UserIsBlocked");

            // Navigate to login page
            driver.Navigate().GoToUrl($"{baseUrl}/");
            Console.WriteLine($"LOG [Step 1] Navigated to the login page");

            // Enter wrong credentials 3 times 1x
            Login(WRONG_EMAIL, WRONG_PASSWORD);
            Login(WRONG_EMAIL, WRONG_PASSWORD);
            Login(WRONG_EMAIL, WRONG_PASSWORD);
            Console.WriteLine($"LOG [Step 2] Tried to login with the wrong credentials 3 times");

            FixedWait15Minutes().GetAwaiter().GetResult();

            // Enter right credentials after being blocked
            Login(RIGHT_EMAIL, RIGHT_PASSWORD);
            Console.WriteLine($"LOG [Step 3] Entered the right credentials and clicked login");

            wait.Until(d => d.Url.Contains("/dashboard"));
            Assert.IsTrue(driver.Url.Contains("/dashboard"), "Gebruiker is niet doorgestuurd naar het dashboard nadat het account geblokkeerd is geweest");
        }

        // User cannot login after three blocks
        [TestMethod]
        public void TC_1_19_6_SpecialBlockMessage()
        {
            string WRONG_EMAIL = "verkeerd@example.com";
            string WRONG_PASSWORD = "Fout123";
            string RIGHT_EMAIL = "gebruiker@example.com";
            string RIGHT_PASSWORD = "Wachtwoord123";

            Console.WriteLine($"LOG Start Testcase 1_19_6_UserIsBlocked");

            // Navigate to login page
            driver.Navigate().GoToUrl($"{baseUrl}/");
            Console.WriteLine($"LOG [Step 1] Navigated to the login page");

            // Enter wrong credentials 3 times 1x
            Login(WRONG_EMAIL, WRONG_PASSWORD);
            Login(WRONG_EMAIL, WRONG_PASSWORD);
            Login(WRONG_EMAIL, WRONG_PASSWORD);
            Console.WriteLine($"LOG [Step 2] Tried to login with the wrong credentials 3 times 1x");

            FixedWait15Minutes().GetAwaiter().GetResult();

            // Enter wrong credentials 3 times 2x
            Login(WRONG_EMAIL, WRONG_PASSWORD);
            Login(WRONG_EMAIL, WRONG_PASSWORD);
            Login(WRONG_EMAIL, WRONG_PASSWORD);
            Console.WriteLine($"LOG [Step 3] Tried to login with the wrong credentials 3 times 2x");

            FixedWait15Minutes().GetAwaiter().GetResult();

            // Enter wrong credentials 3 times 3x
            Login(WRONG_EMAIL, WRONG_PASSWORD);
            Login(WRONG_EMAIL, WRONG_PASSWORD);
            Login(WRONG_EMAIL, WRONG_PASSWORD);
            Console.WriteLine($"LOG [Step 4] Tried to login with the wrong credentials 3 times 3x");
            
            FixedWait15Minutes().GetAwaiter().GetResult();

            // Enter right credentials after being blocked
            Login(RIGHT_EMAIL, RIGHT_PASSWORD);
            Console.WriteLine($"LOG [Step 5] Entered the right credentials and clicked login");

            // Error message is displayed
            var error = wait.Until(d => d.FindElement(By.Id("login-error")));
            Assert.AreEqual("Uw account is geblokkeerd. Controleer uw e-mail om uw account te deblokkeren.", error.Text);
            Assert.IsFalse(driver.Url.Contains("/dashboard"), "Gebruiker is doorgestuurd naar het dashboard terwijl het account geblokkeerd zou moeten zijn.");
        }
    }
}
