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
    public class DossiersModulePage : Base
    {
        //Localizadores principales
        private By dossiersModuleLocator = By.CssSelector("a.nav-link i.fa.fa-archive.nav-icon");
        private By createDossierBtnLocator = By.CssSelector("a.nav-link[href='/Dossiers/Create'] i.far.fa-circle.nav-icon");

        // Localizadores para el formulario de crear expediente
        private By searchUserInputLocator = By.Id("dni");
        private By documentTypeSelectorLocator = By.Id("DocumentType");
        private By descriptionInputLocator = By.Id("Description");
        private By customFileSlectorLocator = By.Id("customFile");
        private By searchUserBtnLocator = By.Id("search-btn");
        private By submitButtonLocator = By.XPath("//input[@type='submit' and @value='Crear']");
        private By confirmButtonLocator = By.CssSelector("button.swal2-confirm.swal2-styled");
        private By successMessageLocator = By.XPath("//div[@class='swal2-html-container' and text()='Expediente registrado correctamente.']");
        private By usersNotFoundMessageLocator = By.XPath("//div[@class='swal2-html-container' and text()='No se encontraron usuarios con el valor de búsqueda proporcionado.']");

        // Localizadores para la lista de expedientes
        private By dossiersListBtnLocator = By.CssSelector("a.nav-link[href='/Dossiers'] i.far.fa-circle.nav-icon");
        private By dossiersListTable = By.CssSelector("table tbody tr");
        private By accionColumnLocator = By.XPath("//th[text()='Acción']");
        private By deleteDossierBtnLocator = By.XPath("(//a[text()='Eliminar'])[1]");
        private By submitDeleteBtnLocator = By.CssSelector("input[type='submit'][value='Eliminar']");
        private By dossierDeletedMessageLocator = By.XPath("//div[@class='swal2-html-container' and text()='Expediente registrado correctamente.']");



        //Localizadores para gestion de horas extras
        private By overtimeManagementBtnLocator = By.XPath("//a[.//p[text()='Gestión de Horas Extras']]");
        private By approveOvertimeRequestBtnLocator = By.XPath("(//tr[td]//form[@method='post'][@action='/Overtime/Approve']//button[@class='btn btn-success'])[1]");
        private By approvedMessageLocator = By.XPath("//div[@class='swal2-html-container' and text()='Solicitud aprobada exitosamente.']");
        private By rejectOvertimeRequestBtnLocator = By.XPath("(//tr[td]//form[@method='post'][@action='/Overtime/Reject']//button[@class='btn btn-danger'])[1]");
        private By rejectedMessageLocator = By.XPath("//div[@class='swal2-html-container' and text()='Solicitud rechazada exitosamente.']");



        public DossiersModulePage(IWebDriver driver) : base(driver)
        {
        }

        // --------------------- MÉTODOS PARA MODULO ---------------------

        public void GoToDossiersModule()
        {
            Click(dossiersModuleLocator);
        }


        public void ClickCreateDossierButton()
        {
            Click(createDossierBtnLocator);
        }


        // --------------------- MÉTODOS PARA CREAR EXPEDIENTE ---------------------
        public void FillCreateDossierForm(string documentType, string description)
        {
            SelectDropdownOption(documentTypeSelectorLocator, documentType);

            Type(description, descriptionInputLocator);
        }

        public void SearchEmployee(string employeeNameOrId)
        {
            Type(employeeNameOrId, searchUserInputLocator);
            ClickSearchUserButton();
        }

        public void ClickSearchUserButton()
        {
            Click(searchUserBtnLocator);
        }

        public void ClickSubmitDossierButton()
        {
            Click(submitButtonLocator);
        }

        public void ClickConfirmButton()
        {
            Click(confirmButtonLocator);
        }


        public string GetDossierCreationSuccessMessage()
        {
            return FindElement(successMessageLocator).Text;
        }

        public string GetUsersNotFoundMessage()
        {
            return FindElement(usersNotFoundMessageLocator).Text;
        }


        // --------------------- MÉTODOS PARA LISTA DE EXPEDIENTES ---------------------
        public void ClickDossiersListButton()
        {
            Click(dossiersListBtnLocator);
        }

        public bool IsActionButtonsVisibleForDossier()
        {
            var actionButtons = FindElements(accionColumnLocator);
            return actionButtons.Count > 0;
        }

        public void ClickDeleteDossierButton()
        {
            Click(deleteDossierBtnLocator);
        }

        public void ClickSubmitDeleteDossierButton()
        {
            Click(submitDeleteBtnLocator);
        }

        public string GetDossierDeletedMessage()
        {
            return FindElement(dossierDeletedMessageLocator).Text;
        }

        // --------------------- MÉTODOS PARA GESTIÓN DE HORAS EXTRAS ---------------------
        public void ClickOvertimeManagementButton()
        {
            Click(overtimeManagementBtnLocator);
        }

        public bool IsDossierListVisible()
        {
            var rows = FindElements(dossiersListTable);

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
