using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using HiddenValley.API.Models;
namespace HiddenValley.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) 
        { 

        }

        //definicion de las tablas en base a los modelos
        //ejemplo public DbSet<Persona> Personas { get; set; } 
        public DbSet<Cabana> Cabanas { get; set; }
        public DbSet<TipoCabana> TiposCabana { get; set; }
        public DbSet<EstadoCabana> EstadosCabana { get; set; }
        public DbSet<BitacoraEstados> BitacoraEstados { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //aqui pueden mapear las tablas y configurar llaves compuestas, nombres de tablas y relaciones
            modelBuilder.Entity<Cabana>().ToTable("cabana");
            modelBuilder.Entity<TipoCabana>().ToTable("tipocabana");
            modelBuilder.Entity<EstadoCabana>().ToTable("estadocabana");
            modelBuilder.Entity<BitacoraEstados>().ToTable("bitacoraestados");

            modelBuilder.Entity<Cabana>(entity => {
                entity.Property(e => e.IdCabana).HasColumnName("idcabana");
                entity.Property(e => e.IdTipoCabana).HasColumnName("idtipocabana");
                entity.Property(e => e.IdEstadoCabana).HasColumnName("idestadocabana");
            });

            modelBuilder.Entity<TipoCabana>(entity => {
                entity.Property(e => e.IdTipoCabana).HasColumnName("idtipocabana");
                entity.Property(e => e.Nombre).HasColumnName("nombre");
                entity.Property(e => e.Descripcion).HasColumnName("descripcion");
                entity.Property(e => e.Capacidad).HasColumnName("capacidad");
                entity.Property(e => e.Precio).HasColumnName("precio");
            });

            modelBuilder.Entity<EstadoCabana>(entity => {
                entity.Property(e => e.IdEstadoCabana).HasColumnName("idestadocabana");
                entity.Property(e => e.Nombre).HasColumnName("nombre");
                entity.Property(e => e.Descripcion).HasColumnName("descripcion");
                entity.HasIndex(e => e.Nombre).IsUnique();
            });

            modelBuilder.Entity<BitacoraEstados>(entity => {
                entity.Property(e => e.IdBitacoraEstado).HasColumnName("idbitacoraestado");
                entity.Property(e => e.IdEstadoAnterior).HasColumnName("idestadoanterior");
                entity.Property(e => e.IdEstadoNuevo).HasColumnName("idestadonuevo");
                entity.Property(e => e.FechaCambio).HasColumnName("fechacambio");
                entity.Property(e => e.IdCabana).HasColumnName("idcabana");
                entity.Property(e => e.IdEmpleado).HasColumnName("idempleado");
            });

            modelBuilder.Entity<Cabana>()
                .HasOne(c => c.TipoCabana)
                .WithMany(t => t.Cabanas)
                .HasForeignKey(c => c.IdTipoCabana);

            modelBuilder.Entity<Cabana>()
                .HasOne(c => c.EstadoCabana)
                .WithMany(e => e.Cabanas)
                .HasForeignKey(c => c.IdEstadoCabana);

            modelBuilder.Entity<BitacoraEstados>()
                .HasOne(b => b.EstadoAnterior)
                .WithMany(e => e.BitacoraEstadosAnterior)
                .HasForeignKey(b => b.IdEstadoAnterior);

            modelBuilder.Entity<BitacoraEstados>()
                .HasOne(b => b.EstadoNuevo)
                .WithMany(e => e.BitacoraEstadosNuevo)
                .HasForeignKey(b => b.IdEstadoNuevo);

            modelBuilder.Entity<BitacoraEstados>()
                .HasOne(b => b.Cabana)
                .WithMany(c => c.BitacoraEstados)
                .HasForeignKey(b => b.IdCabana);
        }
        
    }
}