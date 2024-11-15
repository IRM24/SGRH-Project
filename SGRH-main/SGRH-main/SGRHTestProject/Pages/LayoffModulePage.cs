using OpenQA.Selenium;
using System.Collections.Specialized;

namespace SGRHTestProject.Pages
{
    public class LayoffsModulePage : Base
    {
        //Localizadores principales
        private By actionsManagementLocator => By.LinkText("Gestión de Acciones");
        private By layoffsSectionLocator => By.XPath("/html/body/div[1]/aside[1]/div/div[4]/div/div/nav/ul/li[4]/ul/li[3]/a");
        private By registerLayoffButtonLocator = By.XPath("//a[@href='/Layoffs/CreateLayoff' and contains(@class, 'nav-link')]//p[text()='Registrar despido']");
        private By listLayoffButtonLocator = By.XPath("/html/body/div[1]/aside[1]/div/div[4]/div/div/nav/ul/li[4]/ul/li[3]/ul/li[1]");

        // Localizadores para el formulario de crear despido
        private By searchEmployeeLocator => By.XPath("//*[@id=\"dni\"]");
        private By searchButtonLocator => By.Id("search-btn");
        private By layoffDateLocator => By.XPath("//*[@id=\"DismissalDate\"]");
        private By layoffReasonLocator => By.XPath("//*[@id=\"DismissalCause\"]");
        private By employerResponsibilityCheckboxLocator => By.XPath("//*[@id=\"HasEmployerResponsibility\"]");
        private By createButtonLocator => By.XPath("//*[@id=\"myForm\"]/div[2]/div/input");
        private By confirmOkButtonLocator => By.XPath("//button[contains(text(), 'OK')]");
        private By SuccessMessageLocator => By.Id("swal2-title");


        // Localizadores para eliminar despido
        private By optionsButtonLocator => By.XPath("//*[@id=\"example1\"]/tbody/tr/td[1]");
        //private By deleteButtonLocator => By.XPath("//*[@id=\"example1\"]/tbody/tr[2]/td/ul/li[2]/span[2]/button");
        //private By deleteButtonLocator = By.XPath("//span[@class='dtr-data']//button[text()='Eliminar']");
        private By deleteButtonLocator = By.XPath("//button[@class='btn btn-danger ' and text()='Eliminar']");



        private By searchInputLocator = By.CssSelector("input[type='search'].form-control.form-control-sm");
        private By errorMessageLocator = By.CssSelector(".swal2-html-container");





        public LayoffsModulePage(IWebDriver driver) : base(driver) { }


        // --------------------- MÉTODOS PARA MODULO ---------------------

        public void GoToActionsManagement()
        {
            Click(actionsManagementLocator);
        }

        public void GoToLayoffsSection()
        {
            Click(layoffsSectionLocator);
        }


        public void ClickRegisterLayoff()
        {
            Click(registerLayoffButtonLocator);
        }

        public void ClickListLayoff()
        {
            Click(listLayoffButtonLocator);
        }

        // --------------------- MÉTODOS PARA REGISTRAR DESPIDO ---------------------

        public void ClickCreateLayoff()
        {
            Click(createButtonLocator);
        }


        public void SearchEmployee(string employeeIdentifier)
        {
            Type(employeeIdentifier, searchEmployeeLocator);
            Click(searchButtonLocator);
        }

        public void EnterLayoffDetails(string date, string reason)
        {
            Type(date, layoffDateLocator);
            Type(reason, layoffReasonLocator);
        }

        public void MarkEmployerResponsibility()
        {
            Click(employerResponsibilityCheckboxLocator);
        }

        public void ConfirmLayoffCreation()
        {
            Click(confirmOkButtonLocator);
        }

        public void EnterSearchLayoff(string name)
        {
            ClearField(searchInputLocator);
            Type(name, searchInputLocator);    
        }


        // --------------------- MÉTODOS PARA ELIMINAR DESPIDO ---------------------


        public void ClickOptionsLayoff()
        {
            Click(optionsButtonLocator);
        }

        public void ClickDeleteLayoff()
        {
            Click(deleteButtonLocator);
        }

        public string GetSuccessMessage()
        {
            try
            {
                IWebElement successMessage = FindElement(SuccessMessageLocator);

                return successMessage.Text;
            }
            catch (NoSuchElementException)
            {
                throw new Exception("El mensaje de éxito no fue encontrado.");
            }
        }

        public string GetErrorMessage()
        {
            return FindElement(errorMessageLocator).Text;
        }

    }
}
