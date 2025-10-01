using IS202.NrlApp.Models;
using Microsoft.EntityFrameworkCore;

namespace IS202.NrlApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Obstacle> Obstacles => Set<Obstacle>();
    }
}
