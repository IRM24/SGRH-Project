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
    public class OvertimeModuleTest
    {
        private IWebDriver driver;
        private OvertimeModulePage overtimeModulePage;
        LogInPage logInPage;

        [SetUp]
        public void SetUp()
        {
            overtimeModulePage = new OvertimeModulePage(driver);
            driver = overtimeModulePage.ChromeDriverConnection();
            overtimeModulePage.Visit("https://localhost:7122/Account/Login?ReturnUrl=%2F");
            logInPage = new LogInPage(driver);
            driver.Manage().Window.Maximize();

            string email = "cpicado869@gmail.com";
            string password = "Hola321!";
            logInPage.LogIn(email, password);
            overtimeModulePage.GoToOvertimeSection();
        }

        [TearDown]
        public void TearDown()
        {
            //driver.Quit();
            driver.Dispose();
        }



        // Caso de prueba: Acciones-13 - Aprobar una solicitud de horas extras.
        [Test]
        public void ApproveOvertimeRequest()
        {
            overtimeModulePage.ClickOvertimeManagementButton();
            Thread.Sleep(1000);

            overtimeModulePage.ClickApproveOvertimeRequest();
            Thread.Sleep(1500);

            string approvalMessage = overtimeModulePage.GetApprovedRequestMessage();
            Assert.AreEqual("Solicitud aprobada exitosamente.", approvalMessage, "El mensaje de aprobación no es correcto.");
        }


        // Caso de prueba: Acciones-14 - Rechazar una solicitud de horas extras.
        [Test]
        public void RejectOvertimeRequest()
        {
            overtimeModulePage.ClickOvertimeManagementButton();
            Thread.Sleep(1000);

            overtimeModulePage.ClickRejectOvertimeRequest();
            Thread.Sleep(1500);

            string approvalMessage = overtimeModulePage.GetRejectedRequestMessage();
            Assert.AreEqual("Solicitud rechazada exitosamente.", approvalMessage, "El mensaje de rechazo no es correcto.");
        }



        // Caso de prueba: Acciones-09 - Realizar una solicitud de horas extras
        [Test]
        public void RequestOvertime()
        {
            overtimeModulePage.ClickOvertimeRequestButton();

            string fecha = "24/02/2024";
            int cantidadHoras = 2;
            string descripcion = "Solicitud de horas extras para el 23 de septiembre del 2024";
            string tipoHoras = "Sencillas";

            overtimeModulePage.FillOvertimeRequestForm(fecha, cantidadHoras, descripcion, tipoHoras);

            overtimeModulePage.SubmitOvertimeRequest();

            Assert.AreEqual("Se registró la solicitud exitosamente", overtimeModulePage.GetOvertimeRequestSuccessMessage());
        }


        // Caso de prueba: Acciones-11 - Consultar horas extras registradas.
        [Test]
        public void ViewRegisteredOvertime()
        {
            overtimeModulePage.ClickMyRequestsButton();
            Thread.Sleep(1000);

            Assert.AreEqual("Listado de mis solicitudes de Horas Extra", overtimeModulePage.GetMyRequestsMessagePage());

        }

        // Caso de prueba: Acciones-12 - Visualizar solicitudes de horas extras pendientes.
        [Test]
        public void ViewPendingOvertimeRequests()
        {
            overtimeModulePage.ClickOvertimeManagementButton();
            Thread.Sleep(1000);

            bool arePendingRequestsVisible = overtimeModulePage.ArePendingOvertimeRequestsVisible();

            Assert.IsTrue(arePendingRequestsVisible, "No se encontraron solicitudes de horas extras pendientes.");
        }

    }
}
