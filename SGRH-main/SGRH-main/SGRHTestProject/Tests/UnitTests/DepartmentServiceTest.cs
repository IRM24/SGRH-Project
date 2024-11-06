using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SGRH.Web.Models.Data;
using SGRH.Web.Models.Entities;
using SGRH.Web.Services;
using System.Threading.Tasks;

namespace SGRHTestProject.Tests.UnitTests
{
    [TestFixture]
    public class DepartmentServiceTests
    {
        private DepartmentService _departmentService;
        private SgrhContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<SgrhContext>()
                .UseInMemoryDatabase(databaseName: "DepartmentTestDatabase")
                .Options;

            _context = new SgrhContext(options);

            _departmentService = new DepartmentService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();  // Eliminar la base de datos para cada prueba
            _context.Dispose();  // Disponer el contexto
        }

        [Test]
        public async Task GetDepartmentCount_Should_Return_Correct_Count()
        {
            // Arrange
            var departments = new[]
            {
                new Department { Department_Name = "HR" },
                new Department { Department_Name = "IT" }
            };

            _context.Departments.AddRange(departments);
            await _context.SaveChangesAsync();

            // Act
            var result = await _departmentService.GetDepartmentCount();

            // Assert
            Assert.AreEqual(2, result);
        }

        [Test]
        public async Task CreateDepartment_Should_Return_Success_When_ValidDepartment()
        {
            // Arrange
            var department = new Department
            {
                Department_Name = "Finance"
            };

            // Act
            var (success, message) = await _departmentService.CreateDepartment(department);
            var createdDepartment = await _context.Departments.FirstOrDefaultAsync(d => d.Department_Name == "Finance");

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual("Departamento creado exitosamente.", message);
            Assert.IsNotNull(createdDepartment);  // Verificar que el departamento se haya creado
            Assert.AreEqual("Finance", createdDepartment.Department_Name);
        }

        [Test]
        public async Task CreateDepartment_Should_Return_Failure_When_ExceptionOccurs()
        {
            // Arrange
            var invalidDepartment = new Department
            {
                // No establecemos el nombre del departamento para forzar un fallo
                Department_Name = null
            };

            // Act
            var (success, message) = await _departmentService.CreateDepartment(invalidDepartment);

            // Assert
            Assert.IsFalse(success);
            Assert.AreEqual("Ocurrió un error al intentar crear el departamento.", message);
        }

        [Test]
        public async Task UpdateDepartment_Should_Return_Success_When_Department_Exists()
        {
            // Arrange
            var department = new Department
            {
                Id_Department = 1,
                Department_Name = "OldName"
            };

            // Agregar el departamento a la base de datos en memoria
            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            // Preparar el modelo actualizado
            var updatedDepartment = new Department
            {
                Id_Department = 1,
                Department_Name = "NewName"
            };

            // Act
            var (success, message) = await _departmentService.UpdateDepartment(updatedDepartment);

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual("Departamento actualizado exitosamente.", message);

            // Verificar que el nombre del departamento se haya actualizado
            var departmentFromDb = await _context.Departments.FirstOrDefaultAsync(d => d.Id_Department == updatedDepartment.Id_Department);
            Assert.IsNotNull(departmentFromDb);
            Assert.AreEqual("NewName", departmentFromDb.Department_Name);
        }

        [Test]
        public async Task UpdateDepartment_Should_Return_Failure_When_Department_Does_Not_Exist()
        {
            // Arrange
            var nonExistentDepartment = new Department
            {
                Id_Department = 999,  // Este ID no existe en la base de datos
                Department_Name = "NonExistent"
            };

            // Act
            var (success, message) = await _departmentService.UpdateDepartment(nonExistentDepartment);

            // Assert
            Assert.IsFalse(success);
            Assert.AreEqual("El departamento que intentas actualizar no existe.", message);
        }

        [Test]
        public async Task DeleteDepartment_Should_Return_Success_When_Department_Exists()
        {
            // Arrange
            var department = new Department
            {
                Id_Department = 1,
                Department_Name = "HR"
            };

            // Agregar el departamento a la base de datos en memoria
            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            // Act
            var (success, message) = await _departmentService.DeleteDepartment(department.Id_Department);

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual("Departamento eliminado exitosamente.", message);

            // Verificar que el departamento se haya eliminado
            var departmentFromDb = await _context.Departments.FirstOrDefaultAsync(d => d.Id_Department == department.Id_Department);
            Assert.IsNull(departmentFromDb);  // El departamento ya no debe existir en la base de datos
        }

        [Test]
        public async Task DeleteDepartment_Should_Return_Failure_When_Department_Does_Not_Exist()
        {
            // Arrange
            var nonExistentDepartmentId = 999;  // Este ID no existe en la base de datos

            // Act
            var (success, message) = await _departmentService.DeleteDepartment(nonExistentDepartmentId);

            // Assert
            Assert.IsFalse(success);
            Assert.AreEqual("El departamento que intentas eliminar no existe.", message);
        }

        [Test]
        public async Task GetDepartmentById_Should_Return_Department_When_Department_Exists()
        {
            // Arrange
            var department = new Department
            {
                Id_Department = 1,
                Department_Name = "Finance"
            };

            // Agregar el departamento a la base de datos en memoria
            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            // Act
            var result = await _departmentService.GetDepartmentById(department.Id_Department);

            // Assert
            Assert.IsNotNull(result);  // Verificar que el departamento no sea nulo
            Assert.AreEqual(department.Id_Department, result.Id_Department);  // Verificar que el ID coincida
            Assert.AreEqual(department.Department_Name, result.Department_Name);  // Verificar que el nombre del departamento sea correcto
        }

        [Test]
        public async Task GetDepartmentById_Should_Return_Null_When_Department_Does_Not_Exist()
        {
            // Arrange
            var nonExistentDepartmentId = 999;  // Este ID no existe en la base de datos

            // Act
            var result = await _departmentService.GetDepartmentById(nonExistentDepartmentId);

            // Assert
            Assert.IsNull(result);  // Verificar que el resultado sea nulo cuando el departamento no existe
        }

        [Test]
        public async Task GetDepartments_Should_Return_All_Departments()
        {
            // Arrange
            var departments = new[]
            {
        new Department { Department_Name = "HR" },
        new Department { Department_Name = "IT" },
        new Department { Department_Name = "Finance" }
    };

            // Agregar los departamentos a la base de datos en memoria
            _context.Departments.AddRange(departments);
            await _context.SaveChangesAsync();

            // Act
            var result = await _departmentService.GetDepartments();

            // Assert
            Assert.IsNotNull(result);  // Verificar que el resultado no sea nulo
            Assert.AreEqual(3, result.Count);  // Verificar que se devuelvan los 3 departamentos
            Assert.IsTrue(result.Any(d => d.Department_Name == "HR"));
            Assert.IsTrue(result.Any(d => d.Department_Name == "IT"));
            Assert.IsTrue(result.Any(d => d.Department_Name == "Finance"));
        }


    }
}
