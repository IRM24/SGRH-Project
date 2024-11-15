using Castle.Components.DictionaryAdapter;
using OpenQA.Selenium;
using SixLabors.ImageSharp.Processing.Processors.Filters;

namespace SGRHTestProject.Pages
{
    public class VacationModulePage : Base
    {
        private By gestionAccionesLocator => By.LinkText("Gestión de Acciones");
        private By vacacionesSectionLocator => By.XPath("/html/body/div[1]/aside[1]/div/div[4]/div/div/nav/ul/li[4]/ul/li[1]/a");

        private By misSolicitudesLocator = By.XPath("//a[@href='/Vacations/MyVacationsRequests']//p[text()='Mis solicitudes']");
        private By solicitarVacacionesBtnLocator => By.XPath("/html/body/div[1]/div[2]/section/div/div/div[2]/p/a");
        private By startDateLocator => By.Id("startDate");
        private By endDateLocator => By.Id("endDate");
        private By submitVacacionesBtnLocator => By.XPath("/html/body/div[1]/div[2]/section/div/div[1]/div/div/div/form/div[2]/div/input");

        private By errorMessageBalanceLocator => By.XPath("/html/body/div[2]/div");
        private By descriptionErrorLocator = By.XPath("/html/body/div[1]/div[2]/section/div/div[1]/div/div/div/form/div[1]/div[3]/span");
        //private By errorMessageInvalidDate = By.XPath("/html/body/div[4]/div");
        private By errorMessageInvalidDate = By.XPath("//div[@id='swal2-html-container']");


        private By descripcionLocator = By.XPath("//*[@id=\"Description\"]");
        private By commentsLocator = By.XPath("//*[@id=\"Comments\"]");

        private By pdfDownloadButtonLocator = By.XPath("//*[@id=\"example1_wrapper\"]/div[1]/div[1]/div/button[4]");
        private By excelDownloadButtonLocator = By.XPath("//*[@id=\"example1_wrapper\"]/div[1]/div[1]/div/button[3]");

        private By searchBoxLocator = By.XPath("//*[@id=\"example1_filter\"]/label/input"); // Cambia "searchBoxId" por el ID real de la barra de búsqueda
        private By resultItemLocator = By.XPath("//*[@id=\"example1_wrapper\"]/div[2]/div"); // Cambia "resultClassName" por la clase real de los elementos de resultad



        public VacationModulePage(IWebDriver driver) : base(driver) { }


        // --------------------- MÉTODOS PARA MODULO ---------------------

        public void GoToActionsManagement()
        {
            Click(gestionAccionesLocator);
        }

        public void GoToVacationSection()
        {
            Click(vacacionesSectionLocator);
        }

        public void GoToMyRequests()
        {
            Click(misSolicitudesLocator);
        }


        // --------------------- MÉTODOS PARA SOLICITAR VACACIONES ---------------------
        public void ClickRequestVacation()
        {
            Click(solicitarVacacionesBtnLocator);
        }

        public void EnterVacationStartDate(string startDate)
        {
            Type(startDate, startDateLocator);
        }


        public void EnterVacationDates(string startDate, string endDate)
        {
            Type(startDate, startDateLocator);
            Type(endDate, endDateLocator);
        }

        public void EnterVacationDescriptionComments(string description, string comment)
        {
            Type(description, descripcionLocator);
            Type(comment, commentsLocator);
        }

        public void SubmitVacationRequest()
        {
            Click(submitVacacionesBtnLocator);
        }

        public void LeaveDescriptionAndCommentsEmpty()
        {
            ClearField(descripcionLocator);
            ClearField(commentsLocator);
        }

        public string GetErrorMessageForBalance()
        {
            var errorElement = WaitUntilElementIsVisible(errorMessageBalanceLocator);
            return errorElement.Text;
        }

        public string GetErrorMessageForDescription()
        {
            var errorElement = WaitUntilElementIsVisible(descriptionErrorLocator);
            return errorElement.Text;
        }

        public string GetErrorMessageForInvalidDate()
        {
            var errorElement = WaitUntilElementIsVisible(errorMessageInvalidDate);
            return errorElement.Text;
        }

        // --------------------- MÉTODOS PARA DESCARGAS ---------------------

        public void DownloadVacationRequestsPDF()
        {
            Click(pdfDownloadButtonLocator);
        }

        public void DownloadVacationRequestsExcel()
        {
            Click(excelDownloadButtonLocator);
        }


        // --------------------- MÉTODOS PARA BÚSQUEDAS ---------------------
        public void EnterSearchTerm(string searchTerm)
        {
            ClearField(searchBoxLocator);
            Type(searchTerm, searchBoxLocator);
        }

        public List<IWebElement> GetSearchResults()
        {
            return FindElements(resultItemLocator).ToList();
        }

    }
}
