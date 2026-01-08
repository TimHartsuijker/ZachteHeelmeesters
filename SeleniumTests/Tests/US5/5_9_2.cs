using OpenQA.Selenium;
using System;

public class CreateReferral_NoTreatmentTest : TestBase
{
    public static void RunStandalone()
    {
        var test = new CreateReferral_NoTreatmentTest();

        try
        {
            test.Setup();
            test.RunTest();
            Console.WriteLine("✅ NO TREATMENT TEST PASSED");
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ NO TREATMENT TEST FAILED");
            Console.WriteLine(ex.Message);
        }
        finally
        {
            test.TearDown();
        }
    }

    public void RunTest()
    {
        // Select patient only
        WaitForElement(By.CssSelector("[data-testid='patient-dropdown']")).Click();
        WaitForElement(By.CssSelector("[data-testid='patient-john-doe']")).Click();

        // Add note
        WaitForElement(By.CssSelector("[data-testid='referral-note']"))
            .SendKeys("No treatment selected");

        // Submit
        WaitForElement(By.CssSelector("[data-testid='submit-referral']")).Click();

        // Expect error
        ExpectError("behandelcode");
    }
}