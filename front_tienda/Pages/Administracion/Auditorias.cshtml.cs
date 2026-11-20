using System.Net;
using System.Text.RegularExpressions;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using presentaciones_tienda.interfaces;

namespace front_tienda.Pages.Administracion
{
    public class AuditoriasModel : PageModel
    {
        private IAuditoriasPresentacion IAuditoriasPresentacion;

        public AuditoriasModel(IAuditoriasPresentacion IAuditoriasPresentacion)
        {
            this.IAuditoriasPresentacion = IAuditoriasPresentacion;
        }

        public List<libreria_aplicaciones.entidades.Auditorias> Lista { get; set; } = new();
        public string? Mensaje { get; set; }
        public bool SesionExpirada { get; set; }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("Rol") != "Administrador")
                return Forbid();

            try
            {
                Lista = this.IAuditoriasPresentacion.Consultar();
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
            {
                HttpContext.Session.Clear();
                SesionExpirada = true;
                Mensaje = "La sesión expiró. Inicia sesión de nuevo como administrador.";
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
            }

            return Page();
        }

        public IActionResult OnGetDescargarExcel()
        {
            if (HttpContext.Session.GetString("Rol") != "Administrador")
                return Forbid();

            try
            {
                var auditorias = this.IAuditoriasPresentacion.Consultar();
                var filas = auditorias.Select(auditoria => (IReadOnlyList<object?>)new object?[]
                {
                    auditoria.IdAuditoria,
                    auditoria.FechaHoraUtc.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"),
                    auditoria.UsuarioId,
                    auditoria.CorreoUsuario,
                    auditoria.RolUsuario,
                    auditoria.Accion,
                    auditoria.Entidad,
                    auditoria.IdRegistro,
                    MostrarCambios(auditoria.Cambios)
                }).ToList();
                var encabezados = new[] { "Id auditoría", "Fecha y hora local", "Id usuario", "Correo", "Rol", "Acción", "Entidad", "Id registro", "Cambios" };
                var contenido = front_tienda.Utilidades.ExcelExportador.Crear("Auditorías", encabezados, filas);
                return File(contenido, front_tienda.Utilidades.ExcelExportador.TipoContenido, $"auditorias-{DateTime.Now:yyyyMMdd}.xlsx");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
            {
                HttpContext.Session.Clear();
                return RedirectToPage("/Cuenta/Acceso");
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
                Lista = new();
                return Page();
            }
        }

        public string MostrarCambios(string? cambios)
        {
            if (string.IsNullOrWhiteSpace(cambios))
                return "Sin detalle de campos";

            try
            {
                using var documento = JsonDocument.Parse(cambios);
                if (documento.RootElement.ValueKind != JsonValueKind.Object)
                    return documento.RootElement.ToString();

                var lineas = new List<string>();
                foreach (var campo in documento.RootElement.EnumerateObject())
                {
                    if (campo.Value.ValueKind == JsonValueKind.Object &&
                        campo.Value.TryGetProperty("Anterior", out var anterior) &&
                        campo.Value.TryGetProperty("Nuevo", out var nuevo))
                    {
                        var valorAnterior = MostrarValor(anterior);
                        var valorNuevo = MostrarValor(nuevo);
                        var detalle = anterior.ValueKind == JsonValueKind.Null
                            ? $"Agregado: {valorNuevo}"
                            : nuevo.ValueKind == JsonValueKind.Null
                                ? $"Eliminado: {valorAnterior}"
                                : $"{valorAnterior} → {valorNuevo}";
                        lineas.Add($"{HumanizarNombre(campo.Name)}: {detalle}");
                    }
                    else
                    {
                        lineas.Add($"{HumanizarNombre(campo.Name)}: {MostrarValor(campo.Value)}");
                    }
                }

                return string.Join(Environment.NewLine, lineas);
            }
            catch
            {
                return cambios;
            }
        }

        private static string MostrarValor(JsonElement valor)
        {
            return valor.ValueKind switch
            {
                JsonValueKind.Null => "(vacío)",
                JsonValueKind.True => "Sí",
                JsonValueKind.False => "No",
                JsonValueKind.String => valor.GetString() ?? "(vacío)",
                _ => valor.ToString()
            };
        }

        private static string HumanizarNombre(string nombre)
        {
            var humanizado = Regex.Replace(nombre, "([a-z0-9])([A-Z])", "$1 $2");
            return humanizado.Length == 0
                ? humanizado
                : char.ToUpperInvariant(humanizado[0]) + humanizado[1..].ToLowerInvariant();
        }
    }
}
