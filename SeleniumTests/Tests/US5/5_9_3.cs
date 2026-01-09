using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SeleniumTests.Pages;

namespace SeleniumTests;

[TestClass]
public class _5_9_3
{
    private IWebDriver CreateDriver() => new ChromeDriver();

    [TestMethod]
    public void CreateReferral_NoPatientTest()
    {
        IWebDriver driver = null;
        try
        {
            Console.WriteLine("LOG [Step 1] Starting CreateReferral_NoPatientTest...");
            driver = CreateDriver();
            var page = new DoorverwijzingPage(driver);

            page.Navigate();
            Console.WriteLine("LOG [Step 2] Navigated to doorverwijzing page.");
            Assert.IsTrue(page.WaitForPageLoad(), "Page did not load (required elements missing).");

            // Do not fill patient; fill treatment and note
            page.EnterTreatment("Test Treatment");
            page.NotePatient("Note without patient");
            page.ClickConfirm();

            // Verify referral was NOT created
            Assert.IsFalse(page.IsReferralCreated(), "Expected no referral when patient is missing but one was detected.");
        }
        catch (Exception ex)
        {
            Assert.Fail("Test failed with exception: " + ex.Message);
        }
        finally
        {
            driver?.Quit();
        }
    }
}
