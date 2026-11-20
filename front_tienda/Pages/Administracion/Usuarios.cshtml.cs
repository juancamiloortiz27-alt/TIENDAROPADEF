using libreria_aplicaciones.entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using presentaciones_tienda.interfaces;
using System.Net;

namespace front_tienda.Pages.Administracion
{
    public class UsuariosModel : PageModel
    {
        private IUsuariosPresentacion IUsuariosPresentacion;

        public UsuariosModel(IUsuariosPresentacion IUsuariosPresentacion)
        {
            this.IUsuariosPresentacion = IUsuariosPresentacion;
        }

        public List<UsuarioResumen> Lista { get; set; } = new();
        public string? Mensaje { get; set; }
        public bool SesionExpirada { get; set; }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("Rol") != "Administrador")
                return Forbid();
            Consultar();
            return Page();
        }

        public IActionResult OnPostCambiarRol(CambioRolUsuario entidad)
        {
            if (HttpContext.Session.GetString("Rol") != "Administrador")
                return Forbid();
            try
            {
                this.IUsuariosPresentacion.CambiarRol(entidad);
                Mensaje = "Rol actualizado";
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
            }
            Consultar();
            return Page();
        }

        public IActionResult OnPostCambiarEstado(EstadoUsuario entidad)
        {
            if (HttpContext.Session.GetString("Rol") != "Administrador")
                return Forbid();
            try
            {
                this.IUsuariosPresentacion.CambiarEstado(entidad);
                Mensaje = "Estado de la cuenta actualizado";
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
            }
            Consultar();
            return Page();
        }

        private void Consultar()
        {
            try
            {
                Lista = this.IUsuariosPresentacion.ConsultarUsuarios();
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
        }
    }
}
