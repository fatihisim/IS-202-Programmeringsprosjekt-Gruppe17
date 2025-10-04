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
        /// Tidspunktet når hindringen ble registrert (UTC).
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
