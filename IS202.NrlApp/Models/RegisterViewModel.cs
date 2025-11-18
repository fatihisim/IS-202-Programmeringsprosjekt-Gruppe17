using System.ComponentModel.DataAnnotations;

namespace IS202.NrlApp.Models
{
    // Modell som brukes når en ny bruker registrerer seg i systemet.
    public class RegisterViewModel
    {
        // Fullt navn til brukeren (påkrevd)
        [Required(ErrorMessage = "Full name is required")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        // Brukerens e-postadresse (påkrevd og må være gyldig).
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        // Passord for kontoen. Feltet skjules i skjemaet.
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        // Må samsvare med passordet brukeren skrev inn.
        [Required(ErrorMessage = "Confirm password is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        // Valgfritt telefonnummer.
        [Phone(ErrorMessage = "Invalid phone number")]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        // Rollen brukeren skal få i systemet (påkrevd.)
        [Required(ErrorMessage = "Role is required")]
        [Display(Name = "User Role")]
        public string Role { get; set; }

        // Valgfri organisasjon brukeren er tilknyttet
        [Display(Name = "Organization (optional)")]
        public string? Organization { get; set; }
    }
}
