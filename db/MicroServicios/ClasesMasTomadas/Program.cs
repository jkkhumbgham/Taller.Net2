using ClasesMasTomadas.Datos.Repositorios;
using ClasesMasTomadas.Logica.Servicios;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IRepositorioClasesMasTomadas, RepositorioClasesMasTomadas>();
builder.Services.AddScoped<IServicioClasesMasTomadas, ServicioClasesMasTomadas>();

var app = builder.Build();
app.MapControllers();
app.Run();
