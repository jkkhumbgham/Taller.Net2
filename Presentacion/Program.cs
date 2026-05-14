using Presentacion.Components;
using Presentacion.Datos.Conexion;
using Presentacion.Datos.Repositorios;
using Presentacion.Datos.Repositorios.Interfaces;
using Presentacion.Servicios;
using Presentacion.Servicios.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<IFabricaConexion, FabricaConexion>();

builder.Services.AddScoped<IRepositorioUsuarios, RepositorioUsuarios>();
builder.Services.AddScoped<IRepositorioCursos, RepositorioCursos>();
builder.Services.AddScoped<IRepositorioInscripciones, RepositorioInscripciones>();

builder.Services.AddScoped<IServicioSelectorArquitectura, ServicioSelectorArquitectura>();

var monolitoBaseUrl = builder.Configuration["Arquitecturas:Monolitico:BaseUrl"] ?? "http://monolito:5001";
builder.Services.AddHttpClient<IServicioMonolitico, ServicioMonolitico>(client =>
{
    client.BaseAddress = new Uri(monolitoBaseUrl);
});

builder.Services.AddHttpClient("svc-clases", client =>
{
    var url = builder.Configuration["Arquitecturas:Servicios:ClasesUrl"] ?? "http://svc-clases:5021";
    client.BaseAddress = new Uri(url);
});

builder.Services.AddHttpClient("svc-estadisticas", client =>
{
    var url = builder.Configuration["Arquitecturas:Servicios:EstadisticasUrl"] ?? "http://svc-estadisticas:5022";
    client.BaseAddress = new Uri(url);
});

builder.Services.AddHttpClient("micro-notas", client =>
{
    var url = builder.Configuration["Arquitecturas:MicroServicios:NotasUrl"] ?? "http://micro-notas:5010";
    client.BaseAddress = new Uri(url);
});

builder.Services.AddHttpClient("micro-cursos-acabados", client =>
{
    var url = builder.Configuration["Arquitecturas:MicroServicios:CursosAcabadosUrl"] ?? "http://micro-cursos-acabados:5011";
    client.BaseAddress = new Uri(url);
});

builder.Services.AddHttpClient("micro-clases-mas-tomadas", client =>
{
    var url = builder.Configuration["Arquitecturas:MicroServicios:ClasesMasTomadosUrl"] ?? "http://micro-clases-mas-tomadas:5012";
    client.BaseAddress = new Uri(url);
});

builder.Services.AddHttpClient("micro-mejores-estudiantes", client =>
{
    var url = builder.Configuration["Arquitecturas:MicroServicios:MejoresEstudiantesUrl"] ?? "http://micro-mejores-estudiantes:5013";
    client.BaseAddress = new Uri(url);
});

builder.Services.AddScoped<IServicioArquitecturaServicios, ServicioArquitecturaServicios>();
builder.Services.AddScoped<IServicioArquitecturaMicroServicios, ServicioArquitecturaMicroServicios>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
