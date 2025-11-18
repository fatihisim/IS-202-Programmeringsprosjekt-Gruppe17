using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using IS202.NrlApp.Models;

namespace IS202.NrlApp.Data
{
    // AppDbContext utvider IdentityDbContext for å inkludere brukere, roller og autentisering.
    public class AppDbContext : IdentityDbContext
    {
        // Konstruktør som sender databasedigutasjonen videre til IdentityDbContext
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // Tabell for lagring av hindere (f.eks. master, bygg, objekter på kartet)
        public DbSet<Obstacle> Obstacles { get; set; }

        // Tabell for applikasjonens brukere (utvidet fra IdentityUser)
        public DbSet<User> Users { get; set; }

        // Metode for å konfigurere databasen og relasjonene
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Tilpasset konfigurasjon kan legges her dersom det trens
        }
    }
}
