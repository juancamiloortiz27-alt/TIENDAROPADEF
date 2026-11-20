using libreria_aplicaciones.entidades;
using Microsoft.EntityFrameworkCore;
using libreria_aplicaciones.interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Security.Claims;
using System.Text.Json;

namespace libreria_aplicaciones.implementaciones
{
    public class Conexion : DbContext, IConexion
    {
        private readonly IHttpContextAccessor? IHttpContextAccessor;

        public Conexion(IHttpContextAccessor? IHttpContextAccessor = null)
        {
            this.IHttpContextAccessor = IHttpContextAccessor;
        }

        public string? StringConexion { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(this.StringConexion!, p => { });
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }

        public DbSet<Carritos>? Carritos { get; set; }
        public DbSet<Categorias>? Categorias { get; set; }
        public DbSet<Clientes>? Clientes { get; set; }
        public DbSet<Colores>? Colores { get; set; }
        public DbSet<Cupones>? Cupones { get; set; }
        public DbSet<DetalleCarritos>? DetalleCarritos { get; set; }
        public DbSet<DetallePedidos>? DetallePedidos { get; set; }
        public DbSet<Direcciones>? Direcciones { get; set; }
        public DbSet<Envios>? Envios { get; set; }
        public DbSet<Favoritos>? Favoritos { get; set; }
        public DbSet<Inventarios>? Inventarios { get; set; }
        public DbSet<Marcas>? Marcas { get; set; }
        public DbSet<MetodoPagos>? MetodoPagos { get; set; }
        public DbSet<Pagos>? Pagos { get; set; }
        public DbSet<Pedidos>? Pedidos { get; set; }
        public DbSet<Productos>? Productos { get; set; }
        public DbSet<Proveedores>? Proveedores { get; set; }
        public DbSet<Reseñas>? Reseñas { get; set; }
        public DbSet<Tallas>? Tallas { get; set; }
        public DbSet<Transportadoras>? Transportadoras { get; set; }
        public DbSet<Usuarios>? Usuarios { get; set; }
        public DbSet<Auditorias>? Auditorias { get; set; }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            var entradas = ChangeTracker.Entries()
                .Where(x => x.Entity is not libreria_aplicaciones.entidades.Auditorias &&
                    (x.State == EntityState.Added || x.State == EntityState.Modified || x.State == EntityState.Deleted))
                .ToList();

            if (entradas.Count == 0)
                return base.SaveChanges(acceptAllChangesOnSuccess);

            using var transaccion = Database.CurrentTransaction == null ? Database.BeginTransaction() : null;
            var auditoriasPendientes = entradas
                .Select(CrearAuditoria)
                .Where(x => x != null)
                .ToList();

            var guardados = base.SaveChanges(acceptAllChangesOnSuccess);

            foreach (var pendiente in auditoriasPendientes)
            {
                pendiente!.Auditoria.IdRegistro = ObtenerId(pendiente.Entrada);
                this.Auditorias!.Add(pendiente.Auditoria);
            }

            if (auditoriasPendientes.Count > 0)
                base.SaveChanges(acceptAllChangesOnSuccess);

            transaccion?.Commit();
            return guardados;
        }

        private PendienteAuditoria? CrearAuditoria(EntityEntry entrada)
        {
            var estado = entrada.State;
            var valoresAnteriores = estado == EntityState.Modified || estado == EntityState.Deleted
                ? entrada.GetDatabaseValues()
                : null;
            var claves = entrada.Metadata.FindPrimaryKey()?.Properties.Select(x => x.Name).ToHashSet() ?? new HashSet<string>();
            var cambios = new Dictionary<string, object?>();

            foreach (var propiedad in entrada.Properties)
            {
                if (claves.Contains(propiedad.Metadata.Name))
                    continue;

                var anterior = valoresAnteriores == null ? propiedad.OriginalValue : valoresAnteriores[propiedad.Metadata.Name];
                var actual = propiedad.CurrentValue;
                var confidencial = EsDatoConfidencial(propiedad.Metadata.Name);

                if (estado == EntityState.Added)
                    cambios[propiedad.Metadata.Name] = new { Anterior = (object?)null, Nuevo = confidencial ? "[Dato confidencial]" : actual };
                else if (estado == EntityState.Deleted)
                    cambios[propiedad.Metadata.Name] = new { Anterior = confidencial ? "[Dato confidencial]" : anterior, Nuevo = (object?)null };
                else if (!Equals(anterior, actual))
                    cambios[propiedad.Metadata.Name] = new { Anterior = confidencial ? "[Dato confidencial]" : anterior, Nuevo = confidencial ? "[Dato confidencial]" : actual };
            }

            if (estado == EntityState.Modified && cambios.Count == 0)
                return null;

            var usuario = this.IHttpContextAccessor?.HttpContext?.User;
            var contextoHttp = this.IHttpContextAccessor?.HttpContext;
            var autenticado = usuario?.Identity?.IsAuthenticated == true;
            var idUsuarioTexto = usuario?.FindFirstValue(ClaimTypes.NameIdentifier);
            int? idUsuario = int.TryParse(idUsuarioTexto, out var id) ? id : null;
            var correoEntidad = entrada.Properties.FirstOrDefault(x => x.Metadata.Name == "Correo")?.CurrentValue?.ToString();
            var rolEntidad = entrada.Properties.FirstOrDefault(x => x.Metadata.Name == "Rol")?.CurrentValue?.ToString();
            var correo = autenticado
                ? usuario?.FindFirstValue(ClaimTypes.Email)
                : contextoHttp == null ? "Sistema" : correoEntidad ?? "Registro público";
            var rol = autenticado
                ? usuario?.FindFirstValue(ClaimTypes.Role)
                : contextoHttp == null ? "Sistema" : rolEntidad ?? "Cliente";

            var accion = estado switch
            {
                EntityState.Added => "Insertar",
                EntityState.Modified => "Actualizar",
                EntityState.Deleted => "Eliminar",
                _ => "Cambio"
            };

            var auditoria = new Auditorias
            {
                FechaHoraUtc = DateTime.UtcNow,
                UsuarioId = idUsuario,
                CorreoUsuario = correo,
                RolUsuario = rol,
                Accion = accion,
                Entidad = entrada.Metadata.ClrType.Name,
                IdRegistro = ObtenerId(entrada),
                Cambios = JsonSerializer.Serialize(cambios)
            };

            return new PendienteAuditoria(entrada, auditoria);
        }

        private static string? ObtenerId(EntityEntry entrada)
        {
            var claves = entrada.Metadata.FindPrimaryKey()?.Properties;
            if (claves == null || claves.Count == 0)
                return null;

            return string.Join(", ", claves.Select(x => $"{x.Name}: {entrada.Property(x.Name).CurrentValue}"));
        }

        private static bool EsDatoConfidencial(string nombre)
        {
            return nombre.Contains("Contrasena", StringComparison.OrdinalIgnoreCase) ||
                nombre.Contains("Hash", StringComparison.OrdinalIgnoreCase) ||
                nombre.Contains("Token", StringComparison.OrdinalIgnoreCase);
        }

        private sealed record PendienteAuditoria(EntityEntry Entrada, Auditorias Auditoria);

    }
}
