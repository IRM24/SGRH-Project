using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SGRH.Web.Models.Data;
using SGRH.Web.Models.Entities;
using SGRH.Web.Models.ViewModels;
using SGRH.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGRHTestProject.Tests.UnitTests
{
    [TestFixture]
    public class SettlementServiceTests
    {
        private SgrhContext _context;
        private Mock<UserManager<User>> _userManagerMock;
        private SettlementService _settlementService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<SgrhContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new SgrhContext(options);
            _userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(),
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null);
            _settlementService = new SettlementService(_context, _userManagerMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task CreateSettlement_ValidModel_ShouldReturnSuccess()
        {
            // Arrange
            var user = new User { Id = "user7", Name = "Fabiana", LastName = "Arias", Dni = "202477" };
            var layoff = new Layoff
            {
                Id = 1,
                PersonalAction = new PersonalAction { User = user }
            };

            _context.Layoffs.Add(layoff);
            await _context.SaveChangesAsync();

            var model = new CreateSettlementViewModel
            {
                LayoffId = layoff.Id,
                AvgLast6MonthsSalary = 1200000,
                DailyAvgLast6Months = 42.850m,
                Bonus = 85000,
                UnenjoyedVacation = 5,
                UnenjoyedVacationAmount = 115000,
                Notice = 21,
                NoticeAmount = 160000,
                Severance = 3,
                SeveranceAmount = 175000,
                TotalSettlement = 2000000,
                currentUserId = "user5" // Simulando que es otro usuario
            };

            // Act
            var result = await _settlementService.CreateSettlement(model);

            // Assert
            Assert.IsTrue(result.success);
            Assert.IsNull(result.message);

            var settlement = await _context.Settlements.FirstOrDefaultAsync();
            Assert.IsNotNull(settlement);
            Assert.AreEqual(settlement.TotalSettlement, model.TotalSettlement);
        }

        [Test]
        public async Task CreateSettlement_SameUser_ShouldReturnError()
        {
            // Arrange
            var user = new User { Id = "user1", Name = "Vanessa", LastName = "Flores", Dni = "202411" };
            var layoff = new Layoff
            {
                Id = 1,
                PersonalAction = new PersonalAction { User = user }
            };

            _context.Layoffs.Add(layoff);
            await _context.SaveChangesAsync();

            var model = new CreateSettlementViewModel
            {
                LayoffId = layoff.Id,
                AvgLast6MonthsSalary = 1400000,
                DailyAvgLast6Months = 43.950m,
                Bonus = 130000,
                UnenjoyedVacation = 2,
                UnenjoyedVacationAmount = 65000,
                Notice = 15,
                NoticeAmount = 180000,
                Severance = 4,
                SeveranceAmount = 195000,
                TotalSettlement = 2700000,
                currentUserId = "user1" // Simulando que es otro usuario
            };

            // Act
            var result = await _settlementService.CreateSettlement(model);

            // Assert
            Assert.IsFalse(result.success);
        }

        [Test]
        public async Task CreateSettlement_NullModel_ShouldReturnError()
        {
            // Act
            var result = await _settlementService.CreateSettlement(null);

            // Assert
            Assert.IsFalse(result.success);
            Assert.AreEqual("Lo sentimos, no es posible procesar la liquidación.", result.message);
        }

        [Test]
        public async Task CreateSettlement_ExistingSettlement_ShouldReturnError()
        {
            // Arrange
            var user = new User { Id = "user3", Name = "Luis", LastName = "Campos", Dni = "202422" };
            var layoff = new Layoff
            {
                Id = 1,
                PersonalAction = new PersonalAction { User = user }
            };

            _context.Layoffs.Add(layoff);
            await _context.SaveChangesAsync();

            // Agregar una liquidación existente
            var existingSettlement = new Settlement
            {
                LayoffId = layoff.Id,
                TotalSettlement = 1500000
            };
            _context.Settlements.Add(existingSettlement);
            await _context.SaveChangesAsync();

            var model = new CreateSettlementViewModel
            {
                LayoffId = layoff.Id,
                AvgLast6MonthsSalary = 1800000,
                DailyAvgLast6Months = 55.000m,
                Bonus = 190000,
                UnenjoyedVacation = 8,
                UnenjoyedVacationAmount = 490000,
                Notice = 10,
                NoticeAmount = 100000,
                Severance = 5,
                SeveranceAmount = 200000,
                TotalSettlement = 2800000,
                currentUserId = "user9" // Simulando que es otro usuario
            };

            // Act
            var result = await _settlementService.CreateSettlement(model);

            // Assert
            Assert.IsFalse(result.success);
        }

        [Test]
        public async Task DeleteSettlement_ValidId_ShouldReturnSuccess()
        {
            // Arrange
            var layoff = new Layoff { Id = 1, HasProcessed = false };
            _context.Layoffs.Add(layoff);
            var settlement = new Settlement { Id = 1, LayoffId = 1 };
            _context.Settlements.Add(settlement);
            await _context.SaveChangesAsync();

            // Act
            var result = await _settlementService.DeleteSettlement(1);

            // Assert
            Assert.IsTrue(result.success);
            Assert.AreEqual(0, await _context.Settlements.CountAsync());
        }

        [Test]
        public async Task DeleteSettlement_InvalidId_ShouldReturnError()
        {
            // Act
            var result = await _settlementService.DeleteSettlement(99); // ID que no existe

            // Assert
            Assert.IsFalse(result.success);
            Assert.AreEqual("No se encontró la liquidación.", result.message);
        }

        [Test]
        public async Task GetSettlementById_InvalidId_ShouldReturnNull()
        {
            // Act
            var result = await _settlementService.GetSettlementById(100); // ID que no existe

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetSettlements_ShouldReturnEmptyList_WhenNoSettlementsExist()
        {
            // Act
            var settlements = await _settlementService.GetSettlements();

            // Assert
            Assert.IsNotNull(settlements);
            Assert.IsEmpty(settlements);
        }

        [Test]
        public async Task GetSettlementsCount_ShouldReturnCorrectCount()
        {
            // Arrange
            var layoff = new Layoff { Id = 1 };
            _context.Layoffs.Add(layoff);
            _context.Settlements.Add(new Settlement { LayoffId = 1, TotalSettlement = 2000000 });
            await _context.SaveChangesAsync();

            // Act
            var count = await _settlementService.GetSettlementsCount();

            // Assert
            Assert.AreEqual(1, count);
        }

        [Test]
        public async Task GetTotalSettlementAmount_ShouldReturnCorrectTotal()
        {
            // Arrange
            var layoff = new Layoff { Id = 1 };
            _context.Layoffs.Add(layoff);
            _context.Settlements.Add(new Settlement { LayoffId = 1, TotalSettlement = 2000000 });
            _context.Settlements.Add(new Settlement { LayoffId = 1, TotalSettlement = 3000000 });
            await _context.SaveChangesAsync();

            // Act
            var totalAmount = await _settlementService.GetTotalSettlementAmount();

            // Assert
            Assert.AreEqual(5000000, totalAmount);
        }

        [Test]
        public async Task GetTotalSettlementAmount_ShouldReturnIncorrectTotal_WhenSettlementsExist()
        {
            // Arrange
            var layoff1 = new Layoff { Id = 1 };
            var layoff2 = new Layoff { Id = 2 };

            _context.Layoffs.Add(layoff1);
            _context.Layoffs.Add(layoff2);
            // Agregar liquidaciones con un total esperado diferente
            _context.Settlements.Add(new Settlement { LayoffId = 1, TotalSettlement = 1000000 }); // Este es correcto
            _context.Settlements.Add(new Settlement { LayoffId = 2, TotalSettlement = 2000000 }); // Este es correcto
            await _context.SaveChangesAsync();

            // Act
            var totalAmount = await _settlementService.GetTotalSettlementAmount();

            // Assert
            Assert.AreNotEqual(5000000, totalAmount); // Prueba de que el total incorrecto no debe ser 5000000
            Assert.AreEqual(3000000, totalAmount); // Verifica que el total correcto sea 3000000
        }


        [Test]
        public async Task GetTotalSettlementAmount_ShouldReturnZero_WhenNoSettlementsExist()
        {
            // Act
            var totalAmount = await _settlementService.GetTotalSettlementAmount();

            // Assert
            Assert.AreEqual(0, totalAmount);
        }

    }
}
