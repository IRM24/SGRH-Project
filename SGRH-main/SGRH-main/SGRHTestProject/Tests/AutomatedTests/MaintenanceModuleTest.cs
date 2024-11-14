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
            driver = new ChromeDriver();
            logInPage = new LogInPage(driver);
            maintenanceModulePage = new MaintenanceModulePage(driver);
            driver.Manage().Window.Maximize();

            // Iniciar sesión con un usuario autorizado
            logInPage.Visit("https://localhost:7122/Account/Login?ReturnUrl=%2F");
            logInPage.LogIn("cpicado869@gmail.com", "Hola321!");
            Thread.Sleep(3000); // Espera para que la página cargue completamente
        }

        [TearDown]
        public void TearDown()
        {
            driver.Close();
            driver.Dispose();
        }

        [Test]
        // Mantenimiento-01
        public void CreateDepartment()
        {
            // Acceder al módulo de mantenimiento
            maintenanceModulePage.GoToMaintenanceModule();
            Thread.Sleep(2000);

            // Ir a la sección de "Departamentos"
            maintenanceModulePage.GoToDepartmentsSection();
            Thread.Sleep(2000);

            // Seleccionar la opción para crear un nuevo departamento
            maintenanceModulePage.ClickCreateDepartment();
            Thread.Sleep(2000);

            // Ingresar el nombre del departamento
            maintenanceModulePage.EnterDepartmentName("Finanzas");

            // Confirmar la creación del departamento
            maintenanceModulePage.ClickCreate();
            Thread.Sleep(2000);

            // Verificar que el mensaje de éxito es el esperado
            string successMessage = maintenanceModulePage.GetSuccessMessage();
            Assert.AreEqual("Éxito", successMessage, "El mensaje de éxito no es el esperado.");
        }

        [Test]
        //Mantenimiento-02
        public void EditDepartment()
        {
            // Acceder al módulo de mantenimiento
            maintenanceModulePage.GoToMaintenanceModule();
            Thread.Sleep(2000);

            // Ir a la sección de "Departamentos"
            maintenanceModulePage.GoToDepartmentsSection();
            Thread.Sleep(2000);

            // Buscar el departamento "Alimentación" en la barra de búsqueda
            maintenanceModulePage.SearchDepartment("Finanzas");
            Thread.Sleep(2000);

            // Seleccionar la opción para editar desde los resultados de búsqueda
            maintenanceModulePage.ClickEditDepartment();
            Thread.Sleep(2000);

            // Ingresar el nuevo nombre del departamento
            maintenanceModulePage.EnterNewDepartmentName("Gerencia de Finanzas");

            // Confirmar la actualización del departamento
            maintenanceModulePage.ClickUpdate();
            Thread.Sleep(2000);

            // Verificar que el mensaje de éxito es el esperado
            string successMessage = maintenanceModulePage.GetSuccessMessage();
            Assert.AreEqual("Éxito", successMessage, "El mensaje de éxito no es el esperado.");
        }

        [Test]
        //Mantenimiento-04
        public void SearchExistingDepartment()
        {
            // Acceder al módulo de Mantenimiento
            maintenanceModulePage.GoToMaintenanceModule();
            Thread.Sleep(2000);

            // Ir a la sección de "Departamentos"
            maintenanceModulePage.GoToDepartmentsSection();
            Thread.Sleep(2000);

            // Buscar el departamento "Administrativos" en la barra de búsqueda
            maintenanceModulePage.SearchDepartment("Administrativos");
            Thread.Sleep(2000);

            // Verificar que el departamento se muestra en los resultados de búsqueda
            bool isDepartmentFound = maintenanceModulePage.IsDepartmentDisplayedInResults();
            Assert.IsTrue(isDepartmentFound, "El departamento 'Administrativos' no se encontró en los resultados de búsqueda.");
        }

        [Test]
        //Mantenimiento-03
        public void DeleteDepartment()
        {
            // Acceder al módulo de Mantenimiento
            maintenanceModulePage.GoToMaintenanceModule();
            Thread.Sleep(2000);

            // Ir a la sección de "Departamentos"
            maintenanceModulePage.GoToDepartmentsSection();
            Thread.Sleep(2000);

            // Buscar el departamento "Administrativos" en la barra de búsqueda
            maintenanceModulePage.SearchDepartment("Gerencia de Finanzas");
            Thread.Sleep(2000);

            // Seleccionar la opción "Eliminar" en el departamento deseado
            maintenanceModulePage.ClickDeleteDepartment();
            Thread.Sleep(2000);

            // Confirmar la eliminación del departamento
            maintenanceModulePage.ConfirmDelete();
            Thread.Sleep(5000);

            // Buscar de nuevo para asegurarse de que el departamento ya no existe
            maintenanceModulePage.SearchDepartment("Gerencia de Finanzas");
            Thread.Sleep(3000);
            bool isDepartmentStillPresent = maintenanceModulePage.IsDepartmentDisplayedInResults();

            // Verificar que el departamento ya no esté en la lista
            Assert.IsFalse(isDepartmentStillPresent, "El departamento 'Gerencia de Finanzas' aún aparece en la lista después de la eliminación.");
        }

        [Test]
        //Mantenimiento-08
        public void SearchExistingPosition()
        {
            // Acceder al módulo de Mantenimiento
            maintenanceModulePage.GoToMaintenanceModule();
            Thread.Sleep(2000);

            // Ir a la sección de "Puestos"
            maintenanceModulePage.GoToPositionsSection();
            Thread.Sleep(2000);

            // Dirigirse a la barra de búsqueda e ingresar el nombre del puesto "Auditor"
            maintenanceModulePage.SearchPosition("Auditor");
            Thread.Sleep(2000);

            // Verificar que el puesto "Auditor" aparece en los resultados de la búsqueda en tiempo real
            bool isPositionPresent = maintenanceModulePage.IsPositionDisplayedInResults();

            // Afirmar que el puesto está presente en los resultados
            Assert.IsTrue(isPositionPresent, "El puesto 'Auditor' no se muestra en los resultados de la búsqueda.");
        }

        [Test]
        //Mantenimiento-07
        public void DeletePosition()
        {
            // Acceder al módulo de Mantenimiento
            maintenanceModulePage.GoToMaintenanceModule();
            Thread.Sleep(2000);

            // Ir a la sección de "Puestos"
            maintenanceModulePage.GoToPositionsSection();
            Thread.Sleep(2000);

            // Dirigirse a la barra de búsqueda e ingresar el nombre del puesto "Auditor"
            maintenanceModulePage.SearchPosition("Gerencia de Recursos Humanos");
            Thread.Sleep(2000);

            // Seleccionar la opción de "Eliminar" para el puesto "Auditor"
            maintenanceModulePage.DeletePosition();
            Thread.Sleep(1000);

            // Confirmar la eliminación en el modal de confirmación
            maintenanceModulePage.ConfirmDeletePosition();
            Thread.Sleep(2000);

            // Verificar que el mensaje de éxito "Eliminado con éxito" se muestra
            string successMessage = maintenanceModulePage.GetSuccessMessage();
            Assert.AreEqual("Éxito", successMessage, "El mensaje de éxito no es el esperado.");
        }


    }
}
