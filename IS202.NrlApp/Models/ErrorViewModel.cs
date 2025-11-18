namespace IS202.NrlApp.Models
{
    /// Modell som brukes for å vise feilmeldinger som oppstår i applikasjonen.
    public class ErrorViewModel
    {
        /// Unik ID for aktuelle forespørselen. Nyttig når man skal feilsøke.
        public string? RequestId { get; set; }

        /// Returnerer true hvis RequestId  finnes, slik at den kan vises på feilsiden.
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
