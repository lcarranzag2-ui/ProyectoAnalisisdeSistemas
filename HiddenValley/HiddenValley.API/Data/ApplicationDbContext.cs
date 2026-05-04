using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using HiddenValley.API.Models;
using System.Security.Cryptography.X509Certificates;
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
         public DbSet<Servicio> Servicio => Set<Servicio>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

           modelBuilder.Entity<Servicio>().ToTable("servicio");
        }
    }
}