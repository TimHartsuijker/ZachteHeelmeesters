using OpenQA.Selenium;

namespace SeleniumTests.Pages;

public class CalendarPage(IWebDriver driver) : BasePage(driver)
{
    protected override string Path => "/agenda";

    // --- Locators (gebaseerd op je nieuwe HTML) ---
    private static By CalendarContainer => By.ClassName("doctor-calendar");
    private static By CurrentWeekLabel => By.ClassName("current-week");
    private static By NavButtons => By.ClassName("btn-nav");
    private static By TodayButton => By.CssSelector(".btn-nav.btn-nav-secondary");

    // Actie knoppen
    private static By MakeUnavailableBtn => By.CssSelector(".btn.btn-primary");
    private static By MakeAvailableBtn => By.CssSelector(".btn.btn-secondary");

    // Grid Elementen
    private static By DayColumns => By.ClassName("day-column");
    private static By TimeSlots => By.ClassName("time-slot");
    private static By DayHeaders => By.ClassName("day-header");

    // --- Navigatie ---
    public void Navigate() => NavigateTo(BaseUrl + Path);

    // --- Wacht Logica ---
    public void WaitForCalendarLoad() => WaitForElement(CalendarContainer);

    // --- Acties ---
    public void ClickNextWeek() => Driver.FindElements(NavButtons).Last().Click();

    public void ClickPreviousWeek() => Driver.FindElements(NavButtons).First().Click();

    public void ClickToday() => Click(TodayButton);

    public void ClickMakeUnavailable() => Click(MakeUnavailableBtn);

    // --- Getters & Checks ---
    public string GetCurrentWeekRange() => GetText(CurrentWeekLabel);

    public int GetDayColumnCount() => Driver.FindElements(DayColumns).Count;

    public int GetTotalTimeSlotsCount() => Driver.FindElements(TimeSlots).Count;

    public string GetDoctorId() => Driver.FindElement(CalendarContainer).GetAttribute("doctor-id");

    public bool IsCalendarVisible() => IsElementDisplayed(CalendarContainer);

    // Haal de datums op van de headers (bijv. 2026-01-19)
    public List<string> GetVisibleDates()
    {
        return Driver.FindElements(By.ClassName("day-date"))
                     .Select(e => e.Text)
                     .ToList();
    }

    // Specifieke helper om te checken of de gebruiker "Onbekend" is (zoals in je HTML)
    public string GetUserInfo() => GetText(By.ClassName("user-info"));
}