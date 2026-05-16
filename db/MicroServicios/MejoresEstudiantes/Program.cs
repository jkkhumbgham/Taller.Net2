using MejoresEstudiantes.Datos.Repositorios;
using MejoresEstudiantes.Logica.Servicios;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IRepositorioMejoresEstudiantes, RepositorioMejoresEstudiantes>();
builder.Services.AddScoped<IServicioMejoresEstudiantes, ServicioMejoresEstudiantes>();

var app = builder.Build();
app.MapControllers();
app.Run();
