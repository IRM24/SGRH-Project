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
    public class UserModuleTest
    {
        private IWebDriver driver;
        private UserModulePage userModulePage;
        LogInPage logInPage;

        [SetUp]
        public void SetUp()
        {
            userModulePage = new UserModulePage(driver);
            driver = userModulePage.ChromeDriverConnection();
            userModulePage.Visit("https://localhost:7122/Account/Login?ReturnUrl=%2F");
            logInPage = new LogInPage(driver);
            driver.Manage().Window.Maximize();

            string email = "cpicado869@gmail.com";
            string password = "Hola321!";
            logInPage.LogIn(email, password);
            userModulePage.GoToUserModule();
        }

        [TearDown]
        public void TearDown()
        {
            //driver.Quit();
            driver.Dispose();
        }


        // Caso de prueba: Usuarios-01 - Registrar usuario con edad menor a 18 años
        [Test]
        public void RegisterUserUnderAge()
        {
            userModulePage.ClickRegisterUserButon();

            userModulePage.EnterName("Juana");
            userModulePage.EnterLastName("Lopez");
            userModulePage.SelectDepartment("Ventas");
            userModulePage.SelectPosition("Secretaria");
            userModulePage.SelectRole("Empleado");
            userModulePage.SelectWorkPeriod("Jornada Diurna");
            userModulePage.EnterBirthDate("01/01/2008");

            userModulePage.ClickCreateUserButton();
            Thread.Sleep(1000);

            string errorMessage = userModulePage.GetbirthDateErrorMessage();
            Assert.AreEqual("La fecha de nacimiento debe estar entre 01/01/1900 y hace 18 años.", errorMessage);
        }


        // Caso de prueba: Usuarios-04 - Registrar usuario sin seleccionar un departamento ni un puesto
        [Test]
        public void RegisterUserWithoutDepartmentAndPosition()
        {
            userModulePage.ClickRegisterUserButon();

            userModulePage.EnterName("Maria");
            userModulePage.EnterLastName("Gomez");
            userModulePage.EnterIdNumber("123123123");
            userModulePage.EnterPhoneNumber("1231231234");
            userModulePage.EnterEmail("mariag@correo.com");
            userModulePage.EnterBirthDate("01/01/2002");
            userModulePage.EnterHiringDate("01/01/2023");
            userModulePage.EnterBaseSalary("1500000");
            userModulePage.SelectRole("Empleado");
            userModulePage.SelectWorkPeriod("Jornada Diurna");

            userModulePage.ClickCreateUserButton();
            Thread.Sleep(1000);

            userModulePage.ClickConfirmUserCreation();

            string departmentErrorMessage = userModulePage.GetDepartmentFieldRequiredMessage();
            string positionErrorMessage = userModulePage.GetPositionFieldRequiredMessage();

            Assert.AreEqual("El campo Departamento es requerido.", departmentErrorMessage);
            Assert.AreEqual("El campo Puesto es requerido.", positionErrorMessage);
        }

        
        // Caso de prueba: Usuarios-05 - Buscar un usuario existente a través de la barra de búsqueda
        [Test]
        public void SearchUser()
        {
            string searchQuery = "Picado"; 

            userModulePage.Search(searchQuery);

            var searchResults = userModulePage.GetSearchResults();

            bool isUserFound = searchResults.Any(row => row.Any(cell => cell.Contains(searchQuery)));

            Assert.IsTrue(isUserFound, "No se encontraron resultados con el nombre o correo proporcionado.");
        }



        // Caso de prueba: Usuarios-06 - Editar datos de un usuario
        [Test]
        public void EditUserData()
        {
            string newLastName = "Garcia Lopez";
            string newEmail = "ggarcia@gmail.com";
            string newRole = "SupervisorDpto";

            userModulePage.Search("German Garcia");
            Thread.Sleep(1000);

            userModulePage.ShowInformationOfUser();

            userModulePage.ClickEditDataUserButton();

            userModulePage.EnterLastName(newLastName);
            userModulePage.EnterEmail(newEmail);
            userModulePage.SelectRole(newRole);

            userModulePage.ClickEditSaveUserButton();
            userModulePage.ClickConfirmEditButton();

            string confirmationMessage = userModulePage.GetUserEditedSucessMessage();
            Assert.AreEqual("Usuario editado de manera exitosa.", confirmationMessage);
        }



        // Caso de prueba: Usuarios-07 - Registrar un usuario con un salario base negativo
        [Test]
        public void RegisterUserWithNegativeBaseSalary()
        {
            userModulePage.ClickRegisterUserButon();

            userModulePage.EnterName("Juana");
            userModulePage.EnterLastName("Lopez");
            userModulePage.SelectDepartment("Ventas");
            userModulePage.SelectPosition("Secretaria");
            userModulePage.SelectRole("Empleado");
            userModulePage.SelectWorkPeriod("Jornada Diurna");

            userModulePage.EnterBaseSalary("-1");

            string errorMessage = userModulePage.GetSalaryErrorMessage();
            Assert.AreEqual("El salario base no puede ser negativo.", errorMessage);
        }


        // Caso de prueba: Usuarios-08 - Eliminar un usuario registrado
        [Test]
        public void DeleteUser()
        {
            string name = "Carlos Perez";
            string lastName = "Santos";
            string idNumber = "123456789";
            string birthDate = "01/01/1990";
            string phoneNumber = "1234567890";
            string email = "cperez@example.com";
            string department = "Ventas";
            string position = "Televentas";
            string role = "Empleado";
            string hiringDate = "01/01/2023";
            string baseSalary = "50000";
            string workPeriod = "Jornada Nocturna";

            userModulePage.RegisterUser(name, lastName, idNumber, birthDate, phoneNumber, email, department, position, role, hiringDate, baseSalary, workPeriod);
            Thread.Sleep(1000);

            
            userModulePage.Search("Carlos Perez");
            Thread.Sleep(5000);
            
            userModulePage.ShowInformationOfUser();
            userModulePage.ClickDeleteUserButton();

            userModulePage.ClickConfirmDeleteButton();

            string successMessage = userModulePage.GetUserDeletedSucessMessage();
            Assert.AreEqual("Usuario eliminado de manera exitosa.", successMessage);
        }



        // Caso de prueba: Usuarios-09 - Descargar los usuarios en formato PDF
        [Test]
        public void DownloadUsersAsPDF()
        {
            userModulePage.ClickDownloadPdfButton();

            string downloadedFilePath = UserModulePage.WaitForDownloadedFile("SGRH  Sistema de Gestión de Recursos Humanos.pdf", 10); 

            Assert.IsNotNull(downloadedFilePath, "El archivo PDF no se ha descargado correctamente.");
        }


        // Caso de prueba: Usuarios-10 - Descargar los usuarios en formato Excel
        [Test]
        public void DownloadUsersAsExcel()
        {
            userModulePage.ClickDownloadExcelButton();

            string downloadedFilePath = UserModulePage.WaitForDownloadedFile("SGRH  Sistema de Gestión de Recursos Humanos.xlsx", 10);

            Assert.IsNotNull(downloadedFilePath, "El archivo Excel no se ha descargado correctamente.");
        }


        // Caso de prueba: Usuarios-11 - Visualizar los detalles de un usuario registrado
        [Test]
        public void ViewUserDetails()
        {
            string userName = "Fabiana";
            userModulePage.Search(userName);
            userModulePage.ShowInformationOfUser();

            userModulePage.ClickViewDetailsButton();
            Thread.Sleep(500);

            var userDetails = userModulePage.GetUserDetails();
            Assert.AreEqual("Fabiana", userDetails["Nombre"], "El nombre del usuario no coincide.");
            Assert.AreEqual("fabiana0744@hotmail.com", userDetails["Email"], "El correo electrónico no coincide.");
            Assert.AreEqual("8920-5377", userDetails["NumeroTelefono"], "El número de contacto no coincide.");
            Assert.AreEqual("Gerencia de TI", userDetails["Puesto"], "El puesto no coincide.");
            Assert.AreEqual("Soporte", userDetails["Departamento"], "El departamento no coincide.");

        }


    }
}
