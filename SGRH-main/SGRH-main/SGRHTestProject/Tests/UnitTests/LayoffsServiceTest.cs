using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SGRH.Web.Enums;
using SGRH.Web.Models.Data;
using SGRH.Web.Models.Entities;
using SGRH.Web.Models.ViewModels;
using SGRH.Web.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SGRHTestProject.Tests.UnitTests
{
    [TestFixture]
    public class LayoffsServiceTests
    {
        private LayoffsService _layoffsService;
        private SgrhContext _context;
        private Mock<IPersonalActionService> _personalActionServiceMock;
        private Mock<IServiceUser> _serviceUserMock;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<SgrhContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new SgrhContext(options);
            _personalActionServiceMock = new Mock<IPersonalActionService>();
            _serviceUserMock = new Mock<IServiceUser>();
            _layoffsService = new LayoffsService(_context, _personalActionServiceMock.Object, _serviceUserMock.Object);
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
        public async Task CreateLayoff_UserSelfDismissal_ReturnsFailure()
        {
            var userId = "21";
            var model = new CreateLayoffViewModel
            {
                userId = userId,
                currentUserId = userId,
                DismissalCause = "Comportamiento inapropiado",
                DismissalDate = DateTime.UtcNow,
                HasEmployerResponsibility = true,
                RegisteredBy = userId
            };

            var result = await _layoffsService.CreateLayoff(model);

            Assert.IsFalse(result.success);
        }

        [Test]
        public async Task DeleteLayoff_LayoffNotFound_ReturnsFailure()
        {
            var result = await _layoffsService.DeleteLayoff(1); 

            Assert.IsFalse(result.success);
        }

        [Test]
        public async Task GetLayoffById_InvalidId_ReturnsNull()
        {
            var result = await _layoffsService.GetLayoffById(2); 

            Assert.IsNull(result);
        }


    }
}
