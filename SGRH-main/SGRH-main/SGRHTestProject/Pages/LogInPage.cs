using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGRHTestProject.Pages
{
    public class LogInPage : Base
    { 
        private By emailLocator => By.Id("UserName");
        private By passwordLocator => By.Id("Password");

        private By loginBtnLocator = By.CssSelector("button[type='submit'].btn.btn-primary.btn-block");

        private By dashboardTextLocator = By.XPath("//h1[text()='Dashboard Personal']");

        private By forgotPasswordLocator = By.LinkText("¿Olvidaste tu contraseña?");

        private By generateTemporaryPassLocator = By.ClassName("btn-secondary");

        private By messageTemporarySuccesPassword = By.ClassName("alert-success");

        private By messageTemporaryErrorPassword = By.ClassName("alert-danger");


        public LogInPage(IWebDriver driver) : base(driver)
        {
        }


        public void EnterEmail(string username)
        {
            Type(username, emailLocator);
        }

        public void EnterPassword(string password)
        {
            Type(password, passwordLocator);
        }

        public void ClickLogin()
        {
            Click(loginBtnLocator);
        }

        public String GetHomePageMessage()
        {
            var homePageMessageElement = FindElement(dashboardTextLocator);
            return homePageMessageElement.Text;

        }


        public string GetErrorMessage()
        {
            try
            {
                var errorMessageElement = FindElement(By.XPath("//*[contains(text(), 'Usuario o contraseña incorrecta')]")
);
                return errorMessageElement.Text;
            }
            catch (NoSuchElementException)
            {
                return null;
            }
        }



        public void ClickForgotPasswordLink()
        {
            Click(forgotPasswordLocator);
        }


        public void GenerateTemporaryPassword(String email)
        {
            Type(email, emailLocator);
            Click(generateTemporaryPassLocator);

        }


        public string GetSucessTemporaryPasswordMessage()
        {
            var message = FindElement(messageTemporarySuccesPassword);
            return message.Text;
        }


        public string GetErrorTemporaryPasswordMessage()
        {
            var message = FindElement(messageTemporaryErrorPassword);
            return message.Text;
        }


    }
}
