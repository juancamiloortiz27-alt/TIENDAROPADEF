using libreria_aplicaciones.implementaciones;
using libreria_aplicaciones.interfaces;
using libreria_aplicaciones.nucleo;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
var clave = builder.Configuration["Autenticacion:Clave"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opciones => opciones.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Autenticacion:Emisor"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Autenticacion:Audiencia"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(clave)),
        ValidateLifetime = true,
        RoleClaimType = System.Security.Claims.ClaimTypes.Role
    });
builder.Services.AddScoped<IConexion>(provider =>
{
    var conexion = new Conexion(provider.GetRequiredService<IHttpContextAccessor>());
    conexion.StringConexion = Datosgenerales.ObtenerStringConexion();
    return conexion;
});
builder.Services.AddScoped<IAuditoriasAplicacion, AuditoriasAplicacion>();
builder.Services.AddScoped<ICategoriasAplicacion, CategoriasAplicacion>();
builder.Services.AddScoped<IMarcasAplicacion, MarcasAplicacion>();
builder.Services.AddScoped<IColoresAplicacion, ColoresAplicacion>();
builder.Services.AddScoped<ITallasAplicacion, TallasAplicacion>();
builder.Services.AddScoped<IProveedoresAplicacion, ProveedoresAplicacion>();
builder.Services.AddScoped<IProductosAplicacion, ProductosAplicacion>();
builder.Services.AddScoped<IInventariosAplicacion, InventariosAplicacion>();
builder.Services.AddScoped<ICarritosAplicacion, CarritosAplicacion>();
builder.Services.AddScoped<IDireccionesAplicacion, DireccionesAplicacion>();
builder.Services.AddScoped<IPedidosAplicacion, PedidosAplicacion>();
builder.Services.AddScoped<ICuponesAplicacion, CuponesAplicacion>();
builder.Services.AddScoped<IMetodoPagosAplicacion, MetodoPagosAplicacion>();
builder.Services.AddScoped<ITransportadorasAplicacion, TransportadorasAplicacion>();
builder.Services.AddScoped<IEnviosAplicacion, EnviosAplicacion>();
builder.Services.AddScoped<IReseñasAplicacion, ReseñasAplicacion>();
builder.Services.AddScoped<IFavoritosAplicacion, FavoritosAplicacion>();
builder.Services.AddScoped<IClientesAplicacion, ClientesAplicacion>();
builder.Services.AddScoped<IUsuariosAplicacion, UsuariosAplicacion>();

var app = builder.Build();

var correoAdministrador = builder.Configuration["AdministradorInicial:Correo"];
var contrasenaAdministrador = builder.Configuration["AdministradorInicial:Contrasena"];
if (!string.IsNullOrWhiteSpace(correoAdministrador) && !string.IsNullOrWhiteSpace(contrasenaAdministrador))
{
    using var scope = app.Services.CreateScope();
    var usuariosAplicacion = scope.ServiceProvider.GetRequiredService<IUsuariosAplicacion>();
    usuariosAplicacion.CrearAdministradorInicial(correoAdministrador, contrasenaAdministrador);
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/", () => "");
app.Run();
