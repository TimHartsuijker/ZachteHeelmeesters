using OpenQA.Selenium;
using System;

public class CreateReferral_NoPatientTest : TestBase
{
    public static void RunStandalone()
    {
        var test = new CreateReferral_NoPatientTest();

        try
        {
            test.Setup();
            test.RunTest();
            Console.WriteLine("✅ NO PATIENT TEST PASSED");
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ NO PATIENT TEST FAILED");
            Console.WriteLine(ex.Message);
        }
        finally
        {
            test.TearDown();
        }
    }

    public void RunTest()
    {
        // Select treatment only
        WaitForElement(By.CssSelector("[data-testid='treatment-dropdown']")).Click();
        WaitForElement(By.CssSelector("[data-testid='treatment-physio']")).Click();

        // Add note
        WaitForElement(By.CssSelector("[data-testid='referral-note']"))
            .SendKeys("No patient selected");

        // Submit
        WaitForElement(By.CssSelector("[data-testid='submit-referral']")).Click();

        // Expect error
        ExpectError("patiënt");
    }
}