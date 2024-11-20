using Microsoft.VisualStudio.TestPlatform.Utilities.Helpers;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using SGRHTestProject.Pages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGRHTestProject.Tests.AutomatedTests
{
    [TestFixture]
    public class DossiersModuleTest
    {
        private IWebDriver driver;
        private DossiersModulePage dossiersModulePage;
        LogInPage logInPage;

        [SetUp]
        public void SetUp()
        {
            dossiersModulePage = new DossiersModulePage(driver);
            driver = dossiersModulePage.ChromeDriverConnection();
            dossiersModulePage.Visit("https://localhost:7122/Account/Login?ReturnUrl=%2F");
            logInPage = new LogInPage(driver);
            driver.Manage().Window.Maximize();

            string email = "cpicado869@gmail.com";
            string password = "Hola321!";
            logInPage.LogIn(email, password);
            dossiersModulePage.GoToDossiersModule();
            Thread.Sleep(2000);
        }

        [TearDown]
        public void TearDown()
        {
            //driver.Quit();
            driver.Dispose();
        }


        // Caso de prueba: Expedientes-01 - Crear nuevo expediente para empleado buscando por el nombre
        [Test]
        public void CreateUserDossierByName()
        {
            string employeeName = "Fabiana";
            string documentType = "Identificación";
            string description = "Identificación actualizada Fabiana Arias";

            dossiersModulePage.ClickCreateDossierButton();
            Thread.Sleep(2000);

            dossiersModulePage.SearchEmployee(employeeName);
            Thread.Sleep(5000);

            dossiersModulePage.FillCreateDossierForm(documentType, description);

            dossiersModulePage.ClickSubmitDossierButton();

            dossiersModulePage.ClickConfirmButton();
            Thread.Sleep(500);

            Assert.AreEqual("Expediente registrado correctamente.", dossiersModulePage.GetDossierCreationSuccessMessage());

        }


        // Caso de prueba: Expedientes-02 - Crear nuevo expediente para empleado buscando por el nombre de un empleado inexistente
        [Test]
        public void CreateDossierForNonExistentEmployeeByName()
        {
            string nonExistentEmployeeName = "Ian";

            dossiersModulePage.ClickCreateDossierButton();
            Thread.Sleep(2000);

            dossiersModulePage.SearchEmployee(nonExistentEmployeeName);
            Thread.Sleep(5000);


            string errorMessage = dossiersModulePage.GetUsersNotFoundMessage();
            Assert.AreEqual("No se encontraron usuarios con el valor de búsqueda proporcionado.", errorMessage);
        }




        // Caso de prueba: Expedientes-03 - Crear nuevo expediente para empleado buscando por la cedula
        [Test]
        public void CreateEmployeeDossierById()
        {
            string employeeId = "1-2312-3123";
            string documentType = "Identificación";
            string description = "Identificación actualizada German";
            string documentFile = "identificacionGerman2024.pdf"; // Documento opcional

            dossiersModulePage.ClickCreateDossierButton();
            Thread.Sleep(2000);

            dossiersModulePage.SearchEmployee(employeeId);
            Thread.Sleep(5000);

            dossiersModulePage.FillCreateDossierForm(documentType, description);

            dossiersModulePage.ClickSubmitDossierButton();

            dossiersModulePage.ClickConfirmButton();
            Thread.Sleep(500);

            Assert.AreEqual("Expediente registrado correctamente.", dossiersModulePage.GetDossierCreationSuccessMessage());
        }


        // Caso de prueba: Expedientes-04 - Crear nuevo expediente para empleado buscando por la cédula de un empleado inexistente
        [Test]
        public void CreateDossierForNonExistentEmployeeById()
        {
            string nonExistentEmployeeId = "0-0000-0000";

            dossiersModulePage.ClickCreateDossierButton();
            Thread.Sleep(2000);

            dossiersModulePage.SearchEmployee(nonExistentEmployeeId);
            Thread.Sleep(4000);


            string errorMessage = dossiersModulePage.GetUsersNotFoundMessage();
            Assert.AreEqual("No se encontraron usuarios con el valor de búsqueda proporcionado.", errorMessage);
        }


        // Caso de prueba: Expedientes-05 - Visualizar y buscar los expedientes
        [Test]
        public void ViewAndSearchDossiers()
        {
            dossiersModulePage.ClickDossiersListButton();
            Thread.Sleep(2000);

            bool isListVisible = dossiersModulePage.IsDossierListVisible();
            Assert.IsTrue(isListVisible, "El listado de expedientes no está visible.");

            Assert.IsTrue(dossiersModulePage.IsActionButtonsVisibleForDossier(), "Los botones de acción no están disponibles para el expediente.");
        }



        // Caso de prueba: Expedientes-08 - Eliminar registro de expediente
        [Test]
        public void DeleteDossier()
        {
            dossiersModulePage.ClickDossiersListButton();
            Thread.Sleep(2000);

            dossiersModulePage.ClickDeleteDossierButton();
            Thread.Sleep(1000);
            
            dossiersModulePage.ClickSubmitDeleteDossierButton();
            Thread.Sleep(500);

            dossiersModulePage.ClickConfirmButton();
            Thread.Sleep(1000);

            string successMessage = dossiersModulePage.GetDossierDeletedMessage();
            Assert.AreEqual("Expediente eliminado correctamente.", successMessage, "El mensaje de eliminación no es el esperado.");
        }



    }
}
