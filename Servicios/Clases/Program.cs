using Microsoft.EntityFrameworkCore;
using Clases.Contratos;
using Clases.Datos.Contexto;
using Clases.Datos.Repositorios;
using Clases.Logica.Servicios;
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

builder.Services.AddScoped<IRepositorioClases, RepositorioClases>();
builder.Services.AddScoped<IServicioClases, ServicioClases>();
builder.Services.AddScoped<IServicioClasesSoap, ServicioClasesSoap>();

var app = builder.Build();

app.UseRouting();

((IApplicationBuilder)app).UseSoapEndpoint<IServicioClasesSoap>("/soap/clases", new SoapEncoderOptions(), SoapSerializer.DataContractSerializer);

app.MapControllers();

app.Run();
