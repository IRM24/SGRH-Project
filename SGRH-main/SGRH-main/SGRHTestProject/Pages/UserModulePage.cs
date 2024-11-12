using EllipticCurve;
using Microsoft.CodeAnalysis.Options;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGRHTestProject.Pages
{
    public class UserModulePage : Base
    {

        private By userModuleLocator = By.CssSelector("i.nav-icon.fas.fa-user");

        private By userModuleHeaderMessageLocator = By.CssSelector("h1.m-0");

        private By registerUserButton = By.CssSelector("a.btn.btn-success");

        //Localizadores para registrar usuario
        private By nameField = By.Id("Name");
        private By lastNameField = By.Id("LastName");
        private By idNumberField = By.Id("Dni");
        private By birthDateField = By.Id("BirthDate");
        private By phoneNumberField = By.Id("PhoneNumber");
        private By emailField = By.Id("Email");
        private By departmentDropdown = By.Id("DepartmentId");
        private By positionDropdown = By.Id("PositionId");
        private By roleDropdown = By.Id("UserType");
        private By hiringDateField = By.Id("HiredDate");
        private By baseSalaryField = By.Id("baseSalaryInput");
        private By workPeriodDropdown = By.Id("WorkPeriodId");

        private By createUserBtnLocator = By.CssSelector("input[type='submit'][value='Crear']");


        private By birthDateErrorMessageLocator = By.XPath("//div[@class='swal2-html-container' and text()='La fecha de nacimiento debe estar entre 01/01/1900 y hace 18 años.']");
        private By deparmentFieldRequiredErrorMessage = By.CssSelector("span[data-valmsg-for='DepartmentId'][class='text-danger field-validation-error']");
        private By positionFieldRequiredErrorMessage = By.CssSelector("span[data-valmsg-for='PositionId'][class='text-danger field-validation-error']");
        private By salaryErrorMessageLocator = By.CssSelector("div.swal2-html-container#swal2-html-container");


        private By confirmUserCreationBtn = By.CssSelector("button.swal2-confirm.swal2-styled");

        //Localizadores para busqueda
        private By searchBarLocator = By.CssSelector("input[type='search'].form-control.form-control-sm");
        private By searchResultsLocator = By.CssSelector("table tbody tr");
        private By eachSearchResultLocator = By.CssSelector("td");



        //Localizadores para editar 
        private By showInformationOfUser = By.XPath("//td[@class='dtr-control sorting_1']");
        private By editDataUserBtnLocator = By.XPath("//span[@class='dtr-data']//a[@class='btn btn-info' and @href='/Users/Edit/9a058d20-0edb-406c-ba64-b5ecbce4dc50']\r\n");
        private By editSaveUserBtnLocator = By.CssSelector("input[type='submit'][value='Editar']");
        private By confirmEditButtonLocator = By.CssSelector("button.swal2-confirm.swal2-styled");
        private By userEditedSuccessMessageLocator = By.CssSelector("div.swal2-html-container#swal2-html-container");


        //Localizadores para eliminar
        private By deleteUserBtnLocator = By.XPath("//span[@class='dtr-data']//button[@type='button' and contains(@class, 'btn-danger') and @data-user-name='Carlos Perez']");
        private By confirmDeleteButtonLocator = By.XPath("//button[@type='button' and contains(@class, 'swal2-confirm') and contains(@class, 'swal2-styled') and text()='OK']");
        private By userDeletedSuccessMessageLocator = By.XPath("//div[@class='swal2-html-container' and text()='Usuario eliminado de manera exitosa.']");


        public UserModulePage(IWebDriver driver) : base(driver)
        {
        }


        public void GoToUserModule()
        {
            Click(userModuleLocator);
        }

        public string GetUserModuleHeaderMessage()
        {
            return FindElement(userModuleHeaderMessageLocator).Text;
        }

        public bool IsUserModuleVisible()
        {
            try
            {
                var userModuleIcon = FindElement(userModuleLocator);
                return userModuleIcon.Displayed; // devuelve true
            }
            catch (NoSuchElementException)
            {
                // Si no se encuentra el elemento, significa que no está visible
                return false;
            }
        }

        public void ClickRegisterUserButon()
        {
            Click(registerUserButton);
        }


        public void ClickCreateUserButton()
        {
            Click(createUserBtnLocator);
        }

        public void ClickConfirmUserCreation()
        {
            Click(confirmUserCreationBtn);
        }

        public void EnterName(string name)
        {
            ClearField(nameField);
            Type(name, nameField);
        }

        public void EnterLastName(string lastName)
        {
            ClearField(lastNameField);
            Type(lastName, lastNameField);
        }

        public void EnterIdNumber(string idNumber)
        {
            Type(idNumber, idNumberField);
        }

        public void EnterPhoneNumber(string phoneNumber)
        {
            Type(phoneNumber, phoneNumberField);
        }


        public void EnterEmail(string email)
        {
            ClearField(emailField);
            Type(email, emailField);
        }

        public void SelectDepartment(string department)
        {
            SelectDropdownOption(departmentDropdown, department);
        }

        public void SelectPosition(string position)
        {
            SelectDropdownOption(positionDropdown, position);
        }

        public void SelectRole(string role)
        {
            SelectDropdownOption(roleDropdown, role);
        }

        public void SelectWorkPeriod(string workPeriod)
        {
            SelectDropdownOption(workPeriodDropdown, workPeriod);
        }


        public void EnterBirthDate(string date)
        {
            Type(date, birthDateField);
        }

        public void EnterHiringDate(string date)
        {
            Type(date, hiringDateField);
        }

        public void EnterBaseSalary(string salary)
        {
            Type(salary, baseSalaryField);
        }

        public string GetbirthDateErrorMessage()
        {
            return FindElement(birthDateErrorMessageLocator).Text;
        }

        public string GetSalaryErrorMessage()
        {
            return FindElement(salaryErrorMessageLocator).Text;
        }

        public string GetDepartmentFieldRequiredMessage()
        {
            return FindElement(deparmentFieldRequiredErrorMessage).Text;
        }

        public string GetPositionFieldRequiredMessage()
        {
            return FindElement(positionFieldRequiredErrorMessage).Text;
        }


        public void RegisterUser(string name, string lastName, string idNumber, string birthDate, string phoneNumber, string email,
            string department, string position, string role, string hiringDate, string salary, string workPeriod)
        {
            GoToUserModule();
            ClickRegisterUserButon();

            EnterName(name);
            EnterLastName(lastName);
            EnterIdNumber(idNumber);
            EnterBirthDate(birthDate);
            EnterPhoneNumber(phoneNumber);
            EnterEmail(email);
            SelectDepartment(department);
            SelectPosition(position);
            SelectRole(role);
            EnterHiringDate(hiringDate);
            EnterBaseSalary(salary);
            SelectWorkPeriod(workPeriod);
            
            ClickCreateUserButton();
            ClickConfirmUserCreation(); 
        }


        public void Search(string searchInput)
        {
            Type(searchInput, searchBarLocator);
        }

        public List<List<string>> GetSearchResults()
        {
            var rows = FindElements(searchResultsLocator);

            var results = new List<List<string>>();

            foreach (var row in rows)
            {
                // Obtener todas las celdas de la fila (td)
                var cells = row.FindElements(eachSearchResultLocator);
                var cellTexts = cells.Select(cell => cell.Text).ToList();
                results.Add(cellTexts);
            }

            return results;
        }


        public void ShowInformationOfUser()
        {
            Click(showInformationOfUser);
        }


        public void ClickEdiDataUserButton()
        {
            Click(editDataUserBtnLocator);
        }


        public void ClickEditSaveUserButton()
        {
            Click(editSaveUserBtnLocator);
        }

        public void ClickConfirmEditButton()
        {
            Click(confirmEditButtonLocator);
        }


        public string GetUserEditedSucessMessage()
        {
            return FindElement(userEditedSuccessMessageLocator).Text;
        }


        public void ClickDeleteUserButton()
        {
            Click(deleteUserBtnLocator);
        }

        public void ClickConfirmDeleteButton()
        {
            Click(confirmDeleteButtonLocator);
        }

        public string GetUserDeletedSucessMessage()
        {
            return FindElement(userDeletedSuccessMessageLocator).Text;
        }


    }
}
