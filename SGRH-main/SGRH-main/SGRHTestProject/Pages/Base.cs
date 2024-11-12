using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Chrome;

namespace SGRHTestProject.Pages
{
    public class Base
    {

        private IWebDriver driver;

        public Base(IWebDriver driver)
        {
            this.driver = driver;
        }

        public IWebDriver ChromeDriverConnection()
        {
            //driver = new OpenQA.Selenium.Chrome.ChromeDriver();
            //string driverPath = @"./resources/chromedriver/chromedriver.exe"; // Ruta donde colocaste el chromedriver
            driver = new ChromeDriver("C:\\Users\\fabia\\source\\repos\\SGRH-Project\\SGRH-main\\SGRH-main\\SGRHTestProject\\resources\\chromedriver\\chromedriver.exe");
            return driver;
        }


        // Encontrar un solo elemento
        public IWebElement FindElement(By locator)
        {
            return driver.FindElement(locator);
        }

        // Encontrar múltiples elementos
        public IList<IWebElement> FindElements(By locator)
        {
            return driver.FindElements(locator);
        }

        // Obtener texto de un elemento
        public string GetText(IWebElement element)
        {
            return element.Text;
        }


        // Obtener texto de un elemento por locator
        public string GetText(By locator)
        {
            return driver.FindElement(locator).Text;
        }

        // Escribir texto en un campo
        public void Type(string inputText, By locator)
        {
            driver.FindElement(locator).SendKeys(inputText);
        }

        // Hacer clic en un elemento por locator
        public void Click(By locator)
        {
            driver.FindElement(locator).Click();
        }

        // Hacer clic en un elemento pasado como parámetro
        public void Click(IWebElement element)
        {
            element.Click();
        }


        // Verificar si un elemento está presente y visible en la página
        public bool IsDisplayed(By locator)
        {
            try
            {
                return driver.FindElement(locator).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        // Navegar a una URL específica
        public void Visit(string url)
        {
            driver.Navigate().GoToUrl(url);
        }



        public void SelectDropdownOption(By dropdownLocator, string optionText)
        {
            IWebElement dropdownElement = FindElement(dropdownLocator);
            SelectElement select = new SelectElement(dropdownElement);
            select.SelectByText(optionText);
        }

        public void ClearField(By fieldLocator)
        {
            var field = FindElement(fieldLocator);
            field.Clear();
        }


    }
}
