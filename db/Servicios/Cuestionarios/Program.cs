using Cuestionarios.Contratos;
using Cuestionarios.Datos.Contexto;
using Cuestionarios.Datos.Repositorios;
using Cuestionarios.Logica.Servicios;
using Microsoft.EntityFrameworkCore;
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

builder.Services.AddScoped<IRepositorioCuestionarios, RepositorioCuestionarios>();
builder.Services.AddScoped<IServicioCuestionarios, ServicioCuestionarios>();
builder.Services.AddScoped<IServicioCuestionariosSoap, ServicioCuestionariosSoap>();

var app = builder.Build();

app.UseRouting();

((IApplicationBuilder)app).UseSoapEndpoint<IServicioCuestionariosSoap>(
    "/soap/cuestionarios", new SoapEncoderOptions(), SoapSerializer.DataContractSerializer);

app.MapControllers();
app.Run();
