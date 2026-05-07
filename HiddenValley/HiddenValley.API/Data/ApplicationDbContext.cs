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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //aqui pueden mapear las tablas y configurar llaves compuestas, nombres de tablas y relaciones
        }
    }
}