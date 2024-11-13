using EllipticCurve;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGRHTestProject.Pages
{
    public class OvertimeModulePage : Base
    {
        //Localizadores principales
        private By actionsModuleLocator = By.CssSelector("a.nav-link i.fas.fa-list");
        private By overtimeSectionLocator = By.XPath("//a[.//p[text()='Horas Extra']]");
        private By overtimeRequestBtnLocator = By.XPath("//a[.//p[text()='Solicitud de Horas Extras']]");
        private By myRequestsBtnLocator = By.XPath("//a[.//p[text()='Mis solicitudes'] and @href='/Overtime/MyOvertimes']");

        // Localizadores para el formulario de solicitud de vacaciones
        private By dateInputLocator = By.Id("OT_Date");
        private By hoursQuantityInputLocator = By.Id("Hours_Worked");
        private By requestDescriptionInputLocator = By.Id("Description");
        private By extraHoursTypeSelectorLocator = By.Id("TypeOT");
        private By submitButtonLocator = By.XPath("//input[@value='Enviar Solicitud' and contains(@class, 'btn-success')]");
        private By successMessageLocator = By.XPath("//div[@class='swal2-html-container' and text()='Se registró la solicitud exitosamente']");

        // Localizadores para ver mis solicitudes
        private By myRequestMessagePage = By.XPath("//strong[text()='Listado de mis solicitudes de Horas Extra']");

        //Localizadores para gestion de horas extras
        private By overtimeManagementBtnLocator = By.XPath("//a[.//p[text()='Gestión de Horas Extras']]");
        private By pendingOvertimeRequestsTable = By.CssSelector("table tbody tr");
        private By approveOvertimeRequestBtnLocator = By.XPath("(//tr[td]//form[@method='post'][@action='/Overtime/Approve']//button[@class='btn btn-success'])[1]");
        private By approvedMessageLocator = By.XPath("//div[@class='swal2-html-container' and text()='Solicitud aprobada exitosamente.']");
        private By rejectOvertimeRequestBtnLocator = By.XPath("(//tr[td]//form[@method='post'][@action='/Overtime/Reject']//button[@class='btn btn-danger'])[1]");
        private By rejectedMessageLocator = By.XPath("//div[@class='swal2-html-container' and text()='Solicitud rechazada exitosamente.']");



        public OvertimeModulePage(IWebDriver driver) : base(driver)
        {
        }

        // --------------------- MÉTODOS PARA MODULO ---------------------

        public void GoToActionsModule()
        {
            Click(actionsModuleLocator);
        }

        public void GoToOvertimeSection()
        {
            Click(actionsModuleLocator);
            Thread.Sleep(1000);
            Click(overtimeSectionLocator);
            Thread.Sleep(1000);
        }

        public void ClickOvertimeRequestButton()
        {
            Click(overtimeRequestBtnLocator);
        }

        public void ClickMyRequestsButton()
        {
            Click(myRequestsBtnLocator);
        }

        /*
        public void GoToOvertimeRequestPage()
        {
            GoToActionsModule();
            Thread.Sleep(1000);
            GoToOvertimeSection();
            Thread.Sleep(1000);
            ClickOvertimeRequestButton();
        }
        */

        // --------------------- MÉTODOS PARA LLENAR SOLICITUD ---------------------
        public void FillOvertimeRequestForm(string date, int hoursQuantity, string requestDescription, string extraHoursType)
        {
            Type(date, dateInputLocator);

            ClearField(hoursQuantityInputLocator);
            Type(hoursQuantity.ToString(), hoursQuantityInputLocator);

            Type(requestDescription, requestDescriptionInputLocator);

            SelectDropdownOption(extraHoursTypeSelectorLocator, extraHoursType);
        }

        public void SubmitOvertimeRequest()
        {
            Click(submitButtonLocator);
        }


        public string GetOvertimeRequestSuccessMessage()
        {
            return FindElement(successMessageLocator).Text;
        }


        // --------------------- MÉTODOS PARA VER SOLICITUDES ---------------------
        public string GetMyRequestsMessagePage()
        {
            return FindElement(myRequestMessagePage).Text;
        }


        // --------------------- MÉTODOS PARA GESTIÓN DE HORAS EXTRAS ---------------------
        public void ClickOvertimeManagementButton()
        {
            Click(overtimeManagementBtnLocator);
        }

        public bool ArePendingOvertimeRequestsVisible()
        {
            var rows = FindElements(pendingOvertimeRequestsTable);

            return rows.Count > 0;
        }

        public void ClickApproveOvertimeRequest()
        {
            Click(approveOvertimeRequestBtnLocator);
        }

        public string GetApprovedRequestMessage()
        {
            return FindElement(approvedMessageLocator).Text;
        }

        public void ClickRejectOvertimeRequest()
        {
            Click(rejectOvertimeRequestBtnLocator);
        }


        public string GetRejectedRequestMessage()
        {
            return FindElement(rejectedMessageLocator).Text;
        }
    }
}
