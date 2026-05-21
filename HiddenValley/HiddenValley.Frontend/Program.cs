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

builder.Services.AddScoped<IPersonaClient, PersonaClient>();
builder.Services.AddScoped<ITipoServicioClient, TipoServicioClient>();
builder.Services.AddScoped<ICabanasService, CabanaService>();

await builder.Build().RunAsync();