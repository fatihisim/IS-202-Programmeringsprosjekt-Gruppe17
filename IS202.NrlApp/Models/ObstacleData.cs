using System.ComponentModel.DataAnnotations;

namespace IS202.NrlApp.Models
{
    public class ObstacleData
    {
        [Required, StringLength(80)]
        public string ReporterName { get; set; } = "";

        [Required, StringLength(80)]
        public string Organization { get; set; } = "";

        [Required, StringLength(120)]
        public string ObstacleType { get; set; } = ""; // örn: Mast, Power line, Crane

        [StringLength(300)]
        public string? Comment { get; set; }

        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
        public double? Latitude { get; set; }

        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
        public double? Longitude { get; set; }
    }
}
