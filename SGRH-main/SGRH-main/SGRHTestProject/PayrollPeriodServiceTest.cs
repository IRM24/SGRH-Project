using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SGRH.Web.Models.Data;
using SGRH.Web.Models.Entities;
using SGRH.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGRHTestProject
{
    [TestFixture]
    public class PayrollPeriodServiceTests
    {
        private PayrollPeriodService _payrollPeriodService;
        private SgrhContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<SgrhContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new SgrhContext(options);
            _payrollPeriodService = new PayrollPeriodService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
            var options = new DbContextOptionsBuilder<SgrhContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            using (var tempContext = new SgrhContext(options))
            {
                tempContext.Database.EnsureDeleted();
            }
        }

        [Test]
        public async Task GetAllPayrollPeriodsAsync_WithCurrentYearPeriods_ReturnsCorrectPeriods()
        {
            var payrollPeriods = new List<PayrollPeriod>
            {
                new PayrollPeriod { PeriodName = "1 AL 15 ENERO 2024", StartDate = new DateTime(2024, 1, 1), EndDate = new DateTime(2024, 1, 15) },
                new PayrollPeriod { PeriodName = "16 AL 31 ENERO 2024", StartDate = new DateTime(2024, 1, 16), EndDate = new DateTime(2024, 1, 31) }
            };

            _context.PayrollPeriod.AddRange(payrollPeriods);
            await _context.SaveChangesAsync();

            var result = await _payrollPeriodService.GetAllPayrollPeriodsAsync();

            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task GeneratePayrollPeriodsAsync_GeneratesCorrectNumberOfPeriods()
        {
            await _payrollPeriodService.GeneratePayrollPeriodsAsync(2024);

            var periods = await _context.PayrollPeriod.ToListAsync();
            Assert.AreEqual(24, periods.Count); 
        }

        [Test]
        public async Task GetCurrentPayrollPeriodAsync_ReturnsCorrectPeriod()
        {
            var payrollPeriod = new PayrollPeriod
            {
                PeriodName = "1 AL 15 OCTUBRE 2024",
                StartDate = DateTime.UtcNow.AddDays(-7),  
                EndDate = DateTime.UtcNow.AddDays(7)      
            };

            _context.PayrollPeriod.Add(payrollPeriod);
            await _context.SaveChangesAsync();

            var result = await _payrollPeriodService.GetCurrentPayrollPeriodAsync();

            Assert.AreEqual(payrollPeriod.PeriodName, result.PeriodName);
        }

        [Test]
        public async Task GetAllPayrollPeriodsAsync_EmptyDatabase_ReturnsEmptyList()
        {
            var result = await _payrollPeriodService.GetAllPayrollPeriodsAsync();

            Assert.IsEmpty(result);
        }

        [Test]
        public async Task AddPayrollPeriodAsync_AddsNewPayrollPeriod()
        {
            var payrollPeriod = new PayrollPeriod
            {
                PeriodName = "1 AL 15 MAYO 2024",
                StartDate = new DateTime(2024, 5, 1),
                EndDate = new DateTime(2024, 5, 15)
            };

            await _payrollPeriodService.AddPayrollPeriodAsync(payrollPeriod);

            var periods = await _context.PayrollPeriod.ToListAsync();
            Assert.AreEqual("1 AL 15 MAYO 2024", periods.First().PeriodName);
        }

    }
}
