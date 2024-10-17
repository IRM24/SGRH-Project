using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SGRH.Web.Models.Data;
using SGRH.Web.Models.Entities;
using SGRH.Web.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGRHTestProject
{
    [TestFixture]
    public class AbsenceServiceTests
    {
        private AbsenceService _absenceService;
        private SgrhContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<SgrhContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new SgrhContext(options);
            _absenceService = new AbsenceService(_context, null);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
            var databaseName = "TestDatabase";
            var options = new DbContextOptionsBuilder<SgrhContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;
            using (var tempContext = new SgrhContext(options))
            {
                tempContext.Database.EnsureDeleted();
            }
        }

        [Test]
        public async Task GetAbsenceCategories_ReturnsCategories()
        {
            // Arrange
            var categories = new List<AbsenceCategory>
            {
                new AbsenceCategory { Id_Absence_Category = 1, Category_Name = "Sick Leave" },
                new AbsenceCategory { Id_Absence_Category = 2, Category_Name = "Vacation" }
            };

            _context.AbsenceCategories.AddRange(categories);
            await _context.SaveChangesAsync();

            // Act
            var result = await _absenceService.GetAbsenceCategories();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Sick Leave", result[0].Category_Name);
        }

        [Test]
        public async Task GetAbsenceCategories_EmptyDatabase_ReturnsEmptyList()
        {
            // Act
            var result = await _absenceService.GetAbsenceCategories();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task GetAbsenceCategories_AddOneCategory_ReturnsSingleCategory()
        {
            // Arrange
            var category = new AbsenceCategory { Id_Absence_Category = 1, Category_Name = "Sick Leave" };
            _context.AbsenceCategories.Add(category);
            await _context.SaveChangesAsync();

            // Act
            var result = await _absenceService.GetAbsenceCategories();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Sick Leave", result[0].Category_Name);
        }

        [Test]
        public async Task GetAbsenceCategories_AddMultipleCategories_ReturnsAllCategories()
        {
            // Arrange
            var categories = new List<AbsenceCategory>
            {
                new AbsenceCategory { Id_Absence_Category = 1, Category_Name = "Sick Leave" },
                new AbsenceCategory { Id_Absence_Category = 2, Category_Name = "Vacation" },
                new AbsenceCategory { Id_Absence_Category = 3, Category_Name = "Personal Leave" }
            };

            _context.AbsenceCategories.AddRange(categories);
            await _context.SaveChangesAsync();

            // Act
            var result = await _absenceService.GetAbsenceCategories();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("Sick Leave", result[0].Category_Name);
            Assert.AreEqual("Vacation", result[1].Category_Name);
            Assert.AreEqual("Personal Leave", result[2].Category_Name);
        }
    }
}
