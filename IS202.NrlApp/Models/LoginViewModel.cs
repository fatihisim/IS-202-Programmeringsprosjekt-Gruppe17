using System.ComponentModel.DataAnnotations;

namespace IS202.NrlApp.Models
{
    // Modell som brukes for innloggingsskjemaet i applikasjonen
    public class LoginViewModel
    {
        // Brukernes e-postadresse. Feltet er påkrevd of må være i riktig format.
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        // Brukerens passord. Feltet er påkrevd og skjules i inputfeltet.
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        // Valg for om brukeren skal forbli innlogget etter at nettleseren lukkes.
        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}
