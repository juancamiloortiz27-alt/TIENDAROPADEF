using presentaciones_tienda.interfaces;
using presentaciones_tienda.implementaciones;
using libreria_aplicaciones.interfaces;
using libreria_aplicaciones.implementaciones;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddScoped<IComunicaciones,Comunicaciones>();
builder.Services.AddScoped<ICategoriasPresentacion, CategoriasPresentacion>();
builder.Services.AddScoped<IMarcasPresentacion, MarcasPresentacion>();
builder.Services.AddScoped<IColoresPresentacion, ColoresPresentacion>();
builder.Services.AddScoped<ITallasPresentacion, TallasPresentacion>();
builder.Services.AddScoped<IProveedoresPresentacion, ProveedoresPresentacion>();
builder.Services.AddScoped<IProductosPresentacion, ProductosPresentacion>();
builder.Services.AddScoped<IInventariosPresentacion, InventariosPresentacion>();
builder.Services.AddScoped<ICarritosPresentacion, CarritosPresentacion>();
builder.Services.AddScoped<IDireccionesPresentacion, DireccionesPresentacion>();
builder.Services.AddScoped<IPedidosPresentacion, PedidosPresentacion>();
builder.Services.AddScoped<ICuponesPresentacion, CuponesPresentacion>();
builder.Services.AddScoped<IMetodoPagosPresentacion, MetodoPagosPresentacion>();
builder.Services.AddScoped<ITransportadorasPresentacion, TransportadorasPresentacion>();
builder.Services.AddScoped<IEnviosPresentacion, EnviosPresentacion>();
builder.Services.AddScoped<IReseñasPresentacion, ReseñasPresentacion>();
builder.Services.AddScoped<IFavoritosPresentacion, FavoritosPresentacion>();
builder.Services.AddScoped<IClientesPresentacion, ClientesPresentacion>();
builder.Services.AddScoped<IUsuariosPresentacion, UsuariosPresentacion>();
builder.Services.AddScoped<IAuditoriasPresentacion, AuditoriasPresentacion>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseSession();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
