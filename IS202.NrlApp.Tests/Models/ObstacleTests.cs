using System.ComponentModel.DataAnnotations;
using IS202.NrlApp.Models;
using Xunit;

namespace IS202.NrlApp.Tests.Models
{
    /// <summary>
    /// Enhetstester for Obstacle-modellen.
    /// Tester validering av dataattributter og standardverdier.
    /// </summary>
    public class ObstacleTests
    {
        /// <summary>
        /// Hjelpemetode for å validere en modell og returnere valideringsfeil.
        /// </summary>
        private static List<ValidationResult> ValidateModel(object model)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, context, results, true);
            return results;
        }

        // ============================================================
        // TESTER FOR STANDARDVERDIER
        // ============================================================

        /// <summary>
        /// Test: Ny Obstacle skal ha Status = "Pending" som standard.
        /// </summary>
        [Fact]
        public void NewObstacle_ShouldHaveDefaultStatus_Pending()
        {
            // Arrange & Act
            var obstacle = new Obstacle();

            // Assert
            Assert.Equal("Pending", obstacle.Status);
        }

        /// <summary>
        /// Test: Ny Obstacle skal ha CreatedAt satt til nåværende tidspunkt.
        /// </summary>
        [Fact]
        public void NewObstacle_ShouldHaveCreatedAt_SetToCurrentTime()
        {
            // Arrange
            var before = DateTime.UtcNow.AddSeconds(-1);

            // Act
            var obstacle = new Obstacle();

            // Assert
            var after = DateTime.UtcNow.AddSeconds(1);
            Assert.InRange(obstacle.CreatedAt, before, after);
        }

        /// <summary>
        /// Test: Ny Obstacle skal ha tomme strenger for ReporterName og Organization.
        /// </summary>
        [Fact]
        public void NewObstacle_ShouldHaveEmptyStrings_ForRequiredFields()
        {
            // Arrange & Act
            var obstacle = new Obstacle();

            // Assert
            Assert.Equal("", obstacle.ReporterName);
            Assert.Equal("", obstacle.Organization);
            Assert.Equal("", obstacle.ObstacleType);
        }

        // ============================================================
        // TESTER FOR VALIDERING - REQUIRED FELTER
        // ============================================================

        /// <summary>
        /// Test: Obstacle med alle påkrevde felter skal være gyldig.
        /// </summary>
        [Fact]
        public void Obstacle_WithAllRequiredFields_ShouldBeValid()
        {
            // Arrange
            var obstacle = new Obstacle
            {
                ReporterName = "Test Pilot",
                Organization = "Test Organization",
                ObstacleType = "Power line",
                Status = "Pending"
            };

            // Act
            var results = ValidateModel(obstacle);

            // Assert
            Assert.Empty(results);
        }

        /// <summary>
        /// Test: Obstacle uten ReporterName skal feile validering.
        /// </summary>
        [Fact]
        public void Obstacle_WithoutReporterName_ShouldFailValidation()
        {
            // Arrange
            var obstacle = new Obstacle
            {
                ReporterName = null!, // Påkrevd felt satt til null
                Organization = "Test Organization",
                ObstacleType = "Power line"
            };

            // Act
            var results = ValidateModel(obstacle);

            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("ReporterName"));
        }

        /// <summary>
        /// Test: Obstacle uten ObstacleType skal feile validering.
        /// </summary>
        [Fact]
        public void Obstacle_WithoutObstacleType_ShouldFailValidation()
        {
            // Arrange
            var obstacle = new Obstacle
            {
                ReporterName = "Test Pilot",
                Organization = "Test Organization",
                ObstacleType = null! // Påkrevd felt satt til null
            };

            // Act
            var results = ValidateModel(obstacle);

            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("ObstacleType"));
        }

        // ============================================================
        // TESTER FOR VALIDERING - STRING LENGTH
        // ============================================================

        /// <summary>
        /// Test: ReporterName over 80 tegn skal feile validering.
        /// </summary>
        [Fact]
        public void Obstacle_ReporterNameTooLong_ShouldFailValidation()
        {
            // Arrange
            var obstacle = new Obstacle
            {
                ReporterName = new string('A', 81), // 81 tegn, maks er 80
                Organization = "Test Organization",
                ObstacleType = "Power line"
            };

            // Act
            var results = ValidateModel(obstacle);

            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("ReporterName"));
        }

        /// <summary>
        /// Test: Comment over 300 tegn skal feile validering.
        /// </summary>
        [Fact]
        public void Obstacle_CommentTooLong_ShouldFailValidation()
        {
            // Arrange
            var obstacle = new Obstacle
            {
                ReporterName = "Test Pilot",
                Organization = "Test Organization",
                ObstacleType = "Power line",
                Comment = new string('B', 301) // 301 tegn, maks er 300
            };

            // Act
            var results = ValidateModel(obstacle);

            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("Comment"));
        }

        // ============================================================
        // TESTER FOR VALIDERING - RANGE (KOORDINATER)
        // ============================================================

        /// <summary>
        /// Test: Latitude innenfor gyldig område (-90 til 90) skal være gyldig.
        /// </summary>
        [Theory]
        [InlineData(-90)]
        [InlineData(0)]
        [InlineData(58.146)] // Kristiansand
        [InlineData(90)]
        public void Obstacle_ValidLatitude_ShouldBeValid(double latitude)
        {
            // Arrange
            var obstacle = new Obstacle
            {
                ReporterName = "Test Pilot",
                Organization = "Test Organization",
                ObstacleType = "Power line",
                Latitude = latitude
            };

            // Act
            var results = ValidateModel(obstacle);

            // Assert
            Assert.DoesNotContain(results, r => r.MemberNames.Contains("Latitude"));
        }

        /// <summary>
        /// Test: Latitude utenfor gyldig område skal feile validering.
        /// </summary>
        [Theory]
        [InlineData(-91)]
        [InlineData(91)]
        [InlineData(180)]
        public void Obstacle_InvalidLatitude_ShouldFailValidation(double latitude)
        {
            // Arrange
            var obstacle = new Obstacle
            {
                ReporterName = "Test Pilot",
                Organization = "Test Organization",
                ObstacleType = "Power line",
                Latitude = latitude
            };

            // Act
            var results = ValidateModel(obstacle);

            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("Latitude"));
        }

        /// <summary>
        /// Test: Longitude utenfor gyldig område (-180 til 180) skal feile validering.
        /// </summary>
        [Theory]
        [InlineData(-181)]
        [InlineData(181)]
        public void Obstacle_InvalidLongitude_ShouldFailValidation(double longitude)
        {
            // Arrange
            var obstacle = new Obstacle
            {
                ReporterName = "Test Pilot",
                Organization = "Test Organization",
                ObstacleType = "Power line",
                Longitude = longitude
            };

            // Act
            var results = ValidateModel(obstacle);

            // Assert
            Assert.Contains(results, r => r.MemberNames.Contains("Longitude"));
        }

        // ============================================================
        // TESTER FOR STATUS VERDIER
        // ============================================================

        /// <summary>
        /// Test: Status skal kunne settes til alle gyldige verdier.
        /// </summary>
        [Theory]
        [InlineData("Pending")]
        [InlineData("Approved")]
        [InlineData("Rejected")]
        public void Obstacle_ValidStatus_ShouldBeAccepted(string status)
        {
            // Arrange
            var obstacle = new Obstacle
            {
                ReporterName = "Test Pilot",
                Organization = "Test Organization",
                ObstacleType = "Power line",
                Status = status
            };

            // Act
            var results = ValidateModel(obstacle);

            // Assert
            Assert.Empty(results);
        }

        // ============================================================
        // TESTER FOR GEOJSON DATA
        // ============================================================

        /// <summary>
        /// Test: GeoJsonData skal kunne lagre gyldig GeoJSON.
        /// </summary>
        [Fact]
        public void Obstacle_WithValidGeoJson_ShouldBeValid()
        {
            // Arrange
            var geoJson = @"{""type"":""Point"",""coordinates"":[7.995,58.146]}";
            var obstacle = new Obstacle
            {
                ReporterName = "Test Pilot",
                Organization = "Test Organization",
                ObstacleType = "Power line",
                GeometryType = "Point",
                GeoJsonData = geoJson
            };

            // Act
            var results = ValidateModel(obstacle);

            // Assert
            Assert.Empty(results);
            Assert.Equal(geoJson, obstacle.GeoJsonData);
        }
    }
}
