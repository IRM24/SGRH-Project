using OpenQA.Selenium;

namespace SGRHTestProject.Pages
{
    public class MaintenanceModulePage : Base
    {
        //Localizadores principales
        private By maintenanceModuleLocator => By.LinkText("Mantenimiento");
        private By departmentsSectionLocator => By.LinkText("Departamentos");
        private By positionsSectionLocator => By.LinkText("Puestos");

        // Localizadores para crear departamento
        private By createDepartmentButtonLocator => By.XPath("/html/body/div[1]/div[2]/section/div/div/div[2]/p/a");
        private By departmentNameInputLocator => By.Id("Department_Name");
        private By createButtonLocator => By.XPath("/html/body/div[1]/div[2]/section/div/div/form/div/div[3]/div/input");
        private By successMessageLocator => By.CssSelector(".swal2-title");

        // Localizadores para editar departamento
        private By editDepartmentButtonLocator => By.XPath("//*[@id=\"example1\"]/tbody/tr/td[2]/a");
        private By departmentNameEditInputLocator => By.Id("Department_Name");
        private By updateButtonLocator => By.XPath("/html/body/div[1]/div[2]/section/div/div/form/div/div[3]/div/input");
        private By searchBoxLocator => By.XPath("//*[@id=\"example1_filter\"]/label/input");

        private By departmentSearchResultLocator => By.XPath("//td[contains(text(), 'Administrativos')]");

        // Localizadores para eliminar departamento
        private By deleteButtonLocator => By.XPath("//*[@id=\"example1\"]/tbody/tr[1]/td[3]/button");
        private By confirmDeleteButtonLocator => By.XPath("//button[contains(text(), 'OK')]");

        
        private By searchBoxPositionsLocator => By.XPath("//*[@id=\"example1_filter\"]/label/input");
        private By positionSearchResultLocator => By.XPath("//td[contains(text(), 'Auditor')]");

        // Localizadores para ekiminar puesto
        private By deletePositionButtonLocator => By.XPath("//*[@id=\"example1\"]/tbody/tr[1]/td[4]/button");
        private By confirmPositionDeleteButtonLocator => By.XPath("//button[contains(text(), 'OK')]");

        public MaintenanceModulePage(IWebDriver driver) : base(driver) { }


        // --------------------- MÉTODOS PARA MODULO ---------------------
        public void GoToMaintenanceModule()
        {
            Click(maintenanceModuleLocator);
        }

        public void GoToDepartmentsSection()
        {
            Click(departmentsSectionLocator);
        }

        public void GoToPositionsSection()
        {
            Click(positionsSectionLocator);
        }

        // --------------------- MÉTODOS PARA CREAR DEPARTAMENTO ---------------------
        public void ClickCreateDepartment()
        {
            Click(createDepartmentButtonLocator);
        }

        public void EnterDepartmentName(string departmentName)
        {
            Type(departmentName, departmentNameInputLocator);
        }

        public void ClickCreate()
        {
            Click(createButtonLocator);
        }

        // --------------------- MÉTODOS PARA EDITAR DEPARTAMENTO ---------------------
        public void ClickEditDepartment()
        {
            Click(editDepartmentButtonLocator);
        }

        public void EnterNewDepartmentName(string newDepartmentName)
        {
            ClearField(departmentNameEditInputLocator);
            Type(newDepartmentName, departmentNameEditInputLocator);
        }

        public void ClickUpdate()
        {
            Click(updateButtonLocator);
        }

        // --------------------- MÉTODOS PARA BUSCAR DEPARTAMENTO ---------------------
        public void SearchDepartment(string departmentName)
        {
            Type(departmentName, searchBoxLocator);
        }

        public bool IsDepartmentDisplayedInResults()
        {
            return IsDisplayed(departmentSearchResultLocator);
        }


        // --------------------- MÉTODOS PARA ELIMINAR DEPARTAMENTO ---------------------
        public void ClickDeleteDepartment()
        {
            Click(deleteButtonLocator);
        }

        public void ConfirmDelete()
        {
            Click(confirmDeleteButtonLocator);
        }


        public string GetSuccessMessage()
        {
            try
            {
                return FindElement(successMessageLocator).Text;
            }
            catch (NoSuchElementException)
            {
                throw new Exception("El mensaje de éxito no fue encontrado.");
            }
        }


        // --------------------- MÉTODOS PARA BUSCAR PUESTO ---------------------
        public void SearchPosition(string positionName)
        {
            Type(positionName, searchBoxPositionsLocator);
        }

        public bool IsPositionDisplayedInResults()
        {
            return IsDisplayed(positionSearchResultLocator);
        }

        // --------------------- MÉTODOS PARA ELIMINAR PUESTO ---------------------
        public void DeletePosition()
        {
            Click(deletePositionButtonLocator);
        }

        public void ConfirmDeletePosition()
        {
            Click(confirmPositionDeleteButtonLocator);
        }
    }
}
