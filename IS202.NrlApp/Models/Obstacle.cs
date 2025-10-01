using System;
using System.ComponentModel.DataAnnotations;

namespace IS202.NrlApp.Models
{
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
    }
}
