using OpenQA.Selenium;
using System;

public class CreateReferral_NoTreatmentAndNoPatientTest : TestBase
{
    public static void RunStandalone()
    {
        var test = new CreateReferral_NoTreatmentAndNoPatientTest();

        try
        {
            test.Setup();
            test.RunTest();
            Console.WriteLine("✅ NO TREATMENT & NO PATIENT TEST PASSED");
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ NO TREATMENT & NO PATIENT TEST FAILED");
            Console.WriteLine(ex.Message);
        }
        finally
        {
            test.TearDown();
        }
    }

    public void RunTest()
    {
        // Add note only
        WaitForElement(By.CssSelector("[data-testid='referral-note']"))
            .SendKeys("No treatment and no patient");

        // Submit
        WaitForElement(By.CssSelector("[data-testid='submit-referral']")).Click();

        // Expect error
        ExpectError("behandelcode");
    }
}