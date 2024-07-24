using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ExpertCenter.Tests.Functional;

/// <summary>
/// Перед запуском тестов необходимо запустить сайт с помощью Ctrl+F5
/// </summary>
public class FunctionalTests : IDisposable
{
    private readonly IWebDriver _driver;

    public FunctionalTests()
    {
        _driver = new ChromeDriver();
    }

    [Fact]
    public void TheHomePageHasATitle_ShouldReturnTitle()
    {
        // Act
        _driver.Navigate().GoToUrl("https://localhost:7270/");

        // Assert
        Assert.Contains("Главная", _driver.Title);
    }

    [Fact]
    public void ThePriceListsPageHasATitle_ShouldReturnTitle()
    {
        // Act
        _driver.Navigate().GoToUrl("https://localhost:7270/PriceLists");

        // Assert
        Assert.Contains("Прайс листы", _driver.Title);
    }

    [Fact]
    public void TheCreatePriceListPageHasATitle_ShouldReturnTitle()
    {
        // Act
        _driver.Navigate().GoToUrl("https://localhost:7270/PriceLists/Create");

        // Assert
        Assert.Contains("Создание прайс листа", _driver.Title);
    }

    [Fact]
    public void CreatePriceList_ShouldCreatePriceListAndRedirectDetails()
    {
        // Act
        _driver.Navigate().GoToUrl("https://localhost:7270/PriceLists/Create");
        IWebElement form = _driver.FindElement(By.TagName("form")); // Находим формы

        IWebElement nameInput = form.FindElement(By.Name("Name")); // Находим поле ввода по ID
        nameInput.SendKeys("Test Price List");

        IWebElement submitButton = _driver.FindElement(By.CssSelector("[type='submit']"));
        submitButton.Click();

        // Assert
        WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(3));
        wait.Until(driver => driver.Title.Contains("Test Price List"));
        Assert.Contains("Test Price List", _driver.Title);
    }

    [Fact]
    public void DeletePriceList_ShouldDeletePriceListAndReloadPage()
    {
        // Act
        _driver.Navigate().GoToUrl("https://localhost:7270/PriceLists");
        IWebElement priceListRow = _driver.FindElement(By.XPath("//tr[.//td[contains(., 'Test Price List')]]"));

        IWebElement deleteButton = priceListRow.FindElement(By.CssSelector(".btn-danger"));
        deleteButton.Click();
        WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(1.5));
        wait.Until((driver) =>
        {
            return driver.FindElement(By.ClassName("modal-dialog")) != null;
        });

        IWebElement form = _driver.FindElement(By.TagName("form"));
        IWebElement deleteButtonConfirmed = form.FindElement(By.CssSelector("[type='submit']"));
        wait.Until(driver =>
        {
            var modal = driver.FindElement(By.Id("deleteModal"));
            return modal.GetAttribute("class").Contains("show");
        });
        deleteButtonConfirmed.Click();

        // Assert
        wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(3));
        wait.Until((driver) =>
        {
            return driver.Title != "Прайс листы";
        });
        Assert.Contains("Прайс листы", _driver.Title);
        ReadOnlyCollection<IWebElement> missingElements = _driver.FindElements(By.XPath("//tr[.//td[contains(., 'Test Price List')]]"));
        Assert.Empty(missingElements);
    }

    public void Dispose()
    {
        _driver.Quit();
    }
}
