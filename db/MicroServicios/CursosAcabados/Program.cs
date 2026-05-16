using CursosAcabados.Datos.Repositorios;
using CursosAcabados.Logica.Servicios;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IRepositorioCursosAcabados, RepositorioCursosAcabados>();
builder.Services.AddScoped<IServicioCursosAcabados, ServicioCursosAcabados>();

var app = builder.Build();
app.MapControllers();
app.Run();
