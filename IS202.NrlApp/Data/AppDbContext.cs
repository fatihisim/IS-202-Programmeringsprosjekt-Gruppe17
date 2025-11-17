using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using IS202.NrlApp.Models;

namespace IS202.NrlApp.Data
{
    /// <summary>
    /// AppDbContext utvider IdentityDbContext for å inkludere brukere, roller og autentisering.
    /// Inneholder også applikasjonens domenemodeller som Obstacles.
    /// </summary>
    public class AppDbContext : IdentityDbContext
    {
        /// <summary>
        /// Konstruktør som sender konfigurasjon til basisklassen (IdentityDbContext).
        /// </summary>
        /// <param name="options">Databasekonfigurasjon (tilkoblingsstreng, provider, osv.)</param>
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// DbSet for luftfartshindringer rapportert til NRL.
        /// Inneholder geometri, status, rapportør og behandlingsinformasjon.
        /// </summary>
        public DbSet<Obstacle> Obstacles { get; set; }

        /// <summary>
        /// Konfigurerer databaseskjema og relasjoner mellom entiteter.
        /// </summary>
        /// <param name="modelBuilder">EF Core model builder for schemakonfigurasjon</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Eventuelle tilleggskonfigurasjoner for entiteter kan legges til her.
            // For eksempel: indekser, standardverdier, eller spesielle datatyper.
        }
    }
}