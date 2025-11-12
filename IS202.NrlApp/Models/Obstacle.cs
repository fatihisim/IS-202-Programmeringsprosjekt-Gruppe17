using System;
using System.ComponentModel.DataAnnotations;

namespace IS202.NrlApp.Models
{
    /// <summary>
    /// Representerer en luftfartshindring i systemet.
    /// </summary>
    public class Obstacle
    {
        public int Id { get; set; }

        [Required, StringLength(80)]
        public string ReporterName { get; set; } = "";

        [Required, StringLength(80)]
        public string Organization { get; set; } = "";

        [Required, StringLength(120)]
        public string ObstacleType { get; set; } = "";

        [StringLength(300)]
        public string? Comment { get; set; }

        [Range(-90, 90)]
        public double? Latitude { get; set; }

        [Range(-180, 180)]
        public double? Longitude { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Status og behandling
        [Required, StringLength(20)]
        public string Status { get; set; } = "Pending";

        // Geometri
        [Required, StringLength(20)]
        public string GeometryType { get; set; } = "Point";

        public string? GeoJsonData { get; set; }

        // Bruker-tracking
        [StringLength(450)]
        public string? UserId { get; set; }

        [StringLength(450)]
        public string? ProcessedBy { get; set; }

        public DateTime? ProcessedAt { get; set; }

        [StringLength(500)]
        public string? ProcessorComment { get; set; }
    }
}