using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SGRHTestProject.Pages;
using System;
using System.Threading;

namespace SGRHTestProject.Tests.AutomatedTests
{
    [TestFixture]
    public class VacationModuleTest
    {
        private IWebDriver driver;
        private LogInPage logInPage;
        private UserModulePage userModulePage;
        private VacationModulePage vacationModulePage;

        [SetUp]
        public void SetUp()
        {
            vacationModulePage = new VacationModulePage(driver);
            driver = vacationModulePage.ChromeDriverConnection();
            vacationModulePage.Visit("https://localhost:7122/Account/Login?ReturnUrl=%2F");
            logInPage = new LogInPage(driver);
            driver.Manage().Window.Maximize();

            logInPage.LogIn("cpicado869@gmail.com", "Hola321!");
            Thread.Sleep(3000);
            vacationModulePage.GoToActionsManagement();
            Thread.Sleep(2000);
            vacationModulePage.GoToVacationSection();
            Thread.Sleep(2000);
            vacationModulePage.GoToMyRequests();
            Thread.Sleep(2000);
        }

        [TearDown]
        public void TearDown()
        {
            //driver.Close();
            //driver.Dispose();
        }

        // Caso de prueba: Acciones-01 - Solicitar días de vacaciones en un rango de fechas inválido
        [Test]
        public void RequestVacationWithInvalidDateRange()
        {
            vacationModulePage.ClickRequestVacation();
            Thread.Sleep(2000);

            vacationModulePage.EnterVacationDates("11/11/2024", "23/11/2024");
            vacationModulePage.EnterVacationDescriptionComments("solicitud de vacaciones", "solicito respuesta lo antes posible porfavor");

            string errorMessage = vacationModulePage.GetErrorMessageForInvalidDate();
            Assert.AreEqual("La fecha de salida no puede ser anterior al día de hoy.", errorMessage, "Debería aparecer un mensaje de error indicando que la fecha no puede ser anterior al día de hoy.");
        }


        // Caso de prueba: Acciones-02 - Solicitar vacaciones sin descripción ni comentarios adicionales
        [Test]
        public void RequestVacationWithoutDescriptionOrComments()
        {
            vacationModulePage.ClickRequestVacation();
            Thread.Sleep(2000);

            // Seleccionar un rango de fechas válido
            vacationModulePage.EnterVacationDates("12/12/2024", "13/12/2024");

            // Dejar los campos de descripción y comentarios vacíos
            vacationModulePage.LeaveDescriptionAndCommentsEmpty();

            // Intentar enviar la solicitud
            vacationModulePage.SubmitVacationRequest();
            Thread.Sleep(2000);

            string errorMessage = vacationModulePage.GetErrorMessageForDescription();
            Assert.AreEqual("El campo Descripción es requerido.", errorMessage, "Debería aparecer un mensaje de error indicando que el campo Descripción es requerido.");
        }

        // Caso de prueba: Acciones-03 - Solicitar vacaciones excediendo el saldo disponible
        [Test]
        public void RequestVacationExceedingAvailableBalance()
        {
            vacationModulePage.ClickRequestVacation();
            Thread.Sleep(2000);

            vacationModulePage.EnterVacationDates("01/01/2025", "01/02/2025");

            vacationModulePage.EnterVacationDescriptionComments("solicitud de vacaciones", "solicito respuesta lo antes posible porfavor");

            vacationModulePage.SubmitVacationRequest();
            Thread.Sleep(2000);

            string errorMessage = vacationModulePage.GetErrorMessageForBalance();
            Assert.IsTrue(errorMessage.Contains("Lo sentimos, no cuenta con suficientes días disponibles."),
                          "Se esperaba un mensaje de error indicando que el saldo es insuficiente.");
        }


        // Caso de prueba: Acciones-04 - Descargar las Vacaciones en PDF
        [Test]
        public void DownloadVacationRequestsAsPDF()
        {

            vacationModulePage.DownloadVacationRequestsPDF();
            Thread.Sleep(5000);

            string downloadPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

            string partialFileName = "SGRH  Sistema de Gestión de Recursos Humanos";

            var matchingFiles = Directory.GetFiles(downloadPath, "*.pdf")
                                          .Where(file => Path.GetFileName(file).Contains(partialFileName))
                                          .ToList();

            Assert.IsTrue(matchingFiles.Any(), "El archivo PDF de solicitudes de vacaciones debería haberse descargado con el nombre esperado.");
        }

        // Caso de prueba: Acciones-05 - Descargar las Vacaciones en Excel
        [Test]
        public void DownloadVacationRequestsAsExcel()
        {
            vacationModulePage.DownloadVacationRequestsExcel();
            Thread.Sleep(5000);

            string downloadPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

            string partialFileName = "SGRH  Sistema de Gestión de Recursos Humanos";

            var matchingFiles = Directory.GetFiles(downloadPath, "*.xlsx")
                                          .Where(file => Path.GetFileName(file).Contains(partialFileName))
                                          .ToList();

            Assert.IsTrue(matchingFiles.Any(), "El archivo Excel de solicitudes de vacaciones debería haberse descargado con el nombre esperado.");
        }

        [Test]
        public void SearchExistingVacationRecord()
        {
            string searchTerm = "Solicitud de Vacaciones";
            vacationModulePage.EnterSearchTerm(searchTerm);

            Thread.Sleep(2000);

            var results = vacationModulePage.GetSearchResults();
            Thread.Sleep(2000);

            Assert.IsTrue(results.Any(), "Debería haber al menos un resultado de búsqueda.");
            Assert.IsTrue(results.All(r => r.Text.Contains(searchTerm)), "Todos los resultados deben coincidir con el término de búsqueda.");
        }


    }
}


