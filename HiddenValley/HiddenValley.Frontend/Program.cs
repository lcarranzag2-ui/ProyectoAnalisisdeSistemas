using System;
 
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using HiddenValley.Frontend;
using HiddenValley.Frontend.Interfaces;
using HiddenValley.Frontend.Services;
using HiddenValley.Frontend.Service;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Blazor WebAssembly corre en el navegador, no dentro de Docker
// por eso hay que usar localhost con el puerto que docker-compose expone al exterior
// que segun el docker-compose.yml es el 7084 mapeado al 8080 interno de la API
builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri("http://localhost:7084/") 
});

builder.Services.AddScoped<IPersonaClient, PersonaClient>();
builder.Services.AddScoped<ITipoServicioClient, TipoServicioClient>();
builder.Services.AddScoped<IEmpleadoService, EmpleadoService>();
builder.Services.AddScoped<ICabanasService, CabanaService>();
builder.Services.AddScoped<IReservacionService, ReservacionService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

await builder.Build().RunAsync();