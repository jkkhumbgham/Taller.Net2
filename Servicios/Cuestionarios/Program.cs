using Microsoft.EntityFrameworkCore;
using Cuestionarios.Datos.Contexto;
using Cuestionarios.Datos.Repositorios;
using Cuestionarios.Logica.Servicios;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("UserDb"))
           .UseSnakeCaseNamingConvention());

builder.Services.AddDbContext<ContentDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ContentDb"))
           .UseSnakeCaseNamingConvention());

builder.Services.AddScoped<IRepositorioCuestionarios, RepositorioCuestionarios>();
builder.Services.AddScoped<IServicioCuestionarios, ServicioCuestionarios>();

var app = builder.Build();

app.MapControllers();
app.Run();
