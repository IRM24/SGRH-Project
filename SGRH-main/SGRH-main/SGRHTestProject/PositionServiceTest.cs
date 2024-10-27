using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SGRH.Web.Models.Data;
using SGRH.Web.Models.Entities;
using SGRH.Web.Services;
using System.Threading.Tasks;

namespace SGRHTestProject
{
    [TestFixture] 
    internal class PositionServiceTest
    {
        private PositionService _positionService;
        private SgrhContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<SgrhContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new SgrhContext(options);
            _positionService = new PositionService(_context);
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
        public async Task CreatePositions_CreatesPositionSuccessfully()
        {
            var position = new Position
            {
                Position_Name = "Desarrollador",
                DepartmentId = 1
            };

            var result = await _positionService.CreatePositions(position);

            var createdPosition = await _context.Positions.FirstOrDefaultAsync(p => p.Position_Name == "Desarrollador");
            Assert.IsNotNull(createdPosition);
        }

        [Test]
        public async Task CreatePositions_FailsOnException()
        {
            var position = new Position
            {
                Position_Name = null, 
                DepartmentId = 1
            };

            var result = await _positionService.CreatePositions(position);

            Assert.IsFalse(result.success);
        }

        [Test]
        public async Task CreatePositions_FailsWhenPositionAlreadyExists()
        {
            var position = new Position
            {
                Position_Name = "Gerente",
                DepartmentId = 1
            };

            await _positionService.CreatePositions(position);

            var result = await _positionService.CreatePositions(position);

            Assert.IsFalse(result.success);
        }

        [Test]
        public async Task UpdatePositions_UpdatesDepartmentIdSuccessfully()
        {
            var position = new Position
            {
                Position_Name = "Desarrollador",
                DepartmentId = 1
            };
            _context.Positions.Add(position);
            await _context.SaveChangesAsync();

            position.DepartmentId = 2; 

            var result = await _positionService.UpdatePositions(position);
            var updatedPosition = await _context.Positions.FindAsync(position.Id_Position);

            Assert.IsTrue(result.success);
            Assert.AreEqual(2, updatedPosition.DepartmentId); 
        }

        [Test]
        public async Task UpdatePositions_UpdatesPositionSuccessfully()
        {
            var position = new Position
            {
                Position_Name = "Analista",
                DepartmentId = 1
            };
            _context.Positions.Add(position);
            await _context.SaveChangesAsync();

            position.Position_Name = "Analista Senior";

            var updatedPosition = await _context.Positions.FindAsync(position.Id_Position);
            Assert.AreEqual("Analista Senior", updatedPosition.Position_Name);
        }

        [Test]
        public async Task UpdatePositions_FailsIfPositionDoesNotExist()
        {
            var nonExistingPosition = new Position
            {
                Id_Position = 21,
                Position_Name = "Gerente",
                DepartmentId = 1
            };

            var result = await _positionService.UpdatePositions(nonExistingPosition);

            Assert.IsFalse(result.success);
        }

        [Test]
        public async Task DeletePosition_DeletesPositionSuccessfully()
        {
            var position = new Position
            {
                Position_Name = "Desarrollador",
                DepartmentId = 1
            };
            _context.Positions.Add(position);
            await _context.SaveChangesAsync();

            var result = await _positionService.DeletePosition(position.Id_Position);

            Assert.IsTrue(result.success);

            var deletedPosition = await _context.Positions.FindAsync(position.Id_Position);
            Assert.IsNull(deletedPosition);
        }

        [Test]
        public async Task DeletePosition_FailsIfPositionDoesNotExist()
        {
            var result = await _positionService.DeletePosition(21); 

            Assert.IsFalse(result.success);
        }

        [Test]
        public async Task GetPositionById_ReturnsPosition()
        {
            var position = new Position
            {
                Position_Name = "Asegurador de la Calidad",
                DepartmentId = 2
            };
            _context.Positions.Add(position);
            await _context.SaveChangesAsync();

            var result = await _positionService.GetPositionById(position.Id_Position);

            Assert.IsNotNull(result);
            Assert.AreEqual("Asegurador de la Calidad", result.Position_Name);
        }

        [Test]
        public async Task GetPositionById_ReturnsNullIfNotFound()
        {
            var result = await _positionService.GetPositionById(31); 

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetPositions_ReturnsListOfPositions()
        {
            var positions = new List<Position>
            {
                new Position { Position_Name = "Gerente", DepartmentId = 1 },
                new Position { Position_Name = "Desarrollador", DepartmentId = 2 }
            };

            _context.Positions.AddRange(positions);
            await _context.SaveChangesAsync();

            var result = await _positionService.GetPositions();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Gerente", result[0].Position_Name);
            Assert.AreEqual("Desarrollador", result[1].Position_Name);
        }

        [Test]
        public async Task GetPositions_ReturnsEmptyList_WhenNoPositionsExist()
        {
            var result = await _positionService.GetPositions();

            Assert.IsNotNull(result);
            Assert.IsEmpty(result); 
        }


        [Test]
        public async Task GetPositionsCount_ReturnsCorrectCount()
        {
            _context.Positions.Add(new Position { Position_Name = "Gerente de proyectos", DepartmentId = 1 });
            _context.Positions.Add(new Position { Position_Name = "Asegurador de la Calidad", DepartmentId = 2 });
            await _context.SaveChangesAsync();

            var count = await _positionService.GetPositionsCount();

            Assert.AreEqual(2, count);
        }

    }
}
