using Monolitica.Grpc;
using Presentacion.Components;
using Presentacion.Servicios;
using Presentacion.Servicios.Interfaces;

AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var monolitoGrpcUrl = builder.Configuration["Arquitecturas:Monolitico:GrpcUrl"] ?? "http://localhost:5001";

builder.Services.AddGrpcClient<EstadisticasGrpc.EstadisticasGrpcClient>(o =>
{
    o.Address = new Uri(monolitoGrpcUrl);
});

builder.Services.AddGrpcClient<CrudGrpc.CrudGrpcClient>(o =>
{
    o.Address = new Uri(monolitoGrpcUrl);
});

builder.Services.AddScoped<IServicioCrud, ServicioCrud>();
builder.Services.AddScoped<IServicioSelectorArquitectura, ServicioSelectorArquitectura>();
builder.Services.AddScoped<IServicioMonolitico, ServicioMonolitico>();
builder.Services.AddScoped<IServicioArquitecturaServicios, ServicioArquitecturaServicios>();

builder.Services.AddHttpClient("micro-notas", client =>
{
    var url = builder.Configuration["Arquitecturas:MicroServicios:NotasUrl"] ?? "http://localhost:5010";
    client.BaseAddress = new Uri(url);
});

builder.Services.AddHttpClient("micro-cursos-acabados", client =>
{
    var url = builder.Configuration["Arquitecturas:MicroServicios:CursosAcabadosUrl"] ?? "http://localhost:5011";
    client.BaseAddress = new Uri(url);
});

builder.Services.AddHttpClient("micro-clases-mas-tomadas", client =>
{
    var url = builder.Configuration["Arquitecturas:MicroServicios:ClasesMasTomadosUrl"] ?? "http://localhost:5012";
    client.BaseAddress = new Uri(url);
});

builder.Services.AddHttpClient("micro-mejores-estudiantes", client =>
{
    var url = builder.Configuration["Arquitecturas:MicroServicios:MejoresEstudiantesUrl"] ?? "http://localhost:5013";
    client.BaseAddress = new Uri(url);
});

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
