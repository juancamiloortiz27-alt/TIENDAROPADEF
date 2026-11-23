using System.Security.Claims;
using System.Text.Json;
using libreria_aplicaciones.entidades;
using libreria_aplicaciones.implementaciones;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace pruebas_unitarias
{
    [TestClass]
    public class RepositorioPruebas
    {
        [TestMethod]
        public void GuardarEntidadNueva_CreaAuditoriaConValoresYClaveGenerada()
        {
            using var repositorio = CrearRepositorio();
            repositorio.Categorias!.Add(new Categorias { Nombre = "Abrigos", Estado = true });

            var cantidadGuardada = repositorio.SaveChanges();

            Assert.AreEqual(1, cantidadGuardada);
            var categoria = repositorio.Categorias.AsNoTracking().Single();
            var auditoria = repositorio.Auditorias!.AsNoTracking().Single();
            Assert.AreEqual("Insertar", auditoria.Accion);
            Assert.AreEqual(nameof(Categorias), auditoria.Entidad);
            Assert.AreEqual($"IdCategoria: {categoria.IdCategoria}", auditoria.IdRegistro);
            Assert.AreEqual("Sistema", auditoria.CorreoUsuario);
            Assert.AreEqual("Sistema", auditoria.RolUsuario);

            using var cambios = JsonDocument.Parse(auditoria.Cambios!);
            Assert.AreEqual("Abrigos", cambios.RootElement.GetProperty("Nombre").GetProperty("Nuevo").GetString());
        }

        [TestMethod]
        public void ActualizarEntidad_RegistraSoloPropiedadesModificadas()
        {
            using var repositorio = CrearRepositorio();
            repositorio.Categorias!.Add(new Categorias { Nombre = "Abrigos", Descripcion = "Prendas cálidas" });
            repositorio.SaveChanges();
            var categoria = repositorio.Categorias.AsTracking().Single();
            categoria.Nombre = "Chaquetas";

            repositorio.SaveChanges();

            var auditoria = repositorio.Auditorias!.AsNoTracking().OrderBy(x => x.IdAuditoria).Last();
            Assert.AreEqual("Actualizar", auditoria.Accion);
            using var cambios = JsonDocument.Parse(auditoria.Cambios!);
            Assert.AreEqual("Abrigos", cambios.RootElement.GetProperty("Nombre").GetProperty("Anterior").GetString());
            Assert.AreEqual("Chaquetas", cambios.RootElement.GetProperty("Nombre").GetProperty("Nuevo").GetString());
            Assert.IsFalse(cambios.RootElement.TryGetProperty("Descripcion", out _));
        }

        [TestMethod]
        public void EliminarEntidad_RegistraValoresAnteriores()
        {
            using var repositorio = CrearRepositorio();
            var categoria = new Categorias { Nombre = "Abrigos" };
            repositorio.Categorias!.Add(categoria);
            repositorio.SaveChanges();
            repositorio.Categorias.Remove(categoria);

            repositorio.SaveChanges();

            var auditoria = repositorio.Auditorias!.AsNoTracking().OrderBy(x => x.IdAuditoria).Last();
            Assert.AreEqual("Eliminar", auditoria.Accion);
            using var cambios = JsonDocument.Parse(auditoria.Cambios!);
            Assert.AreEqual("Abrigos", cambios.RootElement.GetProperty("Nombre").GetProperty("Anterior").GetString());
            Assert.AreEqual(JsonValueKind.Null, cambios.RootElement.GetProperty("Nombre").GetProperty("Nuevo").ValueKind);
        }

        [TestMethod]
        public void GuardarUsuario_OcultaHashDeContrasenaEnAuditoria()
        {
            using var repositorio = CrearRepositorio();
            repositorio.Usuarios!.Add(new Usuarios { Correo = "cliente@prueba.com", ContrasenaHash = "hash-secreto", Rol = "Cliente" });

            repositorio.SaveChanges();

            var auditoria = repositorio.Auditorias!.AsNoTracking().Single();
            using var cambios = JsonDocument.Parse(auditoria.Cambios!);
            Assert.AreEqual("[Dato confidencial]", cambios.RootElement.GetProperty("ContrasenaHash").GetProperty("Nuevo").GetString());
            Assert.AreEqual("cliente@prueba.com", cambios.RootElement.GetProperty("Correo").GetProperty("Nuevo").GetString());
            Assert.IsFalse(auditoria.Cambios!.Contains("hash-secreto", StringComparison.Ordinal));
        }

        [TestMethod]
        public void GuardarConUsuarioAutenticado_RegistraIdentidadDelContexto()
        {
            var identidad = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "42"),
                new Claim(ClaimTypes.Email, "admin@prueba.com"),
                new Claim(ClaimTypes.Role, "Administrador")
            }, "Pruebas");
            var accessor = new HttpContextAccessor { HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identidad) } };
            using var repositorio = CrearRepositorio(accessor);
            repositorio.Categorias!.Add(new Categorias { Nombre = "Prueba" });

            repositorio.SaveChanges();

            var auditoria = repositorio.Auditorias!.AsNoTracking().Single();
            Assert.AreEqual(42, auditoria.UsuarioId);
            Assert.AreEqual("admin@prueba.com", auditoria.CorreoUsuario);
            Assert.AreEqual("Administrador", auditoria.RolUsuario);
        }

        [TestMethod]
        public void GuardarAuditoriaDirecta_NoGeneraUnaAuditoriaDuplicada()
        {
            using var repositorio = CrearRepositorio();
            repositorio.Auditorias!.Add(new Auditorias { Accion = "Prueba", Entidad = "Categorias" });

            repositorio.SaveChanges();

            Assert.AreEqual(1, repositorio.Auditorias.AsNoTracking().Count());
        }

        private static ConexionPrueba CrearRepositorio(IHttpContextAccessor? accessor = null) => new(accessor, Guid.NewGuid().ToString("N"));

        private sealed class ConexionPrueba : Conexion
        {
            private readonly string nombreBase;

            public ConexionPrueba(IHttpContextAccessor? accessor, string nombreBase) : base(accessor)
            {
                this.nombreBase = nombreBase;
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder
                    .UseInMemoryDatabase(this.nombreBase)
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                    .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            }
        }
    }
}
