using Microsoft.EntityFrameworkCore;
using IS202.NrlApp.Models;

namespace IS202.NrlApp.Data
{
    // Denne klassen representerer databasen for applikasjonen
    // og brukes til å kommunisere med MariaDB gjennom Entity Framework Core.
    public class AppDbContext : DbContext
    {
        // Konstruktør som mottar databasekonfigurasjon fra appsettings.json
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Tabell (database-sett) for hinder (Obstacle)
        // Dette gjør at vi kan hente, legge til, endre og slette hinderdata
        public DbSet<Obstacle> Obstacles { get; set; }

        // Denne metoden brukes til å konfigurere hvordan databasen skal opprettes
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Eksempel: sørger for at ID alltid blir generert automatisk
            modelBuilder.Entity<Obstacle>()
                .Property(o => o.Id)
                .ValueGeneratedOnAdd();
        }
    }
}
