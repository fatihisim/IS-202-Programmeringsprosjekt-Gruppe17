using System;
using System.ComponentModel.DataAnnotations;

namespace IS202.NrlApp.Models
{
    // Representerer en registrert bruker i systemet
    public class User
    {
        // Unik ID for hver bruker (primærnøkkel).
        [Key]
        public int Id { get; set; } 

        // Brukerens fulle navn (påkrevd).
        [Required(ErrorMessage = "Full name is required")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

          // Brukerens e-postadresse (påkrevd og må være gyldig).
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

         // Passord brukt ved innlogging.
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        // Valgfri organisasjon brukeren er tilknyttet.
        [Display(Name = "Organization")]
        public string? Organization { get; set; } 

        // Valgfritt telefonnummer.
        [Display(Name = "Phone Number")]
        [Phone]
        public string? PhoneNumber { get; set; } 

        // Brukerens rolle i systemet. Standard er Pilot.
        [Display(Name = "Role")]
        public string Role { get; set; } = "Pilot"; 

        // Når brukeren ble registrert.
        public DateTime RegisteredAt { get; set; } = DateTime.Now;
    }
}
