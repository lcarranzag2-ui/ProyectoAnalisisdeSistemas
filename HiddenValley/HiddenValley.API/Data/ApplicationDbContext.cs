using Microsoft.EntityFrameworkCore;
using HiddenValley.API.Models;

namespace HiddenValley.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Persona> Personas { get; set; }
    public DbSet<PuestoTrabajo> PuestosTrabajo { get; set; }
    public DbSet<Empleado> Empleados { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Empleado>()
            .HasOne(e => e.Persona)
            .WithMany()
            .HasForeignKey(e => e.IdPersona)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Empleado>()
            .HasOne(e => e.PuestoTrabajo)
            .WithMany()
            .HasForeignKey(e => e.IdPuestoTrabajo)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Empleado>()
            .HasIndex(e => e.IdPersona)
            .IsUnique();
    }
}