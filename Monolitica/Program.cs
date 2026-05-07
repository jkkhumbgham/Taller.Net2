using Microsoft.EntityFrameworkCore;
using Monolitica.Datos.Contexto;
using Monolitica.Datos.Repositorios;
using Monolitica.Logica.Servicios;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Register DbContexts
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("UserDb"))
           .UseSnakeCaseNamingConvention());

builder.Services.AddDbContext<ContentDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ContentDb"))
           .UseSnakeCaseNamingConvention());

// Register Repositories
builder.Services.AddScoped<IRepositorioInscripciones, RepositorioInscripciones>();
builder.Services.AddScoped<IRepositorioProgresoLecciones, RepositorioProgresoLecciones>();
builder.Services.AddScoped<IRepositorioIntentosCuestionarios, RepositorioIntentosCuestionarios>();

// Register Services
builder.Services.AddScoped<IServicioEstadisticas, ServicioEstadisticas>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
