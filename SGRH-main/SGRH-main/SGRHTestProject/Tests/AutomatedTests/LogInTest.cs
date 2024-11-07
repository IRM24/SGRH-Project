using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
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

        [SetUp]
        public void SetUp()
        {    
            logInPage = new LogInPage(driver);
            driver = logInPage.ChromeDriverConnection();
            logInPage.Visit("https://localhost:7122/Account/Login?ReturnUrl=%2F");
            driver.Manage().Window.Maximize(); 
        }

        [TearDown]
        public void TearDown()
        {
            //driver.Quit();
            //driver.Dispose();
        }


        // Caso de prueba: Autenticación-01 - Credenciales válidas
        [Test]
        public void LoginInWithValidCredentials()
        {
            string email = "cpicado869@gmail.com";
            string password = "Hola321!";
  
            logInPage.EnterEmail(email);
            logInPage.EnterPassword(password);
            logInPage.ClickLogin();
            Thread.Sleep(3000);

            string homePageMessage = logInPage.GetHomePageMessage();
            Assert.AreEqual("Dashboard Personal", homePageMessage);

        }


        // Caso de prueba: Autenticación-02 - Credenciales inválidas
        /*
        [Test]
        public void LoginInWithInvalidCredentials()
        {
            string email = "email_invalido@example.com";
            string password = "Contrasena123!";

            logInPage.EnterEmail(email);
            logInPage.EnterPassword(password);
            logInPage.ClickLogin();

            string errorMessage = logInPage.GetErrorMessage();
            Assert.AreEqual("Usuario o contraseña incorrecta", errorMessage, "Expected error message not displayed.");
        }
        */

        // Caso de prueba: Autenticación-03 - Recuperación de contraseña con un correo electrónico válido
        [Test]
        public void PasswordRecoveryWithValidEmail()
        {
            string email = "email@example.com";

            // Realizar las acciones de recuperación de contraseña
            logInPage.ClickForgotPasswordLink();
            logInPage.GenerateTemporaryPassword(email);

            // Verificar que se muestre un mensaje de confirmación
            string confirmationMessage = logInPage.GetConfirmationMessage();
            Assert.AreEqual("Se ha enviado una nueva contraseña temporal por correo electrónico, favor validar.", confirmationMessage);
        }

    }
}
