using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SeleniumTests.Pages;

namespace SeleniumTests;

[TestClass]
public class _5_9_1
{
    private IWebDriver CreateDriver() => new ChromeDriver();

    [TestMethod]
    public void CreateReferral_SuccessTest()
    {
        IWebDriver driver = null;
        try
        {
            Console.WriteLine("LOG [Step 1] Starting CreateReferral_SuccessTest...");
            driver = CreateDriver();
            var page = new DoorverwijzingPage(driver);

            page.Navigate();
            Console.WriteLine("LOG [Step 2] Navigated to doorverwijzing page.");
            Assert.IsTrue(page.WaitForPageLoad(), "Page did not load (required elements missing).");

            // Fill all fields and submit
            page.EnterPatient("Test Patient");
            page.EnterTreatment("Test Treatment");
            page.NotePatient("This is a test note.");
            page.ClickConfirm();

            // Verify referral created
            Assert.IsTrue(page.IsReferralCreated(), "Expected a new referral to be created but it was not detected.");
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