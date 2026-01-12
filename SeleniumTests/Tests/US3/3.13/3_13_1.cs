using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumTests.P_O_M;
using System;



namespace SeleniumTests { 


[TestClass]
public class _3_13_1
{
    private IWebDriver driver;
    private WebDriverWait wait;
    private string baseUrl = "http://localhost:5173";
    private LoginPage loginPage;

    [TestInitialize]
    public void Setup()
    {
        var options = new ChromeOptions();
        options.AddArgument("--start-maximized");
        options.AddArgument("--ignore-certificate-errors");

        driver = new ChromeDriver(options);
        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

        loginPage = new LoginPage(driver);
    }

    [TestCleanup]
    public void TearDown()
    {
        driver.Quit();
        driver.Dispose();
    }

    
    [TestMethod]
    public void TC_3_13_1_AccessAdminLoginPage()
    {
        Console.WriteLine("Test: Access admin login page");

        
        driver.Navigate().GoToUrl($"{baseUrl}/");
        Console.WriteLine("Startpagina geladen.");

        
        Assert.IsTrue(
            driver.Url.Contains("/"),
            "Gebruiker is niet doorgestuurd naar de loginpagina."
        );
        Console.WriteLine("Gebruiker is doorgestuurd naar de reguliere loginpagina.");

        
        Assert.IsTrue(
            loginPage.IsAdminLoginLinkDisplayed(),
            "Admin login knop is niet zichtbaar op de gebruikers-loginpagina."
        );
        Console.WriteLine("Admin login knop is zichtbaar.");

        
        loginPage.ClickAdminLogin();
        Console.WriteLine("Op admin login knop geklikt.");

       
        wait.Until(d => d.Url.Contains("/admin/login"));

        Assert.IsTrue(
            driver.Url.Contains("/admin/login"),
            "Admin login pagina heeft geen aparte URL."
        );

        Assert.IsFalse(
            driver.Url.Contains("/inloggen"),
            "Admin login pagina is niet gescheiden van de gebruikers-loginpagina."
        );

        Console.WriteLine("Admin login pagina is apart en duidelijk gescheiden.");
        Console.WriteLine("? Test geslaagd.\n");
    }
}
}
