using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SGRH.Web.Enums;
using SGRH.Web.Models;
using SGRH.Web.Models.Data;
using SGRH.Web.Models.Entities;
using SGRH.Web.Models.ViewModels;
using SGRH.Web.Services;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using SGRH.Web.Enums;

namespace SGRHTestProject.Tests.UnitTests
{
    [TestFixture]
    public class OvertimeServiceTests
    {
        private OverTimeService _overtimeService;
        private SgrhContext _context;
        private Mock<UserManager<User>> _mockUserManager;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<SgrhContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new SgrhContext(options);

            var userStoreMock = new Mock<IUserStore<User>>();
            _mockUserManager = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

            _overtimeService = new OverTimeService(_context, _mockUserManager.Object);
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
        public async Task CreateOvertime_ValidData_ReturnsTrue()
        {
            var user = new User { Id = "usuario1", Dni = "111111111", Name = "Camila", LastName = "Ulate" };
            var workPeriod = new WorkPeriod { PeriodName = "Diurno", PeriodDescription = "Descripción del periodo diurno" };
            user.workPeriod = workPeriod;

            var personalAction = new PersonalAction { User = user };
            var otDate = DateTime.Now;
            var hoursWorked = 8;
            var typeOT = TypeOT.Sencillas;
            var salaryPerHour = 10m;

            var (success, message) = await _overtimeService.CreateOvertime(personalAction, otDate, hoursWorked, typeOT, salaryPerHour);

            Assert.IsTrue(success);
            Assert.IsNull(message);
            Assert.AreEqual(1, _context.Overtimes.Count());
        }


        [Test]
        public async Task CreateOvertime_ExistingOvertime_ReturnsFalse()
        {
            var user = new User { Id = "usuario1", Dni = "111111111", Name = "Camila", LastName = "Ulate" };
            var workPeriod = new WorkPeriod { PeriodName = "Diurno", PeriodDescription = "Descripción del periodo diurno" };
            user.workPeriod = workPeriod;

            var personalAction = new PersonalAction { User = user };
            var otDate = DateTime.Now;
            var hoursWorked = 8;
            var typeOT = TypeOT.Sencillas;
            var salaryPerHour = 10m;

            var existingOvertime = new Overtime
            {
                PersonalAction = personalAction,
                OT_Date = otDate,
                Hours_Worked = hoursWorked,
                TypeOT = typeOT,
                AmountOT = 80
            };
            _context.Overtimes.Add(existingOvertime);
            await _context.SaveChangesAsync();

            var (success, message) = await _overtimeService.CreateOvertime(personalAction, otDate, hoursWorked, typeOT, salaryPerHour);

            Assert.IsFalse(success);
            Assert.AreEqual(1, _context.Overtimes.Count());
        }



        [Test]
        public async Task GetOvertimeById_ExistingId_ReturnsOvertime()
        {
            var overtime = new Overtime
            {
                PersonalAction = new PersonalAction { User = new User { Id = "usuario1", Dni = "111111111", Name = "Ian", LastName = "Calvo" } },
                OT_Date = DateTime.Now,
                Hours_Worked = 8,
                TypeOT = TypeOT.Sencillas,
                AmountOT = 10
            };

            _context.Overtimes.Add(overtime);
            await _context.SaveChangesAsync();

            var result = await _overtimeService.GetOvertimeById(overtime.Id_OT);

            Assert.IsNotNull(result);
            Assert.AreEqual(overtime.Id_OT, result.Id_OT);
            Assert.AreEqual(overtime.Hours_Worked, result.Hours_Worked);
        }

