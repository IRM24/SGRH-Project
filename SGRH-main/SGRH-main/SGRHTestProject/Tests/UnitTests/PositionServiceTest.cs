using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SGRH.Web.Models.Data;
using SGRH.Web.Models.Entities;
using SGRH.Web.Services;
using System.Threading.Tasks;

namespace SGRHTestProject.Tests.UnitTests
{
    [TestFixture] // Atributo que marca esta clase como un conjunto de pruebas
    internal class PositionServiceTest
    {
        private PositionService _positionService;
        private SgrhContext _context;

        // Configuración antes de cada prueba
        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<SgrhContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new SgrhContext(options);
            _positionService = new PositionService(_context);
        }

        // Limpieza después de cada prueba
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
            // Arrange
            var position = new Position
            {
                Position_Name = "Desarrollador",
                DepartmentId = 1
            };

            // Act
            var result = await _positionService.CreatePositions(position);

            // Assert
            var createdPosition = await _context.Positions.FirstOrDefaultAsync(p => p.Position_Name == "Desarrollador");
            Assert.IsNotNull(createdPosition);
        }

        [Test]
        public async Task CreatePositions_FailsOnException()
        {
            // Arrange
            var position = new Position
            {
                Position_Name = null, // Esto causará un fallo al guardar
                DepartmentId = 1
            };

            // Act
            var result = await _positionService.CreatePositions(position);

            // Assert
            Assert.IsFalse(result.success);
        }

        [Test]
        public async Task CreatePositions_FailsWhenPositionAlreadyExists()
        {
            // Arrange
            var position = new Position
            {
                Position_Name = "Gerente",
                DepartmentId = 1
            };

            // Agregar una posición inicial
            await _positionService.CreatePositions(position);

            // Act - Intentar agregar la misma posición otra vez
            var result = await _positionService.CreatePositions(position);

            // Assert
            Assert.IsFalse(result.success);
        }

        [Test]
        public async Task UpdatePositions_UpdatesDepartmentIdSuccessfully()
        {
            // Arrange
            var position = new Position
            {
                Position_Name = "Desarrollador",
                DepartmentId = 1
            };
            _context.Positions.Add(position);
            await _context.SaveChangesAsync();

            position.DepartmentId = 2; // Cambiando el DepartmentId

            // Act
            var result = await _positionService.UpdatePositions(position);
            var updatedPosition = await _context.Positions.FindAsync(position.Id_Position);

            // Assert
            Assert.IsTrue(result.success);
            Assert.AreEqual(2, updatedPosition.DepartmentId); // Verifica que se actualice correctamente
        }

        [Test]
        public async Task UpdatePositions_UpdatesPositionSuccessfully()
        {
            // Arrange
            var position = new Position
            {
                Position_Name = "Analista",
                DepartmentId = 1
            };
            _context.Positions.Add(position);
            await _context.SaveChangesAsync();

            position.Position_Name = "Analista Senior";

            // Act
            var updatedPosition = await _context.Positions.FindAsync(position.Id_Position);
            Assert.AreEqual("Analista Senior", updatedPosition.Position_Name);
        }

        [Test]
        public async Task UpdatePositions_FailsIfPositionDoesNotExist()
        {
            // Arrange
            var nonExistingPosition = new Position
            {
                Id_Position = 21,
                Position_Name = "Gerente",
                DepartmentId = 1
            };

            // Act
            var result = await _positionService.UpdatePositions(nonExistingPosition);

            // Assert
            Assert.IsFalse(result.success);
        }

        [Test]
        public async Task DeletePosition_DeletesPositionSuccessfully()
        {
            // Arrange
            var position = new Position
            {
                Position_Name = "Desarrollador",
                DepartmentId = 1
            };
            _context.Positions.Add(position);
            await _context.SaveChangesAsync();

            // Act
            var result = await _positionService.DeletePosition(position.Id_Position);

            // Assert
            Assert.IsTrue(result.success);

            var deletedPosition = await _context.Positions.FindAsync(position.Id_Position);
            Assert.IsNull(deletedPosition);
        }

        [Test]
        public async Task DeletePosition_FailsIfPositionDoesNotExist()
        {
            // Act
            var result = await _positionService.DeletePosition(21); // ID que no existe

            // Assert
            Assert.IsFalse(result.success);
        }

        [Test]
        public async Task GetPositionById_ReturnsPosition()
        {
            // Arrange
            var position = new Position
            {
                Position_Name = "Asegurador de la Calidad",
                DepartmentId = 2
            };
            _context.Positions.Add(position);
            await _context.SaveChangesAsync();

            // Act
            var result = await _positionService.GetPositionById(position.Id_Position);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Asegurador de la Calidad", result.Position_Name);
        }

        [Test]
        public async Task GetPositionById_ReturnsNullIfNotFound()
        {
            // Act
            var result = await _positionService.GetPositionById(31); // ID que no existe

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetPositions_ReturnsListOfPositions()
        {
            // Arrange
            var positions = new List<Position>
            {
                new Position { Position_Name = "Gerente", DepartmentId = 1 },
                new Position { Position_Name = "Desarrollador", DepartmentId = 2 }
            };

            _context.Positions.AddRange(positions);
            await _context.SaveChangesAsync();

            // Act
            var result = await _positionService.GetPositions();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Gerente", result[0].Position_Name);
            Assert.AreEqual("Desarrollador", result[1].Position_Name);
        }

        [Test]
        public async Task GetPositions_ReturnsEmptyList_WhenNoPositionsExist()
        {
            // Act
            var result = await _positionService.GetPositions();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result); // Verifica que la lista esté vacía
        }


        [Test]
        public async Task GetPositionsCount_ReturnsCorrectCount()
        {
            // Arrange
            _context.Positions.Add(new Position { Position_Name = "Gerente de proyectos", DepartmentId = 1 });
            _context.Positions.Add(new Position { Position_Name = "Asegurador de la Calidad", DepartmentId = 2 });
            await _context.SaveChangesAsync();

            // Act
            var count = await _positionService.GetPositionsCount();

            // Assert
            Assert.AreEqual(2, count);
        }

    }
}
