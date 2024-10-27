using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SGRH.Web.Models.Data;
using SGRH.Web.Models.Entities;
using SGRH.Web.Services;
using System.Threading.Tasks;

namespace SGRHTestProject
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
            _context.Database.EnsureDeleted();  
            _context.Dispose();  
        }

        [Test]
        public async Task GetDepartmentCount_Should_Return_Correct_Count()
        {
            var departments = new[]
            {
                new Department { Department_Name = "HR" },
                new Department { Department_Name = "IT" }
            };

            _context.Departments.AddRange(departments);
            await _context.SaveChangesAsync();

            var result = await _departmentService.GetDepartmentCount();

            Assert.AreEqual(2, result);
        }

        [Test]
        public async Task CreateDepartment_Should_Return_Success_When_ValidDepartment()
        {
            var department = new Department
            {
                Department_Name = "Finanzas"
            };

            var (success, message) = await _departmentService.CreateDepartment(department);
            var createdDepartment = await _context.Departments.FirstOrDefaultAsync(d => d.Department_Name == "Finance");

            Assert.IsTrue(success);
            Assert.IsNotNull(createdDepartment);  
            Assert.AreEqual("Finanzas", createdDepartment.Department_Name);
        }

        [Test]
        public async Task CreateDepartment_Should_Return_Failure_When_ExceptionOccurs()
        {
            var invalidDepartment = new Department
            {
                Department_Name = null
            };

            var (success, message) = await _departmentService.CreateDepartment(invalidDepartment);

            Assert.IsFalse(success);
        }

        [Test]
        public async Task UpdateDepartment_Should_Return_Success_When_Department_Exists()
        {
            var department = new Department
            {
                Id_Department = 1,
                Department_Name = "RH"
            };

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            var updatedDepartment = new Department
            {
                Id_Department = 1,
                Department_Name = "Recursos Humanos"
            };

            var (success, message) = await _departmentService.UpdateDepartment(updatedDepartment);

            Assert.IsTrue(success);

            var departmentFromDb = await _context.Departments.FirstOrDefaultAsync(d => d.Id_Department == updatedDepartment.Id_Department);
            Assert.IsNotNull(departmentFromDb);
            Assert.AreEqual("Recursos Humanos", departmentFromDb.Department_Name);
        }

        [Test]
        public async Task UpdateDepartment_Should_Return_Failure_When_Department_Does_Not_Exist()
        {
            // Arrange
            var nonExistentDepartment = new Department
            {
                Id_Department = 999,  // Este ID no existe en la base de datos
                Department_Name = "Recursos Humanos"
            };

            // Act
            var (success, message) = await _departmentService.UpdateDepartment(nonExistentDepartment);

            // Assert
            Assert.IsFalse(success);
        }

        [Test]
        public async Task DeleteDepartment_Should_Return_Success_When_Department_Exists()
        {
            var department = new Department
            {
                Id_Department = 1,
                Department_Name = "Administracion"
            };

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            var (success, message) = await _departmentService.DeleteDepartment(department.Id_Department);

            Assert.IsTrue(success);

            var departmentFromDb = await _context.Departments.FirstOrDefaultAsync(d => d.Id_Department == department.Id_Department);
            Assert.IsNull(departmentFromDb);  
        }

        [Test]
        public async Task DeleteDepartment_Should_Return_Failure_When_Department_Does_Not_Exist()
        {
            var nonExistentDepartmentId = 999; 

            var (success, message) = await _departmentService.DeleteDepartment(nonExistentDepartmentId);

            Assert.IsFalse(success);
        }

        [Test]
        public async Task GetDepartmentById_Should_Return_Department_When_Department_Exists()
        {
            var department = new Department
            {
                Id_Department = 1,
                Department_Name = "TI"
            };

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            var result = await _departmentService.GetDepartmentById(department.Id_Department);

            Assert.IsNotNull(result);  
            Assert.AreEqual(department.Id_Department, result.Id_Department); 
            Assert.AreEqual(department.Department_Name, result.Department_Name);  
        }

        [Test]
        public async Task GetDepartmentById_Should_Return_Null_When_Department_Does_Not_Exist()
        {
            var nonExistentDepartmentId = 999;  

            var result = await _departmentService.GetDepartmentById(nonExistentDepartmentId);

            Assert.IsNull(result);  
        }

        [Test]
        public async Task GetDepartments_Should_Return_All_Departments()
        {
            var departments = new[]
            {
                new Department { Department_Name = "Recursos Humanos" },
                new Department { Department_Name = "TI" },
                new Department { Department_Name = "Gerencia" }
            };

            _context.Departments.AddRange(departments);
            await _context.SaveChangesAsync();

            var result = await _departmentService.GetDepartments();

            Assert.AreEqual(3, result.Count); 
            Assert.IsTrue(result.Any(d => d.Department_Name == "Recursos Humanos"));
            Assert.IsTrue(result.Any(d => d.Department_Name == "TI"));
            Assert.IsTrue(result.Any(d => d.Department_Name == "Gerencia"));
        }


    }
}
