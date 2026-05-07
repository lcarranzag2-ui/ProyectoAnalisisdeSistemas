using Microsoft.EntityFrameworkCore;
using HiddenValley.API.Data; 
using HiddenValley.API.Interfaces; 
using HiddenValley.API.Services;

// Habilitar comportamiento legacy de timestamps en Npgsql 9 para que los DateTime
// (sin Kind explícito) sean compatibles con columnas TIMESTAMP / DATE de Postgres.
// Esto evita errores como "Cannot write DateTime with Kind=Unspecified".
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

//Configurar de PostgresSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IServicioService, ServicioService>();

builder.Services.AddControllers();

//SWAGGER
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection(); 

app.UseRouting();

app.UseCors(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.UseAuthorization();

app.MapControllers();

app.Run();