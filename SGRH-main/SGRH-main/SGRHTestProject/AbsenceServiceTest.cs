using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SGRH.Web.Models;
using SGRH.Web.Models.Data;
using SGRH.Web.Models.Entities;
using SGRH.Web.Services;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SGRHTestProject
{
    [TestFixture]
    public class AbsenceServiceTests
    {
        private AbsenceService _absenceService;
        private SgrhContext _context;
        private Mock<UserManager<User>> _mockUserManager;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<SgrhContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new SgrhContext(options);

            // Mocking UserManager
            var userStoreMock = new Mock<IUserStore<User>>();
            _mockUserManager = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

            // Inyectar _mockUserManager en AbsenceService
            _absenceService = new AbsenceService(_context, _mockUserManager.Object);
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
                new AbsenceCategory { Id_Absence_Category = 1, Category_Name = "Enfermedad" },
                new AbsenceCategory { Id_Absence_Category = 2, Category_Name = "Vacaciones" }
            };

            _context.AbsenceCategories.AddRange(categories);
            await _context.SaveChangesAsync();

            // Act
            var result = await _absenceService.GetAbsenceCategories();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Enfermedad", result[0].Category_Name);
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
            var category = new AbsenceCategory { Id_Absence_Category = 1, Category_Name = "Enfermedad" };
            _context.AbsenceCategories.Add(category);
            await _context.SaveChangesAsync();

            // Act
            var result = await _absenceService.GetAbsenceCategories();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Enfermedad", result[0].Category_Name);
        }

        [Test]
        public async Task GetAbsenceCategories_AddMultipleCategories_ReturnsAllCategories()
        {
            // Arrange
            var categories = new List<AbsenceCategory>
            {
                new AbsenceCategory { Id_Absence_Category = 1, Category_Name = "Enfermedad" },
                new AbsenceCategory { Id_Absence_Category = 2, Category_Name = "Vacaciones" },
                new AbsenceCategory { Id_Absence_Category = 3, Category_Name = "Asuntos personales" }
            };

            _context.AbsenceCategories.AddRange(categories);
            await _context.SaveChangesAsync();

            // Act
            var result = await _absenceService.GetAbsenceCategories();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("Enfermedad", result[0].Category_Name);
            Assert.AreEqual("Vacaciones", result[1].Category_Name);
            Assert.AreEqual("Asuntos personales", result[2].Category_Name);
        }


        [Test]
        public async Task GetAbsences_UserIsEmpleado_ReturnsOnlyUserAbsences()
        {
            // Arrange
            var userId = "usuario123";
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] 
            { 
                new Claim(ClaimTypes.Role, "Empleado"), 
                new Claim(ClaimTypes.NameIdentifier, userId) 
            }));

            // Proporcionar valores para las propiedades requeridas
            var currentUser = new User 
            { 
                Id = userId, 
                Dni = "123456789", 
                Name = "Ian", 
                LastName = "Calvo" 
            };

            var absences = new List<Absence>
            {
                new Absence 
                { 
                    AbsenceId = 1, 
                    User = currentUser, 
                    AbsenceCategory = new AbsenceCategory { Category_Name = "Enfermedad" } 
                },
                new Absence 
                { 
                    AbsenceId = 2, 
                    User = new User { Id = "otroUsuario", Dni = "987654321", Name = "Camila", LastName = "Ulate" }, 
                    AbsenceCategory = new AbsenceCategory { Category_Name = "Vacaciones" } 
                }
            };

            _context.Absences.AddRange(absences);
            await _context.SaveChangesAsync();

            _mockUserManager.Setup(um => um.GetUserId(user)).Returns(userId);
            _mockUserManager.Setup(um => um.GetUserAsync(user)).ReturnsAsync(currentUser);
            _mockUserManager.Setup(um => um.GetRolesAsync(currentUser)).ReturnsAsync(new List<string> { "Empleado" });

            // Act
            var result = await _absenceService.GetAbsences(user);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Enfermedad", result.First().AbsenceCategory.Category_Name);
        }




        [Test]
        public async Task GetAbsences_UserIsSupervisorDpto_ReturnsDepartmentAbsences()
        {
            // Arrange
            var currentUser = new User { Id = "supervisorId", Dni = "111111111", Name = "Fabiana", LastName = "Arias", DepartmentId = 1 };
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Role, "SupervisorDpto"), new Claim(ClaimTypes.NameIdentifier, currentUser.Id) }));

            var user1 = new User
            {
                Id = "empleado1",
                Dni = "123456789",
                Name = "Ian",
                LastName = "Calvo",
                DepartmentId = 1
            };

            var user2 = new User
            {
                Id = "empleado2",
                Dni = "123456789",
                Name = "Camila",
                LastName = "Ulate",
                DepartmentId = 2
            };

            var absences = new List<Absence>
            {
                new Absence { AbsenceId = 1, User = user1, AbsenceCategory = new AbsenceCategory { Category_Name = "Enfermedad" } },
                new Absence { AbsenceId = 2, User = user2, AbsenceCategory = new AbsenceCategory { Category_Name = "Vacaciones" } }
            };
            _context.Absences.AddRange(absences);
            await _context.SaveChangesAsync();

            _mockUserManager.Setup(um => um.GetUserId(user)).Returns(currentUser.Id);
            _mockUserManager.Setup(um => um.GetUserAsync(user)).ReturnsAsync(currentUser);
            _mockUserManager.Setup(um => um.GetRolesAsync(currentUser)).ReturnsAsync(new List<string> { "SupervisorDpto" });

            // Act
            var result = await _absenceService.GetAbsences(user);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Enfermedad", result.First().AbsenceCategory.Category_Name);
        }



        [Test]
        public async Task GetAbsences_UserIsNotEmpleadoOrSupervisor_ReturnsAllAbsences()
        {
            // Arrange
            var currentUser = new User { Id = "adminId", Dni = "111111111", Name = "Fabiana", LastName = "Arias" };
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.NameIdentifier, currentUser.Id) }));

            var user1 = new User
            {
                Id = "empleado1",
                Dni = "123456789",
                Name = "Ian",
                LastName = "Calvo"
            };

            var user2 = new User
            {
                Id = "empleado2",
                Dni = "123456789",
                Name = "Camila",
                LastName = "Ulate"
            };


            var absences = new List<Absence>
            {
                new Absence { AbsenceId = 1, User = user1, AbsenceCategory = new AbsenceCategory { Category_Name = "Enfermedad" } },
                new Absence { AbsenceId = 2, User = user2, AbsenceCategory = new AbsenceCategory { Category_Name = "Vacaciones" } }
            };
            _context.Absences.AddRange(absences);
            await _context.SaveChangesAsync();

            _mockUserManager.Setup(um => um.GetUserId(user)).Returns(currentUser.Id);
            _mockUserManager.Setup(um => um.GetUserAsync(user)).ReturnsAsync(currentUser);
            _mockUserManager.Setup(um => um.GetRolesAsync(currentUser)).ReturnsAsync(new List<string>());

            // Act
            var result = await _absenceService.GetAbsences(user);

            // Assert
            Assert.AreEqual(2, result.Count);
        }



        [Test]
        public async Task RegisterAbsence_ValidData_ReturnsTrue()
        {
            // Arrange
            var userId = "usuario123";
            var absenceModel = new AbsenceViewModel
            {
                Category = "1",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(5),
                Comments = "Enfermedad"
            };

            var documentContents = new List<byte[]> { new byte[] { 0x01, 0x02, 0x03 } };
            var documentFileNames = new List<string> { "documento1.pdf" };
            var documentContentTypes = new List<string> { "aplicacion/pdf" };

            var user = new User { Id = userId, Name = "John", LastName = "Doe", Dni = "123456789" };
            var absenceCategory = new AbsenceCategory { Id_Absence_Category = 1, Category_Name = "Enfermedad" };

            _context.Users.Add(user);
            _context.AbsenceCategories.Add(absenceCategory);
            await _context.SaveChangesAsync();

            // Act
            var result = await _absenceService.RegisterAbsence(userId, absenceModel, documentContents, documentFileNames, documentContentTypes);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(1, _context.Absences.Count());
        }



        [Test]
        public async Task RegisterAbsence_UserNotFound_ReturnsFalse()
        {
            // Arrange
            var userId = "usuarioInvalido";
            var absenceModel = new AbsenceViewModel
            {
                Category = "1",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(5),
                Comments = "Enfermedad"
            };

            var documentContents = new List<byte[]> { new byte[] { 0x01, 0x02, 0x03 } };
            var documentFileNames = new List<string> { "documento1.pdf" };
            var documentContentTypes = new List<string> { "aplicacion/pdf" };

            // Act
            var result = await _absenceService.RegisterAbsence(userId, absenceModel, documentContents, documentFileNames, documentContentTypes);

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(0, _context.Absences.Count());
        }



        [Test]
        public async Task UpdateAbsence_ValidData_ReturnsTrue()
        {
            // Arrange
            var userId = "usuario123";
            var absenceModel = new AbsenceViewModel
            {
                Category = "1",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(5),
                Comments = "Actualizacion enfermedad",
                Documentation = new List<IFormFile> // Simular los archivos que se están subiendo
        {
            new FormFile(new MemoryStream(new byte[] { 0x01, 0x02, 0x03 }), 0, 3, "Data", "documentoActualizado.pdf")
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/pdf"
            }
        }
            };

            var user = new User { Id = userId, Name = "Camila", LastName = "Ulate", Dni = "123456789" };
            var absenceCategory = new AbsenceCategory { Id_Absence_Category = 1, Category_Name = "Enfermedad" };

            var absence = new Absence
            {
                AbsenceId = 1,
                User = user,
                AbsenceCategory = absenceCategory,
                Start_Date = DateTime.Now.AddDays(-5),
                End_Date = DateTime.Now.AddDays(-1),
                Absence_Comments = "Comentarios anteriores",
                Document = new List<Document> { new Document { FileName = "documentoAnterior.pdf", Content = new byte[] { 0x04 }, ContentType = "application/pdf" } }
            };

            _context.Users.Add(user);
            _context.AbsenceCategories.Add(absenceCategory);
            _context.Absences.Add(absence);
            await _context.SaveChangesAsync();

            // Act
            var result = await _absenceService.UpdateAbsence(userId, absence.AbsenceId, absenceModel, documentContents: null, documentFileNames: null, documentContentTypes: null);

            // Assert
            Assert.IsTrue(result);
            var updatedAbsence = await _context.Absences.Include(a => a.Document).FirstOrDefaultAsync(a => a.AbsenceId == absence.AbsenceId);
            Assert.IsNotNull(updatedAbsence);
            Assert.AreEqual(absenceModel.Comments, updatedAbsence.Absence_Comments);
            Assert.AreEqual(1, updatedAbsence.Document.Count);
            Assert.AreEqual("documentoActualizado.pdf", updatedAbsence.Document.First().FileName);
        }




        [Test]
        public async Task UpdateAbsence_AbsenceNotFound_ReturnsFalse()
        {
            // Arrange
            var userId = "usuario123";
            var absenceModel = new AbsenceViewModel
            {
                Category = "1",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(5),
                Comments = "Enfermedad"
            };

            var documentContents = new List<byte[]> { new byte[] { 0x01, 0x02, 0x03 } };
            var documentFileNames = new List<string> { "documentoActualizado.pdf" };
            var documentContentTypes = new List<string> { "application/pdf" };

            // Act
            var result = await _absenceService.UpdateAbsence(userId, 999, absenceModel, documentContents, documentFileNames, documentContentTypes); // Id de ausencia inexistente

            // Assert
            Assert.IsFalse(result);
        }



        [Test]
        public async Task DeleteAbsence_ValidUserAndAbsenceId_ReturnsTrue()
        {
            // Arrange
            var userId = "supervisor123";
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, "SupervisorRh") // Rol autorizado
            }));

            var absence = new Absence
            {
                AbsenceId = 1,
                User = new User { Id = userId, Dni = "222222222", Name = "Ian", LastName = "Calvo" },
                AbsenceCategory = new AbsenceCategory { Id_Absence_Category = 1, Category_Name = "Sick Leave" },
                Document = new List<Document>()
            };

            _context.Absences.Add(absence);
            await _context.SaveChangesAsync();

            // Act
            var result = await _absenceService.DeleteAbsence(user, absence.AbsenceId);

            // Assert
            Assert.IsTrue(result);
            var deletedAbsence = await _context.Absences.FindAsync(absence.AbsenceId);
            Assert.IsNull(deletedAbsence); // La ausencia debe haber sido eliminada
        }



        [Test]
        public async Task DeleteAbsence_AbsenceDoesNotExist_ReturnsFalse()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "usuarioId"),
                new Claim(ClaimTypes.Role, "SupervisorRh")
            }));

            // Act
            var result = await _absenceService.DeleteAbsence(user, 999); // ID que no existe

            // Assert
            Assert.IsFalse(result); // Debe devolver false
        }


        [Test]
        public async Task DeleteAbsence_UserNotAuthorized_ReturnsFalse()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "usuarioId"),
                new Claim(ClaimTypes.Role, "Empleado") // Rol no autorizado
            }));

            var absence = new Absence
            {
                AbsenceId = 1,
                User = new User { Id = "supervisor123", Name = "Camila Ulate" },
                AbsenceCategory = new AbsenceCategory { Id_Absence_Category = 1, Category_Name = "Sick Leave" },
                Document = new List<Document>()
            };

            _context.Absences.Add(absence);
            await _context.SaveChangesAsync();

            // Act
            var result = await _absenceService.DeleteAbsence(user, absence.AbsenceId);

            // Assert
            Assert.IsFalse(result); // Debe devolver false porque el usuario no tiene permiso
        }


        [Test]
        public async Task GetAbsenceById_ValidId_ReturnsAbsence()
        {
            // Arrange
            var absenceId = 1;
            var user = new User { Id = "usuario123", Name = "Camila", LastName = "Ulate", Dni = "123456789" };
            var absenceCategory = new AbsenceCategory { Id_Absence_Category = 1, Category_Name = "Enfermedad" };
            var absence = new Absence
            {
                AbsenceId = absenceId,
                User = user,
                AbsenceCategory = absenceCategory,
                Start_Date = DateTime.Now,
                End_Date = DateTime.Now.AddDays(5),
                Absence_Comments = "Ausencia por dolor de espalda",
                Document = new List<Document>
                {
                    new Document { FileName = "documento.pdf", Content = new byte[] { 0x01 }, ContentType = "application/pdf" }
                }
                    };

            _context.Absences.Add(absence);
            await _context.SaveChangesAsync();

            // Act
            var result = await _absenceService.GetAbsenceById(absenceId);

            // Assert
            Assert.IsNotNull(result); // Verifica que se haya encontrado una ausencia
            Assert.AreEqual(absenceId, result.AbsenceId); // Verifica que el ID de la ausencia sea correcto
        }



        [Test]
        public async Task DownloadDocument_ValidId_ReturnsDocumentViewModel()
        {
            // Arrange
            var documentId = 1;
            var document = new Document
            {
                Id = documentId,
                FileName = "documentoPrueba.pdf",
                ContentType = "application/pdf",
                Size = 1024,
                Content = new byte[] { 0x01, 0x02, 0x03 }
            };

            _context.Document.Add(document);
            await _context.SaveChangesAsync();

            // Act
            var result = await _absenceService.DownloadDocument(documentId);

            // Assert
            Assert.IsNotNull(result); // Verifica que se haya encontrado un documento
            Assert.AreEqual(documentId, result.Id); // Verifica que el ID del documento sea correcto
            Assert.AreEqual("documentoPrueba.pdf", result.FileName); // Verifica que el nombre del archivo sea correcto
        }




        [Test]
        public async Task GetAbsencesByUser_ExistingUserId_ReturnsUserAbsences()
        {
            var user1 = new User { Id = "usuario1", Name = "Camila", LastName = "Ulate", Dni = "123456789" };
            var user2 = new User { Id = "usuario2", Name = "Fabiana", LastName = "Arias", Dni = "111111111" };
            // Arrange
            var absence1 = new Absence
            {
                AbsenceId = 1,
                User = user1,
                AbsenceCategory = new AbsenceCategory { Category_Name = "Enfermedad" },
                Start_Date = DateTime.Now.AddDays(-5),
                End_Date = DateTime.Now,
                Absence_Comments = "Incapacidad por gripe"
            };

            var absence2 = new Absence
            {
                AbsenceId = 2,
                User = user1,
                AbsenceCategory = new AbsenceCategory { Category_Name = "Asuntos Personales" },
                Start_Date = DateTime.Now.AddDays(-5),
                End_Date = DateTime.Now,
                Absence_Comments = "Asuntos privados"
            };

            var absence3 = new Absence
            {
                AbsenceId = 3,
                User = user2,
                AbsenceCategory = new AbsenceCategory { Category_Name = "Vacaciones" },
                Start_Date = DateTime.Now.AddDays(-10),
                End_Date = DateTime.Now.AddDays(-5),
                Absence_Comments = "Feriado"
            };

            _context.Absences.AddRange(absence1, absence2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _absenceService.GetAbsencesByUser(user1.Id);

            // Assert
            Assert.AreEqual(2, result.Count); // Verifica que se devuelvan las dos ausencias
            Assert.AreEqual("Enfermedad", result[0].AbsenceCategory.Category_Name); // Verifica la categoría de la primera ausencia
        }



    }
}
