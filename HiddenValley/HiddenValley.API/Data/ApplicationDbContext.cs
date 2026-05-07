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

        // Módulo Cabañas (PROYECT-61)
        public DbSet<Cabana> Cabanas { get; set; }
        public DbSet<TipoCabana> TiposCabana { get; set; }
        public DbSet<EstadoCabana> EstadosCabana { get; set; }
        public DbSet<BitacoraEstados> BitacoraEstados { get; set; }

        // Módulo Personas (PROYECT-64)
        public DbSet<Persona> Personas { get; set; }

        // Módulo Clientes
        public DbSet<Cliente> Clientes { get; set; }

        // Módulo Reservaciones (PROYECT-60)
        public DbSet<RegistroReservacion> RegistroReservacion { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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

            // ===== Módulo Personas (PROYECT-64) =====
            modelBuilder.Entity<Persona>().ToTable("persona");
            modelBuilder.Entity<Persona>(entity =>
            {
                entity.HasIndex(p => p.DPI).IsUnique();
                entity.HasIndex(p => p.Gmail).IsUnique();
            });

            // ===== Módulo Clientes =====
            modelBuilder.Entity<Cliente>().ToTable("cliente");
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.Property(e => e.IdCliente).HasColumnName("idcliente");
                entity.Property(e => e.IdPersona).HasColumnName("idpersona");
                entity.HasIndex(c => c.IdPersona).IsUnique();
            });

            modelBuilder.Entity<Cliente>()
                .HasOne(c => c.Persona)
                .WithMany()
                .HasForeignKey(c => c.IdPersona);

            // ===== Módulo Reservaciones (PROYECT-60) =====
            modelBuilder.Entity<RegistroReservacion>().ToTable("registroreservacion");
            modelBuilder.Entity<RegistroReservacion>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.FechaEntrada).HasColumnName("fechaentrada").HasColumnType("date");
                entity.Property(e => e.FechaSalida).HasColumnName("fechasalida").HasColumnType("date");
                entity.Property(e => e.CantidadPersonas).HasColumnName("cantidadpersonas");
                entity.Property(e => e.EstadoReserva).HasColumnName("estadoreserva");
                entity.Property(e => e.TotalPagar).HasColumnName("totalpagar");
                entity.Property(e => e.IdCliente).HasColumnName("idcliente");
                entity.Property(e => e.IdCabana).HasColumnName("idcabana");
                entity.Property(e => e.IdEmpleado).HasColumnName("idempleado");
            });

            modelBuilder.Entity<RegistroReservacion>()
                .HasOne(r => r.Cliente)
                .WithMany(c => c.Reservaciones)
                .HasForeignKey(r => r.IdCliente);

            modelBuilder.Entity<RegistroReservacion>()
                .HasOne(r => r.Cabana)
                .WithMany(c => c.Reservaciones)
                .HasForeignKey(r => r.IdCabana);
        }
    }
}
