using Microsoft.EntityFrameworkCore;
using HiddenValley.API.Models;

namespace HiddenValley.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Persona> Personas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Persona>(entity =>
            {
                entity.HasIndex(p => p.DPI).IsUnique();
                entity.HasIndex(p => p.Gmail).IsUnique();
            });
        }
    }
}
