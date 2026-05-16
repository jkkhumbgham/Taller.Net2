using Notas.Datos.Repositorios;
using Notas.Logica.Servicios;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IRepositorioNotas, RepositorioNotas>();
builder.Services.AddScoped<IServicioNotas, ServicioNotas>();

var app = builder.Build();
app.MapControllers();
app.Run();
