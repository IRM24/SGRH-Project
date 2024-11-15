using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SGRHTestProject.Pages;
using System;
using System.Threading;

namespace SGRHTestProject.Tests.AutomatedTests
{
    [TestFixture]
    public class LayoffsModuleTest
    {
        private IWebDriver driver;
        private LogInPage logInPage;
        private LayoffsModulePage layoffsModulePage;

        [SetUp]
        public void SetUp()
        {
            layoffsModulePage = new LayoffsModulePage(driver);
            driver = layoffsModulePage.ChromeDriverConnection();
            layoffsModulePage.Visit("https://localhost:7122/Account/Login?ReturnUrl=%2F");
            logInPage = new LogInPage(driver);
            driver.Manage().Window.Maximize();

            logInPage.LogIn("cpicado869@gmail.com", "Hola321!");
            Thread.Sleep(3000);
            layoffsModulePage.GoToActionsManagement();
            Thread.Sleep(2000);
        }

        [TearDown]
        public void TearDown()
        {
            //driver.Close();
            driver.Dispose();
        }


        // Caso de prueba: Acciones-15 - Registrar despido con responsabilidad del empleador
        [Test]
        public void RegisterLayoffWithEmployerResponsibility()
        {
            layoffsModulePage.GoToLayoffsSection();
            Thread.Sleep(2000);

            layoffsModulePage.ClickRegisterLayoff();
            Thread.Sleep(2000);

            layoffsModulePage.SearchEmployee("Michael");
            Thread.Sleep(6000);

            layoffsModulePage.EnterLayoffDetails("11-30-2024", "Reestructuración de la empresa");

            layoffsModulePage.MarkEmployerResponsibility();
            Thread.Sleep(1000);

            layoffsModulePage.ClickCreateLayoff();
            Thread.Sleep(2000);

            layoffsModulePage.ConfirmLayoffCreation();
            Thread.Sleep(2000);

            string successMessage = layoffsModulePage.GetSuccessMessage();
            Assert.AreEqual("Éxito", successMessage, "El mensaje de éxito no es el esperado.");
        }


        // Caso de prueba: Acciones-17 - Registrar despido sin responsabilidad del empleador
        [Test]
        public void RegisterLayoffWithoutEmployerResponsibility()
        {
            layoffsModulePage.GoToLayoffsSection();
            Thread.Sleep(2000);

            layoffsModulePage.ClickRegisterLayoff();
            Thread.Sleep(2000);

            layoffsModulePage.SearchEmployee("Xinia");
            Thread.Sleep(6000);

            layoffsModulePage.EnterLayoffDetails("11-30-2024", "Comportamiento inapropiado");
            Thread.Sleep(1000);

            layoffsModulePage.ClickCreateLayoff();
            Thread.Sleep(2000);

            layoffsModulePage.ConfirmLayoffCreation();
            Thread.Sleep(2000);

            string successMessage = layoffsModulePage.GetSuccessMessage();
            Assert.AreEqual("Éxito", successMessage, "El mensaje de éxito no es el esperado.");
        }


        // Caso de prueba: Acciones-16 - Registrar despido de un empleado que ya está despedido.
        [Test]
        public void RegisterLayoffForEmployeeAlreadyLaidOff()
        {
            layoffsModulePage.GoToLayoffsSection();
            Thread.Sleep(2000);

            layoffsModulePage.ClickRegisterLayoff();
            Thread.Sleep(2000);

            layoffsModulePage.SearchEmployee("Fabiana");
            Thread.Sleep(6000);

            layoffsModulePage.EnterLayoffDetails("11-30-2024", "Reestructuración de la empresa");
            Thread.Sleep(1000);

            layoffsModulePage.ClickCreateLayoff();
            Thread.Sleep(2000);

            layoffsModulePage.ConfirmLayoffCreation();
            Thread.Sleep(2000);

            Assert.AreEqual("Ya existe un despido registrado para este usuario.", layoffsModulePage.GetErrorMessage(), "El mensaje de error no es el esperado.");
        }

        // Caso de prueba: Acciones-19 - Eliminar un registro de despido.
        [Test]
        public void DeleteLayoff()
        {
            layoffsModulePage.GoToLayoffsSection();
            Thread.Sleep(2000);

            layoffsModulePage.ClickListLayoff();
            Thread.Sleep(2000);

            layoffsModulePage.EnterSearchLayoff("Michael");
            Thread.Sleep(4000); 

            layoffsModulePage.ClickOptionsLayoff();
            Thread.Sleep(1000);

            layoffsModulePage.ClickDeleteLayoff();
            Thread.Sleep(2000);

            layoffsModulePage.ConfirmLayoffCreation();
            Thread.Sleep(2000);

            string successMessage = layoffsModulePage.GetSuccessMessage();
            Assert.AreEqual("Éxito", successMessage, "El mensaje de éxito no es el esperado.");
        }

    }
}
