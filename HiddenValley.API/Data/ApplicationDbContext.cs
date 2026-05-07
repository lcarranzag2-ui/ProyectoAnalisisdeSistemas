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
        //ejemplo public DbSet<Persona> Personas { get; set;
        public DbSet<Persona> Persona { get; set; } 
        public DbSet<Cliente> Cliente { get; set; }
        public DbSet<RegistroReservacion> RegistroReservacion { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           base.OnModelCreating(modelBuilder);

            //aqui pueden mapear las tablas y configurar llaves compuestas, nombres de tablas y relaciones
            modelBuilder.Entity<Persona>().ToTable("persona");
            modelBuilder.Entity<Cliente>().ToTable("cliente");
            modelBuilder.Entity<RegistroReservacion>().ToTable("registroreservacion");
            modelBuilder.Entity<Persona>(entity => {
            entity.Property(e => e.IdPersona).HasColumnName("idpersona");
            entity.Property(e => e.Nombres).HasColumnName("nombres");
            entity.Property(e => e.Apellidos).HasColumnName("apellidos");
            entity.Property(e => e.DPI).HasColumnName("dpi");
            entity.Property(e => e.Telefono).HasColumnName("telefono");
            entity.Property(e => e.Gmail).HasColumnName("gmail");
            entity.Property(e => e.Direccion).HasColumnName("direccion"); 
        });

        modelBuilder.Entity<Cliente>(entity => {
            entity.Property(e => e.IdCliente).HasColumnName("idcliente");
            entity.Property(e => e.IdPersona).HasColumnName("idpersona");
        });
        modelBuilder.Entity<RegistroReservacion>(entity => {
        entity.ToTable("registroreservacion");
        entity.Property(e => e.Id).HasColumnName("id"); 
        entity.Property(e => e.FechaEntrada).HasColumnName("fechaentrada");
        entity.Property(e => e.FechaSalida).HasColumnName("fechasalida");
        entity.Property(e => e.EstadoReserva).HasColumnName("estadoreserva");
        entity.Property(e => e.TotalPagar).HasColumnName("totalpagar");
        entity.Property(e => e.IdCliente).HasColumnName("idcliente");
        entity.Property(e => e.IdCabana).HasColumnName("idcabana");
        entity.Property(e => e.IdEmpleado).HasColumnName("idempleado");
        entity.Property(e => e.CantidadPersonas).HasColumnName("cantidadpersonas");
         });

        }
    }
}