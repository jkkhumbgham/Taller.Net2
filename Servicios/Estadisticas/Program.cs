using Microsoft.EntityFrameworkCore;
using Estadisticas.Datos.Contexto;
using Estadisticas.Datos.Repositorios;
using Estadisticas.Logica.Servicios;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("UserDb"))
           .UseSnakeCaseNamingConvention());

builder.Services.AddDbContext<ContentDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ContentDb"))
           .UseSnakeCaseNamingConvention());

builder.Services.AddScoped<IRepositorioEstadisticas, RepositorioEstadisticas>();
builder.Services.AddScoped<IServicioEstadisticas, ServicioEstadisticas>();

var app = builder.Build();

app.MapControllers();
app.Run();
