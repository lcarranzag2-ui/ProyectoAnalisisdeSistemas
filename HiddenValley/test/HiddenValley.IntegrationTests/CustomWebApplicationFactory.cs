using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using HiddenValley.API.Data; 

namespace HiddenValley.IntegrationTests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // 1. Remover TODA la configuración previa del DbContext (Opciones y el Contexto mismo)
            var dbContextOptionsDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (dbContextOptionsDescriptor != null) services.Remove(dbContextOptionsDescriptor);

            var genericOptionsDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions));
            if (genericOptionsDescriptor != null) services.Remove(genericOptionsDescriptor);

            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(ApplicationDbContext));
            if (dbContextDescriptor != null) services.Remove(dbContextDescriptor);


            // 2. Crear un proveedor de servicios interno aislado EXCLUSIVO para InMemory.
            // Esto evita por completo que las extensiones de PostgreSQL salpiquen tus pruebas.
            var internalServiceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            // 3. Registrar el contexto obligándolo a usar el proveedor aislado en memoria
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("HiddenValleyIntegrationTestDb")
                       .UseInternalServiceProvider(internalServiceProvider); // Fuerza el aislamiento de EF Core
            });
        });
    }
}