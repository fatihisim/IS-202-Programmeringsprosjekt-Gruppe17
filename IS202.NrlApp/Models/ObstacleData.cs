using System.ComponentModel.DataAnnotations;

namespace IS202.NrlApp.Models
{
    /// <summary>
    /// ViewModel for obstacle rapportering.
    /// ReporterName og Organization hentes automatisk fra innlogget bruker.
    /// </summary>
    public class ObstacleData
    {
        [Required(ErrorMessage = "Obstacle type is required")]
        [StringLength(120)]
        public string ObstacleType { get; set; } = "";

        [StringLength(300)]
        public string? Comment { get; set; }

        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
        public double? Latitude { get; set; }

        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
        public double? Longitude { get; set; }

        [Required]
        [StringLength(20)]
        public string GeometryType { get; set; } = "Point";

        public string? GeoJsonData { get; set; }
    }
}