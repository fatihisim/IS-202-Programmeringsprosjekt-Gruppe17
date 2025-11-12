using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using IS202.NrlApp.Models;

namespace IS202.NrlApp.Data
{
    // AppDbContext utvider IdentityDbContext for å inkludere brukere, roller og autentisering.
    public class AppDbContext : IdentityDbContext
    {
        // Konstruktør som sender opsjoner til basisklassen (IdentityDbContext)
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // DbSet for applikasjonens domene-modeller (for eksempel hinder på kartet)
        public DbSet<Obstacle> Obstacles { get; set; }

        // DbSet for registrerte brukere i systemet
        public DbSet<User> Users { get; set; }

        // Metode for å konfigurere databasen og relasjonene
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Her kan man legge til spesielle konfigurasjoner eller standarddata senere.
        }
    }
}
