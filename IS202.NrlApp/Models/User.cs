using System;
using System.ComponentModel.DataAnnotations;

namespace IS202.NrlApp.Models
{
    // Representerer en registrert bruker i systemet
    public class User
    {
        [Key]
        public int Id { get; set; } // Unik identifikator for hver bruker

        [Required(ErrorMessage = "Full name is required")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Organization")]
        public string? Organization { get; set; } // Valgfritt felt for brukers organisasjon

        [Display(Name = "Phone Number")]
        [Phone]
        public string? PhoneNumber { get; set; } // Valgfritt felt for kontaktinformasjon

        [Display(Name = "Role")]
        public string Role { get; set; } = "Pilot"; // Standardrolle ved registrering

        public DateTime RegisteredAt { get; set; } = DateTime.Now; // Tidspunkt for registrering
    }
}
