using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Monolitica.Datos.Contexto;
using Monolitica.Datos.Repositorios;
using Monolitica.GrpcServicios;
using Monolitica.Logica.Servicios;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8080, o => o.Protocols = HttpProtocols.Http2);
});

builder.Services.AddGrpc();

builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("UserDb"))
           .UseSnakeCaseNamingConvention());

builder.Services.AddDbContext<ContentDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ContentDb"))
           .UseSnakeCaseNamingConvention());

builder.Services.AddScoped<IRepositorioInscripciones, RepositorioInscripciones>();
builder.Services.AddScoped<IRepositorioProgresoLecciones, RepositorioProgresoLecciones>();
builder.Services.AddScoped<IRepositorioIntentosCuestionarios, RepositorioIntentosCuestionarios>();
builder.Services.AddScoped<IServicioEstadisticas, ServicioEstadisticas>();

var app = builder.Build();

app.MapGrpcService<EstadisticasGrpcServicio>();
app.MapGrpcService<CrudGrpcServicio>();

app.Run();
