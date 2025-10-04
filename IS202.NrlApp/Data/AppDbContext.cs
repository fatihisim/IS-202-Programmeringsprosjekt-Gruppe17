using IS202.NrlApp.Models;
using Microsoft.EntityFrameworkCore;

namespace IS202.NrlApp.Data
{
    /// <summary>
    /// Representerer applikasjonens databasekontekst.
    /// Håndterer lagring og henting av data via Entity Framework Core.
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Initialiserer databasekonteksten med konfigurerte alternativer.
        /// </summary>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        /// <summary>
        /// Tabell (DbSet) som lagrer alle hindringsdata.
        /// </summary>
        public DbSet<Obstacle> Obstacles => Set<Obstacle>();
    }
}
