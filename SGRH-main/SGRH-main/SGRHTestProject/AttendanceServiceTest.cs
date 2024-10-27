using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SGRH.Web.Models;
using SGRH.Web.Models.Data;
using SGRH.Web.Models.Entities;
using SGRH.Web.Services;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SGRHTestProject
{
    [TestFixture]
    public class AttendanceServiceTests
    {
        private AttendanceService _attendanceService;
        private SgrhContext _context;
        private Mock<UserManager<User>> _mockUserManager;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<SgrhContext>()
                .UseInMemoryDatabase(databaseName: "AttendanceTestDatabase")
                .Options;

            _context = new SgrhContext(options);

            var userStoreMock = new Mock<IUserStore<User>>();
            _mockUserManager = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

            _attendanceService = new AttendanceService(_context, _mockUserManager.Object);
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
        public async Task RegisterEntry_UserDoesNotHaveEntry_ReturnsTrue()
        {
            var userId = "usuario1";
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }));

            var currentUser = new User
            {
                Id = userId,
                Dni = "202433",
                Name = "Fabiana",
                LastName = "Arias"
            };

            _mockUserManager.Setup(um => um.GetUserAsync(user)).ReturnsAsync(currentUser);
            _mockUserManager.Setup(um => um.GetUserId(user)).Returns(userId);

            var result = await _attendanceService.RegisterEntry(userId);

            var attendanceEntry = await _context.Attendances.FirstOrDefaultAsync(a => a.UserId == userId);
            Assert.IsNotNull(attendanceEntry);
            Assert.IsNotNull(attendanceEntry.EntryTime);
        }


        [Test]
        public async Task RegisterEntry_UserAlreadyHasEntry_ReturnsFalse()
        {
            var userId = "usuario1";
            var currentDate = DateTime.Now;

            var attendance = new Attendance
            {
                UserId = userId,
                EntryTime = currentDate,
                Date = currentDate.Date
            };
            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();

            var result = await _attendanceService.RegisterEntry(userId);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task RegisterEntry_AlreadyHasEntryForToday_ReturnsFalse()
        {
            var userId = "user1";
            var user = new User { Id = userId, Dni = "202409", Name = "Ian", LastName = "Calvo" };
            var existingAttendance = new Attendance
            {
                UserId = userId,
                Date = DateTime.Today,
                EntryTime = DateTime.Now
            };

            _context.Attendances.Add(existingAttendance);
            await _context.SaveChangesAsync();

            _mockUserManager.Setup(um => um.FindByIdAsync(userId)).ReturnsAsync(user);
            _mockUserManager.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);

            var result = await _attendanceService.RegisterEntry(userId);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task HasEntryForToday_ReturnsTrueWhenEntryExists()
        {
            var userId = "user3";
            var attendance = new Attendance
            {
                UserId = userId,
                Date = DateTime.Today,
                EntryTime = DateTime.Now
            };

            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();

            var result = await _attendanceService.HasEntryForToday(userId);

            Assert.IsTrue(result);
        }

        [Test]
        public async Task HasEntryForToday_ReturnsFalseWhenNoEntryExists()
        {
            var userId = "user4";

            var result = await _attendanceService.HasEntryForToday(userId);

            Assert.IsFalse(result);
        }

        [Test]
        public async Task GetAttendances_UserIsAdmin_ReturnsAllAttendances()
        {
            var userId = "adminId";
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim(ClaimTypes.NameIdentifier, userId)
            }));

            var attendances = new List<Attendance>
            {
                new Attendance { UserId = "empleado1", EntryTime = DateTime.Now, Date = DateTime.Today },
                new Attendance { UserId = "empleado2", EntryTime = DateTime.Now, Date = DateTime.Today }
            };

            _context.Attendances.AddRange(attendances);
            await _context.SaveChangesAsync();

            _mockUserManager.Setup(um => um.GetUserId(user)).Returns(userId);

            var result = await _attendanceService.GetAttendances(user);

            Assert.AreEqual(2, result.Count());
        }


        [Test]
        public async Task GetAttendances_UserIsEmpleado_ReturnsUserAttendances()
        {
            var userId = "usuario1";
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, "Empleado"),
                new Claim(ClaimTypes.NameIdentifier, userId)
            }));

            var currentUser = new User
            {
                Id = userId,
                Name = "Ian",
                LastName = "Calvo"
            };

            var attendances = new List<Attendance>
            {
                new Attendance { UserId = userId, EntryTime = DateTime.Now, Date = DateTime.Today },
                new Attendance { UserId = userId, EntryTime = DateTime.Now.AddDays(-1), Date = DateTime.Today.AddDays(-1) }
            };

            _context.Attendances.AddRange(attendances);
            await _context.SaveChangesAsync();

            _mockUserManager.Setup(um => um.GetUserId(user)).Returns(userId);
            _mockUserManager.Setup(um => um.GetUserAsync(user)).ReturnsAsync(currentUser);

            var result = await _attendanceService.GetAttendances(user);

            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.All(a => a.UserId == userId));
        }

    }
}
