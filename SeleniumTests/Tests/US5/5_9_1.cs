using OpenQA.Selenium;
using System;

public class CreateReferral_SuccessTest : TestBase
{
    public static void RunStandalone()
    {
        var test = new CreateReferral_SuccessTest();

        try
        {
            test.Setup();
            test.RunTest();
            Console.WriteLine("✅ SUCCESS TEST PASSED");
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ SUCCESS TEST FAILED");
            Console.WriteLine(ex.Message);
        }
        finally
        {
            test.TearDown();
        }
    }

    public void RunTest()
    {
        // Select treatment
        WaitForElement(By.CssSelector("[data-testid='treatment-dropdown']")).Click();
        WaitForElement(By.CssSelector("[data-testid='treatment-physio']")).Click();

        // Select patient
        WaitForElement(By.CssSelector("[data-testid='patient-dropdown']")).Click();
        WaitForElement(By.CssSelector("[data-testid='patient-john-doe']")).Click();

        // Add note
        WaitForElement(By.CssSelector("[data-testid='referral-note']"))
            .SendKeys("Automated success test");

        // Submit
        WaitForElement(By.CssSelector("[data-testid='submit-referral']")).Click();

        // Verify success
        var success = WaitForElement(By.CssSelector("[data-testid='referral-success']"));
        if (!success.Text.Contains("success"))
            throw new Exception("Referral was not created.");

        // Verify notification
        var notification = WaitForElement(By.CssSelector("[data-testid='patient-notification']"));
        if (!notification.Text.Contains("referral"))
            throw new Exception("Patient notification missing.");
    }
}