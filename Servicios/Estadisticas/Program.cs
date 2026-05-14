using Microsoft.EntityFrameworkCore;
using Estadisticas.Contratos;
using Estadisticas.Datos.Contexto;
using Estadisticas.Datos.Repositorios;
using Estadisticas.Logica.Servicios;
using SoapCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSoapCore();

builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("UserDb"))
           .UseSnakeCaseNamingConvention());

builder.Services.AddDbContext<ContentDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ContentDb"))
           .UseSnakeCaseNamingConvention());

builder.Services.AddScoped<IRepositorioEstadisticas, RepositorioEstadisticas>();
builder.Services.AddScoped<IServicioEstadisticas, ServicioEstadisticas>();
builder.Services.AddScoped<IServicioEstadisticasSoap, ServicioEstadisticasSoap>();

var app = builder.Build();

app.UseRouting();

((IApplicationBuilder)app).UseSoapEndpoint<IServicioEstadisticasSoap>("/soap/estadisticas", new SoapEncoderOptions(), SoapSerializer.DataContractSerializer);

app.MapControllers();

app.Run();
