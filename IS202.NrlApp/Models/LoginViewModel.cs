using System.ComponentModel.DataAnnotations;

namespace IS202.NrlApp.Models
{
    // Denne klassen brukes til å samle inn data fra innloggingsskjemaet
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        // Husk meg-funksjon for å holde brukeren innlogget
        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}
