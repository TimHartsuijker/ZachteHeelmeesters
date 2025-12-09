using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace SeleniumTests;

[TestClass]
public class _2_30_1
{
	[TestMethod]
	public void Dashboard_DisplaysWelcomeMessage()
	{
		// Arrange
		IWebDriver driver = null;

		try
		{
			Console.WriteLine("LOG [Step 1] Start test: Dashboard_DisplaysWelcomeMessage");

			// Create a new Chrome browser instance
			driver = new ChromeDriver();
			var loginPage = new SeleniumTests.Pages.LoginPage(driver);

			// Step 1: Go to the login page
			loginPage.Navigate();
			Console.WriteLine("LOG [Step 1] Navigated to login page.");

			// Step 2: Enter valid login credentials for a patient
			loginPage.EnterEmail("patient@example.com");
			loginPage.EnterPassword("Test123!");
			Console.WriteLine("LOG [Step 2] Entered patient credentials.");

			// Step 3: Click the login button
			loginPage.ClickLogin();
			Console.WriteLine("LOG [Step 3] Clicked login button.");

			// Step 4: Wait until the dashboard is loaded
			// EXAMPLE SELECTOR – Adjust if your dashboard has a different identifier
			var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
			wait.Until(driver => driver.FindElement(By.Id("dashboard-container")));
			Console.WriteLine("LOG [Step 4] Dashboard screen loaded.");

			// Step 5: Check if a welcome message is displayed
			// EXAMPLE: looking for an element containing a welcome text
			// Replace selector with the actual one from your application
			var welcomeMessage = driver.FindElement(By.XPath("//*[contains(text(),'Welkom')]"));

			if (welcomeMessage.Displayed)
			{
				Console.WriteLine("LOG [Step 5] PASS: Welcome message is visible on the dashboard.");
			}
			else
			{
				Console.WriteLine("LOG [Step 5] FAIL: Welcome message not visible.");
				throw new Exception("Welcome message not displayed.");
			}

			// Step 6: Test completed
			Console.WriteLine("LOG [Step 6] Test completed successfully.");
		}
		catch (NoSuchElementException ex)
		{
			Console.WriteLine("ERROR [E001] Element not found: " + ex.Message);
			throw;
		}
		catch (Exception ex)
		{
			Console.WriteLine("ERROR [E999] Unexpected error: " + ex.ToString());
			throw;
		}
		finally
		{
			if (driver != null)
			{
				driver.Quit();
				Console.WriteLine("LOG WebDriver closed.");
			}
		}
	}
}
