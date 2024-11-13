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
            driver = new ChromeDriver();
            logInPage = new LogInPage(driver);
            layoffsModulePage = new LayoffsModulePage(driver);
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
        
        //Acciones 15
        // Caso de prueba: Registrar despido con responsabilidad del empleador
        [Test]
        public void RegisterLayoffWithEmployerResponsibility()
        {
            // Acceder al módulo de gestión de acciones
            layoffsModulePage.GoToActionsManagement();
            Thread.Sleep(2000);

            // Ir a la sección "Despidos"
            layoffsModulePage.GoToLayoffsSection();
            Thread.Sleep(2000);

            // Presionar el botón para registrar un despido
            layoffsModulePage.ClickRegisterLayoff();
            Thread.Sleep(2000);

            // Buscar al empleado por su identificador (por ejemplo, DNI o número de empleado)
            layoffsModulePage.SearchEmployee("Michael");
            Thread.Sleep(6000);

            // Cerrar el mensaje SweetAlert

            // Ingresar la fecha y razón del despido
            layoffsModulePage.EnterLayoffDetails("11-30-2024", "Reestructuración de la empresa");

            // Marcar la casilla de responsabilidad del empleador
            layoffsModulePage.MarkEmployerResponsibility();
            Thread.Sleep(1000);

            // Hacer clic en el botón para crear el despido
            layoffsModulePage.ClickCreateLayoff();
            Thread.Sleep(2000);

            // Confirmar la creación del despido en el modal
            layoffsModulePage.ConfirmLayoffCreation();
            Thread.Sleep(2000);

            // Validar que el despido fue registrado exitosamente (esto se haría comprobando un mensaje de confirmación o redirección)
            // Aquí deberías verificar un mensaje de éxito o alguna otra confirmación visual en la página
            //string confirmationMessage = driver.FindElement(By.CssSelector("selector_del_mensaje")).Text;
            // Declarar la variable successMessage como IWebElement
            string successMessage = layoffsModulePage.GetSuccessMessage();

            // Verificar que el texto del mensaje de éxito es el esperado
            Assert.AreEqual("Éxito", successMessage, "El mensaje de éxito no es el esperado.");
        }

        //Acciones 17
        // Caso de prueba: Registrar despido sin responsabilidad del empleador
        [Test]
        public void RegisterLayoffWithoutEmployerResponsibility()
        {
            // Acceder al módulo de gestión de acciones
            layoffsModulePage.GoToActionsManagement();
            Thread.Sleep(2000);

            // Ir a la sección "Despidos"
            layoffsModulePage.GoToLayoffsSection();
            Thread.Sleep(2000);

            // Presionar el botón para registrar un despido
            layoffsModulePage.ClickRegisterLayoff();
            Thread.Sleep(2000);

            // Buscar al empleado por su identificador (por ejemplo, DNI o número de empleado)
            layoffsModulePage.SearchEmployee("Xinia");
            Thread.Sleep(6000);

            // Cerrar el mensaje SweetAlert

            // Ingresar la fecha y razón del despido
            layoffsModulePage.EnterLayoffDetails("11-30-2024", "Comportamiento inapropiado");

            Thread.Sleep(1000);

            // Hacer clic en el botón para crear el despido
            layoffsModulePage.ClickCreateLayoff();
            Thread.Sleep(2000);

            // Confirmar la creación del despido en el modal
            layoffsModulePage.ConfirmLayoffCreation();
            Thread.Sleep(2000);

            // Validar que el despido fue registrado exitosamente (esto se haría comprobando un mensaje de confirmación o redirección)
            // Aquí deberías verificar un mensaje de éxito o alguna otra confirmación visual en la página
            //string confirmationMessage = driver.FindElement(By.CssSelector("selector_del_mensaje")).Text;
            // Declarar la variable successMessage como IWebElement
            string successMessage = layoffsModulePage.GetSuccessMessage();

            // Verificar que el texto del mensaje de éxito es el esperado
            Assert.AreEqual("Éxito", successMessage, "El mensaje de éxito no es el esperado.");
        }

        //Acciones 16
        [Test]
        public void RegisterLayoffForEmployeeAlreadyLaidOff()
        {
            // Acceder al módulo de gestión de acciones
            layoffsModulePage.GoToActionsManagement();
            Thread.Sleep(2000);

            // Ir a la sección "Despidos"
            layoffsModulePage.GoToLayoffsSection();
            Thread.Sleep(2000);

            // Presionar el botón para registrar un despido
            layoffsModulePage.ClickRegisterLayoff();
            Thread.Sleep(2000);

            // Buscar al empleado por nombre (en este caso "Luis Castro")
            layoffsModulePage.SearchEmployee("Fabiana");
            Thread.Sleep(6000); // Esperar a que cargue el empleado en la lista


            // Ingresar la fecha y razón del despido
            layoffsModulePage.EnterLayoffDetails("11-30-2024", "Reestructuración de la empresa");

            Thread.Sleep(1000);

            // Hacer clic en el botón para crear el despido
            layoffsModulePage.ClickCreateLayoff();
            Thread.Sleep(2000);

            // Confirmar la creación del despido en el modal
            layoffsModulePage.ConfirmLayoffCreation();
            Thread.Sleep(2000);

            // Verificar que el mensaje de error se muestra correctamente
            string errorMessage = driver.FindElement(By.CssSelector(".swal2-html-container")).Text;

            // Verificar que el mensaje de error es el esperado
            Assert.AreEqual("Ya existe un despido registrado para este usuario.", errorMessage, "El mensaje de error no es el esperado.");
        }

        //Acciones 19
        [Test]
        public void DeleteLayoff()
        {
            // Acceder al módulo de gestión de acciones
            layoffsModulePage.GoToActionsManagement();
            Thread.Sleep(2000);

            // Ir a la sección "Despidos"
            layoffsModulePage.GoToLayoffsSection();
            Thread.Sleep(2000);

            // Seleccionar la opción "Listado de despidos"
            layoffsModulePage.ClickListLayoff();
            Thread.Sleep(2000);

            // Buscar el registro de despido del empleado (por ejemplo, empleado "Michael")
            layoffsModulePage.EnterSearchLayoff("Jean");
            Thread.Sleep(4000); // Espera a que cargue el empleado en la lista

            layoffsModulePage.ClickOptionsLayoff();
            Thread.Sleep(1000);

            // Hacer clic en el botón "Eliminar" asociado al registro del despido
            layoffsModulePage.ClickDeleteLayoff();
            Thread.Sleep(2000);

            // Confirmar la eliminación en la ventana emergente
            layoffsModulePage.ConfirmLayoffCreation(); // Suponemos que el botón "OK" es el mismo para confirmación
            Thread.Sleep(2000);

            // Validar que el registro de despido ha sido eliminado exitosamente
            // Verificar si el mensaje de éxito aparece
            string successMessage = layoffsModulePage.GetSuccessMessage();

            // Verificar que el mensaje de éxito es el esperado
            Assert.AreEqual("Éxito", successMessage, "El mensaje de éxito no es el esperado.");

            // Opcionalmente, también podrías verificar que el empleado ya no aparece en el listado de despidos
            // (esto depende de la implementación de la lista de despidos y si el sistema muestra o no un mensaje de error en caso de que no exista)
        }

    }
}
