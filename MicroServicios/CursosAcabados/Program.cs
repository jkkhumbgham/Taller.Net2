using Microsoft.EntityFrameworkCore;
using CursosAcabados.Datos.Contexto;
using CursosAcabados.Datos.Repositorios;
using CursosAcabados.Logica.Servicios;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("UserDb"))
           .UseSnakeCaseNamingConvention());

builder.Services.AddDbContext<ContentDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ContentDb"))
           .UseSnakeCaseNamingConvention());

builder.Services.AddScoped<IRepositorioCursosAcabados, RepositorioCursosAcabados>();
builder.Services.AddScoped<IServicioCursosAcabados, ServicioCursosAcabados>();

var app = builder.Build();

app.MapControllers();
app.Run();
