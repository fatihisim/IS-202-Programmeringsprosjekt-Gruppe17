namespace IS202.NrlApp.Models
{
    /// <summary>
    /// Modell som brukes til å vise feilmeldinger i applikasjonen.
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// ID for gjeldende forespørsel (brukes til feilsøking).
        /// </summary>
        public string? RequestId { get; set; }

        /// <summary>
        /// Angir om RequestId skal vises på feilsiden.
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
