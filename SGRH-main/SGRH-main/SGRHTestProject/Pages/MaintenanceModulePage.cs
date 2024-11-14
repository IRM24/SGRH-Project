using OpenQA.Selenium;

namespace SGRHTestProject.Pages
{
    public class MaintenanceModulePage : Base
    {
        private By maintenanceModuleLocator => By.LinkText("Mantenimiento");
        private By departmentsSectionLocator => By.LinkText("Departamentos");
        private By createDepartmentButtonLocator => By.XPath("/html/body/div[1]/div[2]/section/div/div/div[2]/p/a");
        private By departmentNameInputLocator => By.Id("Department_Name"); // Reemplaza "DepartmentName" con el ID real del campo de entrada
        private By createButtonLocator => By.XPath("/html/body/div[1]/div[2]/section/div/div/form/div/div[3]/div/input"); // Reemplaza el texto si es diferente
        private By successMessageLocator => By.CssSelector(".swal2-title"); // Selector del título de SweetAlert

        private By editDepartmentButtonLocator => By.XPath("//*[@id=\"example1\"]/tbody/tr/td[2]/a"); // Actualiza si es necesario
        private By departmentNameEditInputLocator => By.Id("Department_Name"); // Reemplaza con el ID real del campo de entrada
        private By updateButtonLocator => By.XPath("/html/body/div[1]/div[2]/section/div/div/form/div/div[3]/div/input"); // Reemplaza si el texto es diferente
        private By searchBoxLocator => By.XPath("//*[@id=\"example1_filter\"]/label/input"); // Actualiza si el selector es diferente

        private By departmentSearchResultLocator => By.XPath("//td[contains(text(), 'Administrativos')]"); // Localiza el departamento buscado en los resultados

        private By deleteButtonLocator => By.XPath("//*[@id=\"example1\"]/tbody/tr[1]/td[3]/button"); // Localiza el botón de eliminar en la fila del departamento
        private By confirmDeleteButtonLocator => By.XPath("//button[contains(text(), 'OK')]"); // Botón en el modal de confirmación de eliminación

        private By positionsSectionLocator => By.LinkText("Puestos"); // Enlace para la sección de "Puestos"
        private By searchBoxPositionsLocator => By.XPath("//*[@id=\"example1_filter\"]/label/input");
        private By positionSearchResultLocator => By.XPath("//td[contains(text(), 'Auditor')]"); // Verificación del resultado en tiempo real para el puesto buscado

        private By deletePositionButtonLocator => By.XPath("//*[@id=\"example1\"]/tbody/tr[1]/td[4]/button"); // Botón de eliminar para el puesto "Auditor"
        private By confirmPositionDeleteButtonLocator => By.XPath("//button[contains(text(), 'OK')]"); // Confirmación del swal

        public MaintenanceModulePage(IWebDriver driver) : base(driver) { }

        public void GoToMaintenanceModule()
        {
            Click(maintenanceModuleLocator);
        }

        public void GoToDepartmentsSection()
        {
            Click(departmentsSectionLocator);
        }

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

        public void SearchDepartment(string departmentName)
        {
            Type(departmentName, searchBoxLocator);
        }

        public bool IsDepartmentDisplayedInResults()
        {
            return IsDisplayed(departmentSearchResultLocator);
        }

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

        public void GoToPositionsSection()
        {
            Click(positionsSectionLocator);
        }

        public void SearchPosition(string positionName)
        {
            Type(positionName, searchBoxPositionsLocator);
        }

        public bool IsPositionDisplayedInResults()
        {
            return IsDisplayed(positionSearchResultLocator);
        }

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
