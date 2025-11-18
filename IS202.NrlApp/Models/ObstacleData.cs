using System.ComponentModel.DataAnnotations;

namespace IS202.NrlApp.Models
{
    /// Modell som brukes når en bruker sender inn en ny hindringsrapport.
    /// ReporterName og Organization fylles inn automatisk fra den innloggede brukeren.
    public class ObstacleData
    {
        // Type hindring som rapporteres (påkrevd)
        [Required(ErrorMessage = "Obstacle type is required")]
        [StringLength(120)]
        public string ObstacleType { get; set; } = "";

        // Valgfri kommentar om hindringen
        [StringLength(300)]
        public string? Comment { get; set; }

        // Breddegrad for hindringens plassering
        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
        public double? Latitude { get; set; }

        // Lengdegrad for hindringens plassering
        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
        public double? Longitude { get; set; }

        // Geometritype (f.eks. Point, Linestring, Polygon). Standard: Point.
        [Required]
        [StringLength(20)]
        public string GeometryType { get; set; } = "Point";

        // GeoJSON-data som beskriver hindringens form
        public string? GeoJsonData { get; set; }
    }
}