using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Moq;
using IS202.NrlApp.Controllers;
using IS202.NrlApp.Data;
using IS202.NrlApp.Models;
using Xunit;

namespace IS202.NrlApp.Tests.Controllers
{
    /// <summary>
    /// Enhetstester for ObstacleController.
    /// Bruker InMemory database for isolerte tester.
    /// </summary>
    public class ObstacleControllerTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly ObstacleController _controller;

        /// <summary>
        /// Oppsett som kjøres før hver test.
        /// Oppretter en InMemory database og controller.
        /// </summary>
        public ObstacleControllerTests()
        {
            // Opprett InMemory database med unikt navn for hver test
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _controller = new ObstacleController(_context);
            
            // Sett opp TempData
            _controller.TempData = new TempDataDictionary(
                new DefaultHttpContext(), 
                Mock.Of<ITempDataProvider>());
        }

        /// <summary>
        /// Rydder opp etter hver test.
        /// </summary>
        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        /// <summary>
        /// Hjelpemetode for å sette opp brukeridentitet på controlleren.
        /// </summary>
        private void SetupUser(string userId, string role, string fullName = "Test User")
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim("Role", role),
                new Claim("FullName", fullName)
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
            
            // Oppdater TempData for ny HttpContext
            _controller.TempData = new TempDataDictionary(
                _controller.ControllerContext.HttpContext, 
                Mock.Of<ITempDataProvider>());
        }

        // ============================================================
        // TESTER FOR LIST ACTION
        // ============================================================

        /// <summary>
        /// Test: List-action skal returnere ViewResult.
        /// </summary>
        [Fact]
        public void List_ShouldReturnViewResult()
        {
            // Act
            var result = _controller.List();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        /// <summary>
        /// Test: List-action skal returnere alle hindringer.
        /// </summary>
        [Fact]
        public void List_ShouldReturnAllObstacles()
        {
            // Arrange
            _context.Obstacles.AddRange(
                new Obstacle { ReporterName = "Pilot1", Organization = "Org1", ObstacleType = "Crane", Status = "Pending" },
                new Obstacle { ReporterName = "Pilot2", Organization = "Org2", ObstacleType = "Building", Status = "Approved" },
                new Obstacle { ReporterName = "Pilot3", Organization = "Org3", ObstacleType = "Power line", Status = "Rejected" }
            );
            _context.SaveChanges();

            // Act
            var result = _controller.List() as ViewResult;
            var model = result?.Model as List<Obstacle>;

            // Assert
            Assert.NotNull(model);
            Assert.Equal(3, model.Count);
        }

        /// <summary>
        /// Test: List-action skal filtrere etter status.
        /// </summary>
        [Fact]
        public void List_WithStatusFilter_ShouldReturnFilteredObstacles()
        {
            // Arrange
            _context.Obstacles.AddRange(
                new Obstacle { ReporterName = "Pilot1", Organization = "Org1", ObstacleType = "Crane", Status = "Pending" },
                new Obstacle { ReporterName = "Pilot2", Organization = "Org2", ObstacleType = "Building", Status = "Approved" },
                new Obstacle { ReporterName = "Pilot3", Organization = "Org3", ObstacleType = "Power line", Status = "Approved" }
            );
            _context.SaveChanges();

            // Act
            var result = _controller.List(statusFilter: "Approved") as ViewResult;
            var model = result?.Model as List<Obstacle>;

            // Assert
            Assert.NotNull(model);
            Assert.Equal(2, model.Count);
            Assert.All(model, o => Assert.Equal("Approved", o.Status));
        }

        /// <summary>
        /// Test: List-action skal sette riktige ViewBag-statistikker.
        /// </summary>
        [Fact]
        public void List_ShouldSetCorrectViewBagStatistics()
        {
            // Arrange
            _context.Obstacles.AddRange(
                new Obstacle { ReporterName = "P1", Organization = "O1", ObstacleType = "Crane", Status = "Pending" },
                new Obstacle { ReporterName = "P2", Organization = "O2", ObstacleType = "Crane", Status = "Pending" },
                new Obstacle { ReporterName = "P3", Organization = "O3", ObstacleType = "Crane", Status = "Approved" }
            );
            _context.SaveChanges();

            // Act
            var result = _controller.List() as ViewResult;

            // Assert
            Assert.Equal(3, _controller.ViewBag.TotalCount);
            Assert.Equal(2, _controller.ViewBag.PendingCount);
            Assert.Equal(1, _controller.ViewBag.ApprovedCount);
            Assert.Equal(0, _controller.ViewBag.RejectedCount);
        }

        // ============================================================
        // TESTER FOR MYREPORTS ACTION
        // ============================================================

        /// <summary>
        /// Test: MyReports skal kun returnere innlogget brukers rapporter.
        /// </summary>
        [Fact]
        public void MyReports_ShouldReturnOnlyCurrentUserObstacles()
        {
            // Arrange
            var userId = "user123";
            SetupUser(userId, "Pilot");

            _context.Obstacles.AddRange(
                new Obstacle { ReporterName = "Pilot1", Organization = "O1", ObstacleType = "Crane", Status = "Pending", UserId = userId },
                new Obstacle { ReporterName = "Pilot2", Organization = "O2", ObstacleType = "Building", Status = "Approved", UserId = userId },
                new Obstacle { ReporterName = "Other", Organization = "O3", ObstacleType = "Power line", Status = "Pending", UserId = "other_user" }
            );
            _context.SaveChanges();

            // Act
            var result = _controller.MyReports() as ViewResult;
            var model = result?.Model as List<Obstacle>;

            // Assert
            Assert.NotNull(model);
            Assert.Equal(2, model.Count);
            Assert.All(model, o => Assert.Equal(userId, o.UserId));
        }

        // ============================================================
        // TESTER FOR DASHBOARD ACTION
        // ============================================================

        /// <summary>
        /// Test: Dashboard skal være tilgjengelig for Registerfører.
        /// </summary>
        [Fact]
        public void Dashboard_AsRegisterforer_ShouldReturnViewResult()
        {
            // Arrange
            SetupUser("admin1", "Registerfører", "Admin User");

            // Act
            var result = _controller.Dashboard();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        /// <summary>
        /// Test: Dashboard skal avvise tilgang for vanlig Pilot.
        /// </summary>
        [Fact]
        public void Dashboard_AsPilot_ShouldRedirectToHome()
        {
            // Arrange
            SetupUser("pilot1", "Pilot", "Pilot User");

            // Act
            var result = _controller.Dashboard();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Home", redirectResult.ControllerName);
        }

        // ============================================================
        // TESTER FOR APPROVE ACTION
        // ============================================================

        /// <summary>
        /// Test: Approve skal endre status til "Approved".
        /// </summary>
        [Fact]
        public void Approve_AsRegisterforer_ShouldSetStatusToApproved()
        {
            // Arrange
            SetupUser("admin1", "Registerfører", "Admin User");
            var obstacle = new Obstacle
            {
                ReporterName = "Pilot1",
                Organization = "Org1",
                ObstacleType = "Crane",
                Status = "Pending"
            };
            _context.Obstacles.Add(obstacle);
            _context.SaveChanges();

            // Act
            _controller.Approve(obstacle.Id, "Looks good!");

            // Assert
            var updated = _context.Obstacles.Find(obstacle.Id);
            Assert.NotNull(updated);
            Assert.Equal("Approved", updated.Status);
            Assert.Equal("Looks good!", updated.Feedback);
            Assert.Equal("Admin User", updated.ProcessedBy);
            Assert.NotNull(updated.ProcessedAt);
        }

        /// <summary>
        /// Test: Approve som Pilot skal avvises.
        /// </summary>
        [Fact]
        public void Approve_AsPilot_ShouldBeRejected()
        {
            // Arrange
            SetupUser("pilot1", "Pilot", "Pilot User");
            var obstacle = new Obstacle
            {
                ReporterName = "Pilot1",
                Organization = "Org1",
                ObstacleType = "Crane",
                Status = "Pending"
            };
            _context.Obstacles.Add(obstacle);
            _context.SaveChanges();

            // Act
            var result = _controller.Approve(obstacle.Id, null);

            // Assert
            var updated = _context.Obstacles.Find(obstacle.Id);
            Assert.Equal("Pending", updated?.Status); // Status skal ikke endres
            Assert.IsType<RedirectToActionResult>(result);
        }

        // ============================================================
        // TESTER FOR DELETE ACTION
        // ============================================================

        /// <summary>
        /// Test: Delete skal fjerne hindring fra databasen.
        /// </summary>
        [Fact]
        public void Delete_ByOwner_ShouldRemoveObstacle()
        {
            // Arrange
            var userId = "pilot1";
            SetupUser(userId, "Pilot");
            var obstacle = new Obstacle
            {
                ReporterName = "Pilot1",
                Organization = "Org1",
                ObstacleType = "Crane",
                Status = "Pending",
                UserId = userId
            };
            _context.Obstacles.Add(obstacle);
            _context.SaveChanges();
            var obstacleId = obstacle.Id;

            // Act
            _controller.Delete(obstacleId);

            // Assert
            var deleted = _context.Obstacles.Find(obstacleId);
            Assert.Null(deleted);
        }

        /// <summary>
        /// Test: Delete av godkjent rapport som Pilot skal avvises.
        /// </summary>
        [Fact]
        public void Delete_ApprovedObstacle_AsPilot_ShouldBeRejected()
        {
            // Arrange
            var userId = "pilot1";
            SetupUser(userId, "Pilot");
            var obstacle = new Obstacle
            {
                ReporterName = "Pilot1",
                Organization = "Org1",
                ObstacleType = "Crane",
                Status = "Approved", // Godkjent rapport
                UserId = userId
            };
            _context.Obstacles.Add(obstacle);
            _context.SaveChanges();

            // Act
            _controller.Delete(obstacle.Id);

            // Assert
            var stillExists = _context.Obstacles.Find(obstacle.Id);
            Assert.NotNull(stillExists); // Skal fortsatt eksistere
        }

        /// <summary>
        /// Test: Delete av andres rapport skal avvises for Pilot.
        /// </summary>
        [Fact]
        public void Delete_OtherUsersObstacle_AsPilot_ShouldBeRejected()
        {
            // Arrange
            SetupUser("pilot1", "Pilot");
            var obstacle = new Obstacle
            {
                ReporterName = "Other Pilot",
                Organization = "Org1",
                ObstacleType = "Crane",
                Status = "Pending",
                UserId = "other_user" // Annen bruker
            };
            _context.Obstacles.Add(obstacle);
            _context.SaveChanges();

            // Act
            _controller.Delete(obstacle.Id);

            // Assert
            var stillExists = _context.Obstacles.Find(obstacle.Id);
            Assert.NotNull(stillExists); // Skal fortsatt eksistere
        }

        // ============================================================
        // TESTER FOR EDIT ACTION
        // ============================================================

        /// <summary>
        /// Test: Edit av avvist rapport av eier skal sette status til Pending.
        /// </summary>
        [Fact]
        public void Edit_RejectedObstacle_ByOwner_ShouldSetStatusToPending()
        {
            // Arrange
            var userId = "pilot1";
            SetupUser(userId, "Pilot");
            var obstacle = new Obstacle
            {
                ReporterName = "Pilot1",
                Organization = "Org1",
                ObstacleType = "Crane",
                Status = "Rejected",
                UserId = userId,
                Feedback = "Please fix location"
            };
            _context.Obstacles.Add(obstacle);
            _context.SaveChanges();

            var editModel = new ObstacleData
            {
                ObstacleType = "Crane - Updated",
                Comment = "Fixed location",
                Latitude = 58.5,
                Longitude = 8.0
            };

            // Act
            _controller.Edit(obstacle.Id, editModel);

            // Assert
            var updated = _context.Obstacles.Find(obstacle.Id);
            Assert.NotNull(updated);
            Assert.Equal("Pending", updated.Status); // Status skal være Pending igjen
            Assert.Equal("Crane - Updated", updated.ObstacleType);
            Assert.Null(updated.Feedback); // Feedback skal være nullstilt
        }
    }
}
