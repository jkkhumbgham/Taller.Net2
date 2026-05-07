using Microsoft.EntityFrameworkCore;
using Clases.Datos.Contexto;
using Clases.Datos.Repositorios;
using Clases.Logica.Servicios;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("UserDb"))
           .UseSnakeCaseNamingConvention());

builder.Services.AddDbContext<ContentDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ContentDb"))
           .UseSnakeCaseNamingConvention());

builder.Services.AddScoped<IRepositorioClases, RepositorioClases>();
builder.Services.AddScoped<IServicioClases, ServicioClases>();

var app = builder.Build();

app.MapControllers();
app.Run();