        [Test]
        public async Task GetOvertimeById_NonExistingId_ReturnsNull()
        {
            var nonExistingId = 999; 

            var result = await _overtimeService.GetOvertimeById(nonExistingId);

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetOvertimes_EmpleadoRole_ReturnsUserOvertimes()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "usuario1"),
                new Claim(ClaimTypes.Role, "Empleado")
            }));

            var currentUser = new User { Id = "usuario1", Dni = "111111111", Name = "Ian", LastName = "Calvo", DepartmentId = 1 };
            _mockUserManager.Setup(um => um.GetUserId(user)).Returns(currentUser.Id);
            _mockUserManager.Setup(um => um.GetUserAsync(user)).ReturnsAsync(currentUser);

            var overtime1 = new Overtime
            {
                Id_OT = 1,
                PersonalAction = new PersonalAction { User = currentUser }
            };
            var overtime2 = new Overtime
            {
                Id_OT = 2,
                PersonalAction = new PersonalAction { User = currentUser }
            };

            _context.Overtimes.AddRange(overtime1, overtime2);
            await _context.SaveChangesAsync();

            var result = await _overtimeService.GetOvertimes(user);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(overtime1.Id_OT, result[0].Id_OT);
            Assert.AreEqual(overtime2.Id_OT, result[1].Id_OT);
        }


        [Test]
        public async Task GetOvertimes_SupervisorDptoRole_ReturnsDepartmentOvertimes()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "supervisor1"),
                new Claim(ClaimTypes.Role, "SupervisorDpto")
            }));

            var supervisor = new User { Id = "supervisor1", Dni = "123456789", Name = "Camila", LastName = "Ulate", DepartmentId = 1 };
            _mockUserManager.Setup(um => um.GetUserId(user)).Returns(supervisor.Id);
            _mockUserManager.Setup(um => um.GetUserAsync(user)).ReturnsAsync(supervisor);

            var user1 = new User { Id = "usuario1", Dni = "111111111", Name = "Ian", LastName = "Calvo", DepartmentId = 1 };
            var user2 = new User { Id = "usuario2", Dni = "222222222", Name = "Fabiana", LastName = "Arias", DepartmentId = 2 };
            var user3 = new User { Id = "usuario3", Dni = "333333333", Name = "Yurgen", LastName = "Arias", DepartmentId = 1 };

            var overtime1 = new Overtime { Id_OT = 1, PersonalAction = new PersonalAction { User = user1 } };
            var overtime2 = new Overtime { Id_OT = 2, PersonalAction = new PersonalAction { User = user2 } };
            var overtime3 = new Overtime { Id_OT = 3, PersonalAction = new PersonalAction { User = user3 } };

            _context.Overtimes.AddRange(overtime1, overtime2, overtime3);
            await _context.SaveChangesAsync();

            var result = await _overtimeService.GetOvertimes(user);

            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public async Task GetOvertimes_SupervisorRHRole_ReturnsAllOvertimesExceptCurrentUser()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "supervisor2"),
                new Claim(ClaimTypes.Role, "SupervisorRH")
            }));

            var supervisor = new User { Id = "supervisor1", Dni = "111111111", Name = "Camila", LastName = "Ulate", DepartmentId = 1 };
            _mockUserManager.Setup(um => um.GetUserId(user)).Returns(supervisor.Id);
            _mockUserManager.Setup(um => um.GetUserAsync(user)).ReturnsAsync(supervisor);

            var user1 = new User { Id = "usuario1", Dni = "111111111", Name = "Ian", LastName = "Calvo", DepartmentId = 1 };
            var user2 = new User { Id = "usuario2", Dni = "222222222", Name = "Fabiana", LastName = "Arias", DepartmentId = 2 };

            var overtime1 = new Overtime { Id_OT = 1, PersonalAction = new PersonalAction { User = user1 } };
            var overtime2 = new Overtime { Id_OT = 2, PersonalAction = new PersonalAction { User = user2 } };

            _context.Overtimes.AddRange(overtime1, overtime2);
            await _context.SaveChangesAsync();

            var result = await _overtimeService.GetOvertimes(user);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(overtime1.Id_OT, result[0].Id_OT);
            Assert.AreEqual(overtime2.Id_OT, result[1].Id_OT);
        }

        [Test]
        public async Task GetOvertimeCount_UserHasApprovedOvertime_ReturnsCorrectCount()
        {
            var user1 = new User { Id = "usuario1", Dni = "111111111", Name = "Ian", LastName = "Calvo", DepartmentId = 1 };

            var approvedOvertime1 = new Overtime { Id_OT = 1, PersonalAction = new PersonalAction { User = user1, Status = SGRH.Web.Enums.Status.Aprobado } };
            var approvedOvertime2 = new Overtime { Id_OT = 2, PersonalAction = new PersonalAction { User = user1, Status = SGRH.Web.Enums.Status.Aprobado } };
            var deniedOvertime = new Overtime { Id_OT = 3, PersonalAction = new PersonalAction { User = user1, Status = SGRH.Web.Enums.Status.Rechazado } };

            _context.Overtimes.AddRange(approvedOvertime1, approvedOvertime2, deniedOvertime);
            await _context.SaveChangesAsync();

            var count = await _overtimeService.GetOvertimeCount(user1.Id);

            Assert.AreEqual(2, count);
        }

    }
}
