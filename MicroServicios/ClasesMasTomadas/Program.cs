using Microsoft.EntityFrameworkCore;
using ClasesMasTomadas.Datos.Contexto;
using ClasesMasTomadas.Datos.Repositorios;
using ClasesMasTomadas.Logica.Servicios;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("UserDb"))
           .UseSnakeCaseNamingConvention());

builder.Services.AddDbContext<ContentDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ContentDb"))
           .UseSnakeCaseNamingConvention());

builder.Services.AddScoped<IRepositorioClasesMasTomadas, RepositorioClasesMasTomadas>();
builder.Services.AddScoped<IServicioClasesMasTomadas, ServicioClasesMasTomadas>();

var app = builder.Build();

app.MapControllers();
app.Run();
