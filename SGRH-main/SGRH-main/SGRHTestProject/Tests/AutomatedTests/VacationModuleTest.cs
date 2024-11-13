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
            driver = new ChromeDriver();
            logInPage = new LogInPage(driver);
            userModulePage = new UserModulePage(driver);
            vacationModulePage = new VacationModulePage(driver);
            driver.Manage().Window.Maximize();

            // Iniciar sesión con un usuario de empleado autorizado
            logInPage.Visit("https://localhost:7122/Account/Login?ReturnUrl=%2F");
            logInPage.LogIn("cpicado869@gmail.com", "Hola321!");
            Thread.Sleep(3000); // Espera hasta que la página cargue completamente
        }

        [TearDown]
        public void TearDown()
        {
            driver.Close();
            driver.Dispose();
        }

        // Caso de prueba: Acciones-01 - Solicitar días de vacaciones en un rango de fechas inválido
        [Test]
        public void RequestVacationWithInvalidDateRange()
        {
            // Acceder al módulo de gestión de acciones
            vacationModulePage.GoToGestionAcciones();
            Thread.Sleep(2000);

            // Ir a la sección "Vacaciones" y seleccionar "Mis Solicitudes"
            vacationModulePage.GoToVacacionesSection();
            Thread.Sleep(2000);

            vacationModulePage.GoToMisSolicitudes();
            Thread.Sleep(2000);


            // Localizar y presionar el botón de "Solicitar Vacaciones"
            vacationModulePage.ClickSolicitarVacaciones();

            Thread.Sleep(2000);

            string currentUrl = driver.Url;

            // Seleccionar un rango de fechas inválido (por ejemplo, fecha de fin antes de la fecha de salida)
            vacationModulePage.EnterVacationDates("11-30-2024", "11-29-2024");

            vacationModulePage.EnterVacationDescriptionComments("solicitud de vacaciones", "solicito respuesta lo antes posible porfavor");

            // Intentar enviar la solicitud
            vacationModulePage.SubmitVacationRequest();
            Thread.Sleep(2000);

            string errorMessage = vacationModulePage.GetErrorMessageForDescription();
            Assert.AreEqual("La fecha de regreso no puede ser anterior a la fecha de salida.", errorMessage, "Debería aparecer un mensaje de error indicando que la fecha no puede ser anterior al día de hoy.");
        }

        // Caso de prueba: Acciones-02 - Solicitar vacaciones sin descripción ni comentarios adicionales
        [Test]
        public void RequestVacationWithoutDescriptionOrComments()
        {
            // Acceder al módulo de gestión de acciones
            vacationModulePage.GoToGestionAcciones();
            Thread.Sleep(2000);

            // Ir a la sección "Vacaciones" y seleccionar "Mis Solicitudes"
            vacationModulePage.GoToVacacionesSection();
            Thread.Sleep(2000);

            vacationModulePage.GoToMisSolicitudes();
            Thread.Sleep(2000);

            // Localizar y presionar el botón de "Solicitar Vacaciones"
            vacationModulePage.ClickSolicitarVacaciones();
            Thread.Sleep(2000);

            // Seleccionar un rango de fechas válido
            vacationModulePage.EnterVacationDates("12-16-2024", "12-18-2024");

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
            // Acceder al módulo de gestión de acciones
            vacationModulePage.GoToGestionAcciones();
            Thread.Sleep(2000);

            // Ir a la sección "Vacaciones" y seleccionar "Mis Solicitudes"
            vacationModulePage.GoToVacacionesSection();
            Thread.Sleep(2000);

            vacationModulePage.GoToMisSolicitudes();
            Thread.Sleep(2000);

            // Localizar y presionar el botón de "Solicitar Vacaciones"
            vacationModulePage.ClickSolicitarVacaciones();
            Thread.Sleep(2000);

            // Seleccionar un rango de fechas que exceda el saldo disponible de días de vacaciones
            // Suponiendo que el saldo disponible es inferior a la cantidad de días seleccionados
            vacationModulePage.EnterVacationDates("12-05-2024", "01-05-2025"); // Selecciona un rango de fechas largo

            vacationModulePage.EnterVacationDescriptionComments("solicitud de vacaciones", "solicito respuesta lo antes posible porfavor");

            // Intentar enviar la solicitud
            vacationModulePage.SubmitVacationRequest();
            Thread.Sleep(2000);

            // Verificar que el sistema no permite realizar la solicitud y se mantiene en el formulario
            //string currentUrl = driver.Url;
            //Assert.AreEqual(currentUrl, driver.Url, "El URL debería permanecer igual si la solicitud falla por exceder el saldo de días disponibles.");

            string errorMessage = vacationModulePage.GetErrorMessageForBalance();
            Assert.IsTrue(errorMessage.Contains("Lo sentimos, no cuenta con suficientes días disponibles."),
                          "Se esperaba un mensaje de error indicando que el saldo es insuficiente.");
        }

        //Acciones 04
        [Test]
        public void DownloadVacationRequestsAsPDF()
        {
            // Acceder al módulo de gestión de acciones
            vacationModulePage.GoToGestionAcciones();
            Thread.Sleep(2000);

            // Ir a la sección "Vacaciones" y seleccionar "Mis Solicitudes"
            vacationModulePage.GoToVacacionesSection();
            Thread.Sleep(2000);

            vacationModulePage.GoToMisSolicitudes();
            Thread.Sleep(2000);

            // Descargar el PDF de solicitudes de vacaciones
            vacationModulePage.DownloadVacationRequestsPDF();
            Thread.Sleep(5000); // Espera a que el archivo PDF se descargue

            // Obtener la ruta de la carpeta de descargas de Windows
            string downloadPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

            string partialFileName = "SGRH  Sistema de Gestión de Recursos Humanos";

            // Obtener archivos que contengan el nombre parcial en la carpeta de descargas
            var matchingFiles = Directory.GetFiles(downloadPath, "*.pdf")
                                          .Where(file => Path.GetFileName(file).Contains(partialFileName))
                                          .ToList();

            // Verificar que existe al menos un archivo que contiene el nombre parcial
            Assert.IsTrue(matchingFiles.Any(), "El archivo PDF de solicitudes de vacaciones debería haberse descargado con el nombre esperado.");
        }

        //Acciones 05

        [Test]
        public void DownloadVacationRequestsAsExcel()
        {
            // Acceder al módulo de gestión de acciones
            vacationModulePage.GoToGestionAcciones();
            Thread.Sleep(2000);

            // Ir a la sección "Vacaciones" y seleccionar "Mis Solicitudes"
            vacationModulePage.GoToVacacionesSection();
            Thread.Sleep(2000);

            vacationModulePage.GoToMisSolicitudes();
            Thread.Sleep(2000);

            // Descargar el Excel de solicitudes de vacaciones
            vacationModulePage.DownloadVacationRequestsExcel();
            Thread.Sleep(5000); // Espera a que el archivo Excel se descargue

            // Obtener la ruta de la carpeta de descargas de Windows
            string downloadPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

            string partialFileName = "SGRH  Sistema de Gestión de Recursos Humanos";

            // Obtener archivos que contengan el nombre parcial en la carpeta de descargas
            var matchingFiles = Directory.GetFiles(downloadPath, "*.xlsx")
                                          .Where(file => Path.GetFileName(file).Contains(partialFileName))
                                          .ToList();

            // Verificar que existe al menos un archivo que contiene el nombre parcial
            Assert.IsTrue(matchingFiles.Any(), "El archivo Excel de solicitudes de vacaciones debería haberse descargado con el nombre esperado.");
        }

        [Test]
        public void SearchExistingVacationRecord()
        {
            // Acceder al módulo de gestión de acciones
            vacationModulePage.GoToGestionAcciones();
            Thread.Sleep(2000);

            // Ir a la sección "Vacaciones" y seleccionar "Mis Solicitudes"
            vacationModulePage.GoToVacacionesSection();
            Thread.Sleep(2000);

            vacationModulePage.GoToMisSolicitudes();
            Thread.Sleep(2000);

            // Realizar una búsqueda en la barra de búsqueda con el término deseado
            string searchTerm = "Solicitud de Vacaciones"; // Cambia esto al término que quieras buscar
            vacationModulePage.EnterSearchTerm(searchTerm);

            // Espera a que se muestren los resultados en tiempo real
            Thread.Sleep(2000); // Espera breve para permitir que los resultados se actualicen

            // Verificar que los resultados coinciden con el término de búsqueda
            var results = vacationModulePage.GetSearchResults();
            Thread.Sleep(2000); // Espera breve para permitir que los resultados se actualicen

            Assert.IsTrue(results.Any(), "Debería haber al menos un resultado de búsqueda.");
            Assert.IsTrue(results.All(r => r.Text.Contains(searchTerm)), "Todos los resultados deben coincidir con el término de búsqueda.");
        }


    }
}


