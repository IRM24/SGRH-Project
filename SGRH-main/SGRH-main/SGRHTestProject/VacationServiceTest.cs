using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SGRH.Web.Models.Data;
using SGRH.Web.Models.Entities;
using SGRH.Web.Services;
using System.Threading.Tasks;

namespace SGRHTestProject
{
    public class VacationServiceTest
    {
        private VacationService _vacationService;
        private Mock<UserManager<User>> _userManagerMock;
        private SgrhContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<SgrhContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new SgrhContext(options);

            var testUser = new User
            {
                Id = "user1",
                Name = "Yurgen",
                LastName = "Cambronero",
                Dni = "202403",
                VacationDays = 10
            };

            _context.Users.Add(testUser);
            _context.SaveChanges();

            var userStoreMock = new Mock<IUserStore<User>>();
            _userManagerMock = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

            _userManagerMock.Setup(um => um.FindByIdAsync(testUser.Id)).ReturnsAsync(testUser);

            _vacationService = new VacationService(_context, _userManagerMock.Object);
        }

        [Test]
        public async Task AddInitialVacationDays_ValidRequest_IncrementsVacationDays()
        {
            var initialVacationDaysToAdd = 3;

            var result = await _vacationService.AddInitialVacationDays("user1", initialVacationDaysToAdd);
            var updatedUser = await _context.Users.FindAsync("user1");

            Assert.IsTrue(result.success); 
            Assert.AreEqual(13, updatedUser.VacationDays); 
        }

        [Test]
        public async Task VacationBalance_ValidUser_ReturnsCorrectBalance()
        {
            var balance = await _vacationService.VacationBalance("user1");

            Assert.AreEqual(10, balance);
        }



        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
