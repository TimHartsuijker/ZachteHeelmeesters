using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Threading;

public abstract class TestBase
{
    protected IWebDriver driver;

    protected void Setup()
    {
        driver = new ChromeDriver();
        driver.Manage().Window.Maximize();
        driver.Navigate().GoToUrl("http://localhost:5173/referrals/create");
    }

    protected void TearDown()
    {
        driver.Quit();
    }

    protected IWebElement WaitForElement(By by, int timeoutSeconds = 10)
    {
        for (int i = 0; i < timeoutSeconds * 10; i++)
        {
            try
            {
                var element = driver.FindElement(by);
                if (element.Displayed && element.Enabled)
                    return element;
            }
            catch (NoSuchElementException) { }

            Thread.Sleep(100);
        }

        throw new Exception($"Element not found or not clickable: {by}");
    }

    protected void ExpectError(string expectedMessage)
    {
        var error = WaitForElement(By.CssSelector("[data-testid='form-error']"));

        if (!error.Text.ToLower().Contains(expectedMessage.ToLower()))
            throw new Exception($"Expected error not shown: {expectedMessage}");
    }
}