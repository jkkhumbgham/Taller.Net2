using Microsoft.EntityFrameworkCore;
using MejoresEstudiantes.Datos.Contexto;
using MejoresEstudiantes.Datos.Repositorios;
using MejoresEstudiantes.Logica.Servicios;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("UserDb"))
           .UseSnakeCaseNamingConvention());

builder.Services.AddDbContext<ContentDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ContentDb"))
           .UseSnakeCaseNamingConvention());

builder.Services.AddScoped<IRepositorioMejoresEstudiantes, RepositorioMejoresEstudiantes>();
builder.Services.AddScoped<IServicioMejoresEstudiantes, ServicioMejoresEstudiantes>();

var app = builder.Build();

app.MapControllers();
app.Run();
