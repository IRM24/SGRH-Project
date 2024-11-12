using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
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
    public class LogInTest
    {
        private IWebDriver driver;
        private LogInPage logInPage;
        UserModulePage userModulePage;

        [SetUp]
        public void SetUp()
        {    
            logInPage = new LogInPage(driver);
            driver = logInPage.ChromeDriverConnection();
            logInPage.Visit("https://localhost:7122/Account/Login?ReturnUrl=%2F");
            userModulePage = new UserModulePage(driver);
            driver.Manage().Window.Maximize(); 
        }

        [TearDown]
        public void TearDown()
        {
            //driver.Quit();
            driver.Dispose();
        }


        // Caso de prueba: Autenticación-01 - Credenciales válidas
        [Test]
        public void LoginInWithValidCredentials()
        {
            string email = "cpicado869@gmail.com";
            string password = "Hola321!";
  
            logInPage.LogIn(email, password);
            Thread.Sleep(3000);

            string homePageMessage = logInPage.GetHomePageMessage();
            Assert.AreEqual("Dashboard Personal", homePageMessage);

        }


        // Caso de prueba: Autenticación-02 - Credenciales inválidas
        [Test]
        public void LoginInWithInvalidCredentials()
        {
            string email = "email_invalido@example.com";
            string password = "Contrasena123!";

            logInPage.LogIn(email, password);

            string errorMessage = logInPage.GetErrorMessage();
            Assert.AreEqual("Usuario o contraseña incorrecta", errorMessage, "Expected error message not displayed.");
        }
        

        // Caso de prueba: Autenticación-03 - Recuperación de contraseña con un correo electrónico válido
        [Test]
        public void PasswordRecoveryWithValidEmail()
        {
            string email = "email@example.com";

            // Realizar las acciones de recuperación de contraseña
            logInPage.ClickForgotPasswordLink();
            logInPage.GenerateTemporaryPassword(email);

            // Verificar que se muestre un mensaje de confirmación
            string confirmationMessage = logInPage.GetSucessTemporaryPasswordMessage();
            Assert.AreEqual("Se ha enviado una nueva contraseña temporal por correo electrónico, favor validar.", confirmationMessage);
        }


        // Caso de prueba: Autenticación-04 - Recuperación de contraseña con un correo electrónico no registrado
        [Test]
        public void PasswordRecoveryWithInvalidEmail()
        {
            string email = "email_invalido@example.com";

            // Realizar las acciones de recuperación de contraseña
            logInPage.ClickForgotPasswordLink();
            logInPage.GenerateTemporaryPassword(email);

            // Verificar que se muestre un mensaje de confirmación
            string confirmationMessage = logInPage.GetErrorTemporaryPasswordMessage();
            Assert.AreEqual("No se encontró un usuario con el correo electrónico proporcionado.", confirmationMessage);
        }


        // Caso de prueba: Autenticación-05 - Recuperación de contraseña con campo de correo electrónico vacío
        [Test]
        public void PasswordRecoveryWithEmptyEmailField()
        {
            logInPage.ClickForgotPasswordLink();
            logInPage.GenerateTemporaryPassword(""); 
            string errorMessage = logInPage.GetErrorTemporaryPasswordMessage();
            Assert.AreEqual("Por favor, ingrese un correo electrónico.", errorMessage);  // Ajusta el mensaje según la aplicación
        }


        // Caso de prueba: Autenticación-06 - Validar acceso al módulo de usuarios con cuenta de administrador
        [Test]
        public void AccessToResourcesWithAdminUser
()
        {
            string email = "cpicado869@gmail.com";
            string password = "Hola321!";

            logInPage.LogIn(email, password);
            Thread.Sleep(3000);

            userModulePage.GoToUserModule();
            Thread.Sleep(3000);

            string headerMessage = userModulePage.GetUserModuleHeaderMessage();
            Assert.AreEqual("Gestión de Usuarios", headerMessage);
        }



        // Caso de prueba: Autenticación-07 - Denegar acceso a recursos no autorizados para empleados
        [Test]
        public void AccessToResourcesDeniedForEmployeeRole()
        {
            string email = "hpicado@test.com";
            string password = "Hola321!";

            logInPage.LogIn(email, password);
            Thread.Sleep(3000);

            bool isUserModuleVisible = userModulePage.IsUserModuleVisible();
            Assert.IsFalse(isUserModuleVisible, "El usuario de rol empleado no debería tener acceso al módulo de usuarios.");

        }



        // Caso de prueba: Autenticación-08 - Bloqueo de cuenta después de tres intentos fallidos
        [Test]
        public void AccountLockoutAfterFailedAttempts()
        {
            string email = "fabiana0744@hotmail.com";
            string password = "Hola3321!";

            logInPage.EnterEmail(email);
            for (int i = 0; i < 3; i++)
            {
                logInPage.EnterPassword(password);
                logInPage.ClickLogin();
                Thread.Sleep(2000);
            }
            Thread.Sleep(2000);
            // Verificar si aparece el mensaje de bloqueo temporal
            string lockoutMessage = logInPage.GetAccountLockoutMessage();
            Assert.AreEqual("La cuenta está bloqueada debido a demasiados intentos fallidos de inicio de sesión. Por favor, intente en otro momento.", lockoutMessage, "La cuenta debería estar bloqueada después de tres intentos fallidos.");
        }


        

        // Caso de prueba: Autenticación-10 - El usuario puede cerrar sesión de manera manual
        [Test]
        public void UserLogOutManually()
        {
            string email = "cpicado869@gmail.com";
            string password = "Hola321!";

            logInPage.LogIn(email, password);
            Thread.Sleep(3000);

            string homePageMessage = logInPage.GetHomePageMessage();
            Assert.AreEqual("Dashboard Personal", homePageMessage);

            // Hacer clic en el botón de cerrar sesión
            logInPage.ClickLogout();
            Thread.Sleep(3000);

            // Verificar que la página haya redirigido a la página de inicio de sesión
            string loginPageTitle = logInPage.GetLoginPageTitle();
            Assert.AreEqual("Ingrese sus credenciales para iniciar sesión", loginPageTitle);
        }
        
    }
}
