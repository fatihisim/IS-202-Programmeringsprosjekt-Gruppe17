using System;
using System.ComponentModel.DataAnnotations;

namespace IS202.NrlApp.Models
{
    /// Modell som representerer en rapportert hindring som lagres i databasen.
    public class Obstacle
    {
        /// Unik ID for hindringen (primærnøkkel).
        public int Id { get; set; }

        /// Navn på personen som rapporterer hindringen.
        [Required, StringLength(80)]
        public string ReporterName { get; set; } = "";

        /// Organisasjonen rapportøren tilhører.
        [Required, StringLength(80)]
        public string Organization { get; set; } = "";

        /// Type hindring som rapporteres. (f.eks. mast, bygg)
        [Required, StringLength(120)]
        public string ObstacleType { get; set; } = "";

        /// Valgfri kommentar for ekstra informasjon.
        [StringLength(300)]
        public string? Comment { get; set; }

        /// Breddegrad (latitude) for hindringens plassering.
        [Range(-90, 90)]
        public double? Latitude { get; set; }

        /// Lengdegrad (longitude) for hindringens plassering.
        [Range(-180, 180)]
        public double? Longitude { get; set; }

        /// Geometri-type (f.eks. Point, LineString, Polygon).
        [StringLength(50)]
        public string? GeometryType { get; set; }

        /// GeoJSON-data som beskriver hindringens geometri.
        public string? GeoJsonData { get; set; }

        /// ID til brukeren som sendte inn rapporten.
        [StringLength(450)]
        public string? UserId { get; set; }

        /// Gjeldende status for rpporten (Pending, Approved, Rejected).
        [Required, StringLength(50)]
        public string Status { get; set; } = "Pending";

        /// Hvem som behandlet rapporten (registerfører).
        [StringLength(80)]
        public string? ProcessedBy { get; set; }

        /// Når rapporten ble behandlet.
        public DateTime? ProcessedAt { get; set; }

        /// Tilbakemelding fra registerfører (ved godkjenning/avslag).
        [StringLength(500)]
        public string? Feedback { get; set; }

        /// Når hindringen ble registrert (UTC).
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
