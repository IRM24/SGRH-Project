using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SGRHTestProject.Pages;
using System.Threading;

namespace SGRHTestProject.Tests.AutomatedTests
{
    [TestFixture]
    public class MaintenanceModuleTest
    {
        private IWebDriver driver;
        private LogInPage logInPage;
        private MaintenanceModulePage maintenanceModulePage;

        [SetUp]
        public void SetUp()
        {
            maintenanceModulePage = new MaintenanceModulePage(driver);
            driver = maintenanceModulePage.ChromeDriverConnection();
            maintenanceModulePage.Visit("https://localhost:7122/Account/Login?ReturnUrl=%2F");
            logInPage = new LogInPage(driver);
            driver.Manage().Window.Maximize();

            logInPage.LogIn("cpicado869@gmail.com", "Hola321!");
            Thread.Sleep(3000);
            maintenanceModulePage.GoToMaintenanceModule();
            Thread.Sleep(2000);
        }

        [TearDown]
        public void TearDown()
        {
            //driver.Close();
            driver.Dispose();
        }


        // Caso de prueba: Mantenimiento-01 - Crear un departamento
        [Test]
        public void CreateDepartment()
        {
            maintenanceModulePage.GoToDepartmentsSection();
            Thread.Sleep(2000);

            maintenanceModulePage.ClickCreateDepartment();
            Thread.Sleep(2000);

            maintenanceModulePage.EnterDepartmentName("Finanzas");

            maintenanceModulePage.ClickCreate();
            Thread.Sleep(2000);

            string successMessage = maintenanceModulePage.GetSuccessMessage();
            Assert.AreEqual("Éxito", successMessage, "El mensaje de éxito no es el esperado.");
        }


        // Caso de prueba: Mantenimiento-02 - Editar un departamento
        [Test]
        public void EditDepartment()
        {
            maintenanceModulePage.GoToDepartmentsSection();
            Thread.Sleep(2000);

            maintenanceModulePage.SearchDepartment("Finanzas");
            Thread.Sleep(2000);

            maintenanceModulePage.ClickEditDepartment();
            Thread.Sleep(2000);

            maintenanceModulePage.EnterNewDepartmentName("Gerencia de Finanzas");

            maintenanceModulePage.ClickUpdate();
            Thread.Sleep(2000);

            string successMessage = maintenanceModulePage.GetSuccessMessage();
            Assert.AreEqual("Éxito", successMessage, "El mensaje de éxito no es el esperado.");
        }


        // Caso de prueba: Mantenimiento-03 - Eliminar un departamento
        [Test]
        public void DeleteDepartment()
        {
            maintenanceModulePage.GoToDepartmentsSection();
            Thread.Sleep(2000);

            maintenanceModulePage.SearchDepartment("Gerencia de Finanzas");
            Thread.Sleep(2000);

            maintenanceModulePage.ClickDeleteDepartment();
            Thread.Sleep(2000);

            maintenanceModulePage.ConfirmDelete();
            Thread.Sleep(5000);

            maintenanceModulePage.SearchDepartment("Gerencia de Finanzas");
            Thread.Sleep(3000);
            bool isDepartmentStillPresent = maintenanceModulePage.IsDepartmentDisplayedInResults();

            Assert.IsFalse(isDepartmentStillPresent, "El departamento 'Gerencia de Finanzas' aún aparece en la lista después de la eliminación.");
        }


        // Caso de prueba: Mantenimiento-04 - Buscar un departamento existente
        [Test]
        public void SearchExistingDepartment()
        {
            maintenanceModulePage.GoToDepartmentsSection();
            Thread.Sleep(2000);

            maintenanceModulePage.SearchDepartment("Administrativos");
            Thread.Sleep(2000);

            bool isDepartmentFound = maintenanceModulePage.IsDepartmentDisplayedInResults();
            Assert.IsTrue(isDepartmentFound, "El departamento 'Administrativos' no se encontró en los resultados de búsqueda.");
        }


        // Caso de prueba: Mantenimiento-07 - Eliminar un puesto
        [Test]
        public void DeletePosition()
        {
            maintenanceModulePage.GoToPositionsSection();
            Thread.Sleep(2000);

            maintenanceModulePage.SearchPosition("Gerencia de Recursos Humanos");
            Thread.Sleep(2000);

            maintenanceModulePage.DeletePosition();
            Thread.Sleep(1000);

            maintenanceModulePage.ConfirmDeletePosition();
            Thread.Sleep(2000);

            string successMessage = maintenanceModulePage.GetSuccessMessage();
            Assert.AreEqual("Éxito", successMessage, "El mensaje de éxito no es el esperado.");
        }


        // Caso de prueba: Mantenimiento-08 - Buscar un puesto existente
        [Test]
        public void SearchExistingPosition()
        {
            maintenanceModulePage.GoToPositionsSection();
            Thread.Sleep(2000);

            maintenanceModulePage.SearchPosition("Auditor");
            Thread.Sleep(2000);

            bool isPositionPresent = maintenanceModulePage.IsPositionDisplayedInResults();

            Assert.IsTrue(isPositionPresent, "El puesto 'Auditor' no se muestra en los resultados de la búsqueda.");
        }


    }
}
