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

// REGLA DE ORO: Aquí va la URL de tu BACKEND (API)
builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri("http://localhost:5017/") 
});

builder.Services.AddScoped<IClienteClient, ClienteClient>();
builder.Services.AddScoped<IPersonaClient, PersonaClient>();
builder.Services.AddScoped<ITipoServicioClient, TipoServicioClient>();
builder.Services.AddScoped<IEmpleadoService, EmpleadoService>();
builder.Services.AddScoped<ICabanasService, CabanaService>();
builder.Services.AddScoped<IReservacionService, ReservacionService>();
builder.Services.AddScoped<IPuestoTrabajoService, PuestoTrabajoService>();
builder.Services.AddScoped<IServicioService, ServicioService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IReservacionServicioService, ReservacionServicioService>();
builder.Services.AddScoped<ITipoCabanaService, TipoCabanaService>();

await builder.Build().RunAsync();