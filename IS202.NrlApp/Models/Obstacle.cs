using System;
using System.ComponentModel.DataAnnotations;

namespace IS202.NrlApp.Models
{
    /// <summary>
    /// Datamodell som representerer en hindring lagret i databasen.
    /// </summary>
    public class Obstacle
    {
        /// <summary>
        /// Unik ID for hver hindring (primærnøkkel i databasen).
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Navn på rapportøren.
        /// </summary>
        [Required, StringLength(80)]
        public string ReporterName { get; set; } = "";

        /// <summary>
        /// Organisasjonen rapportøren tilhører.
        /// </summary>
        [Required, StringLength(80)]
        public string Organization { get; set; } = "";

        /// <summary>
        /// Type hindring som rapporteres.
        /// </summary>
        [Required, StringLength(120)]
        public string ObstacleType { get; set; } = "";

        /// <summary>
        /// Eventuell kommentar relatert til hindringen.
        /// </summary>
        [StringLength(300)]
        public string? Comment { get; set; }

        /// <summary>
        /// Breddegrad (latitude).
        /// </summary>
        [Range(-90, 90)]
        public double? Latitude { get; set; }

        /// <summary>
        /// Lengdegrad (longitude).
        /// </summary>
        [Range(-180, 180)]
        public double? Longitude { get; set; }

        /// <summary>
        /// Geometri-type (Point, LineString, Polygon).
        /// </summary>
        [StringLength(50)]
        public string? GeometryType { get; set; }

        /// <summary>
        /// GeoJSON-representasjon av geometrien.
        /// </summary>
        public string? GeoJsonData { get; set; }

        /// <summary>
        /// ID til brukeren som opprettet rapporten.
        /// </summary>
        [StringLength(450)]
        public string? UserId { get; set; }

        /// <summary>
        /// Status på rapporten (Pending, Approved, Rejected).
        /// </summary>
        [Required, StringLength(50)]
        public string Status { get; set; } = "Pending";

        /// <summary>
        /// Navn på personen som behandlet rapporten (Registerfører).
        /// </summary>
        [StringLength(80)]
        public string? ProcessedBy { get; set; }

        /// <summary>
        /// Tidspunkt for når rapporten ble behandlet.
        /// </summary>
        public DateTime? ProcessedAt { get; set; }

        /// <summary>
        /// Tilbakemelding fra Registerfører (approval/rejection feedback).
        /// </summary>
        [StringLength(500)]
        public string? Feedback { get; set; }

        /// <summary>
        /// Tidspunktet når hindringen ble registrert (UTC).
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
