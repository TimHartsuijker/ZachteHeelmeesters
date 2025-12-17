using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

internal class CreateReferralTest
{
    private static IWebDriver driver;

    // Rename Main to Run to avoid multiple entry points
    public static void Run(string[] args)
    {
        try
        {
            Setup();
            RunTest();
            Console.WriteLine("TEST PASSED");
        }
        catch (Exception ex)
        {
            Console.WriteLine("TEST FAILED");
            Console.WriteLine(ex.Message);
        }
        finally
        {
            TearDown();
        }
    }

    static void Setup()
    {
        driver = new ChromeDriver();
        driver.Manage().Window.Maximize();
        driver.Navigate().GoToUrl("http://localhost:5173/referrals/create");
    }

    static void RunTest()
    {
        //1️ Select treatment
        WaitForElement(By.CssSelector("[data-testid='treatment-dropdown']")).Click();
        WaitForElement(By.CssSelector("[data-testid='treatment-physio']")).Click();

        //2️ Select patient
        WaitForElement(By.CssSelector("[data-testid='patient-dropdown']")).Click();
        WaitForElement(By.CssSelector("[data-testid='patient-john-doe']")).Click();

        //3️ Add note
        var note = WaitForElement(By.CssSelector("[data-testid='referral-note']"));
        note.SendKeys("Referral created during automated test");

        //4️ Submit referral
        WaitForElement(By.CssSelector("[data-testid='submit-referral']")).Click();

        //5️ Verify referral saved
        var successMessage = WaitForElement(By.CssSelector("[data-testid='referral-success']"));
        if (!successMessage.Text.Contains("success"))
            throw new Exception("Referral was not saved successfully.");

        //6️ Verify patient notification
        var notification = WaitForElement(By.CssSelector("[data-testid='patient-notification']"));
        if (!notification.Text.Contains("new referral"))
            throw new Exception("Patient notification was not shown.");
    }

    static void TearDown()
    {
        driver.Quit();
    }

    //Manual wait (polling)
    static IWebElement WaitForElement(By by, int timeoutSeconds = 10)
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
}