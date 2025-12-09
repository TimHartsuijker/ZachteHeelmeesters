using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace SeleniumTests;

[TestClass]
public class _2_30_2_1
{
    [TestMethod]
    public void NavigationMenu_VisibleAfterLogin()
    {
        IWebDriver driver = null;

        try
        {
            Console.WriteLine("LOG Start test: Navigation menu visible after login");

            driver = new ChromeDriver();
            var loginPage = new SeleniumTests.Pages.LoginPage(driver);

            // Step 1: Navigate to login page
            loginPage.Navigate();

            // Step 2: Enter valid patient login
            loginPage.EnterEmail("patient@example.com");
            loginPage.EnterPassword("Test123!");

            // Step 3: Click Login
            loginPage.ClickLogin();

            // Step 4: Wait until dashboard loads
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(d => d.FindElement(By.Id("dashboard-container")));

            // Step 5: Check if navigation menu is visible
            var navMenu = driver.FindElement(By.Id("navigation-menu")); // Adjust ID to your app

            if (!navMenu.Displayed)
                throw new Exception("Navigation menu is NOT visible.");

            Console.WriteLine("PASS: Navigation menu is visible after login.");
        }
        finally
        {
            driver?.Quit();
        }
    }
}
