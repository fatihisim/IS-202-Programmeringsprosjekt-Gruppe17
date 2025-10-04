using System.ComponentModel.DataAnnotations;

namespace IS202.NrlApp.Models
{
    /// <summary>
    /// ViewModel som representerer data som sendes fra skjemaet i brukergrensesnittet.
    /// Brukes til validering av innsendte verdier før lagring i databasen.
    /// </summary>
    public class ObstacleData
    {
        /// <summary>
        /// Navn på personen som rapporterer hindringen.
        /// </summary>
        [Required, StringLength(80)]
        public string ReporterName { get; set; } = "";

        /// <summary>
        /// Organisasjonen rapportøren tilhører (f.eks. NRL, Statnett, etc.).
        /// </summary>
        [Required, StringLength(80)]
        public string Organization { get; set; } = "";

        /// <summary>
        /// Type hindring (f.eks. Mast, Power line, Crane).
        /// </summary>
        [Required, StringLength(120)]
        public string ObstacleType { get; set; } = "";

        /// <summary>
        /// Valgfritt felt for ekstra kommentarer om hindringen.
        /// </summary>
        [StringLength(300)]
        public string? Comment { get; set; }

        /// <summary>
        /// Breddegrad (latitude) der hindringen befinner seg.
        /// </summary>
        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
        public double? Latitude { get; set; }

        /// <summary>
        /// Lengdegrad (longitude) der hindringen befinner seg.
        /// </summary>
        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
        public double? Longitude { get; set; }
    }
}
