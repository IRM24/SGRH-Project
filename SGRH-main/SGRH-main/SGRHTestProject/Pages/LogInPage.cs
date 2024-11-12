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

        private By lockoutMessageLocator = By.XPath("//li[contains(text(), 'La cuenta está bloqueada debido a demasiados intentos fallidos de inicio de sesión')]");

        private By logoutBtnLocator = By.XPath("//a[@href='javascript:void(0);' and @class='nav-link']");

        private By loginPageTitleLocator = By.ClassName("login-box-msg");

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

        public void ClickLogout()
        {
            Click(logoutBtnLocator);
        }


        public void LogIn(string username, string password)
        {
            EnterEmail(username);
            EnterPassword(password);
            ClickLogin();
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


        public string GetAccountLockoutMessage()
        {
            return FindElement(lockoutMessageLocator).Text;
        }


        


        public string GetLoginPageTitle()
        {
            return FindElement(loginPageTitleLocator).Text;
        }

    }
}
