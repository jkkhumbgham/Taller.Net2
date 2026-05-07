using Microsoft.EntityFrameworkCore;
using Notas.Datos.Contexto;
using Notas.Datos.Repositorios;
using Notas.Logica.Servicios;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("UserDb"))
           .UseSnakeCaseNamingConvention());

builder.Services.AddDbContext<ContentDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ContentDb"))
           .UseSnakeCaseNamingConvention());

builder.Services.AddScoped<IRepositorioNotas, RepositorioNotas>();
builder.Services.AddScoped<IServicioNotas, ServicioNotas>();

var app = builder.Build();

app.MapControllers();
app.Run();
