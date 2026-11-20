using libreria_aplicaciones.entidades;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using presentaciones_tienda.interfaces;

namespace front_tienda.Pages.Cuenta
{
    public class AccesoModel : PageModel
    {
        private IUsuariosPresentacion IUsuariosPresentacion;

        public AccesoModel(IUsuariosPresentacion IUsuariosPresentacion)
        {
            this.IUsuariosPresentacion = IUsuariosPresentacion;
        }

        [BindProperty] public CredencialesUsuario Credenciales { get; set; } = new();
        [BindProperty(SupportsGet = true)] public string? ReturnUrl { get; set; }
        public string? Mensaje { get; set; }
        public bool CredencialesInvalidas { get; set; }

        public void OnGet() { }

        public IActionResult OnPostIngresar()
        {
            try
            {
                var resultado = this.IUsuariosPresentacion.IniciarSesion(Credenciales);
                GuardarSesion(resultado);
                if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                    return LocalRedirect(ReturnUrl);
                return RedirectToPage("/Index");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
            {
                CredencialesInvalidas = true;
                return Page();
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
                return Page();
            }
        }

        public IActionResult OnPostSalir()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Index");
        }

        private void GuardarSesion(ResultadoAutenticacion resultado)
        {
            HttpContext.Session.SetString("Token", resultado.Token!);
            HttpContext.Session.SetString("Correo", resultado.Correo!);
            HttpContext.Session.SetString("Rol", resultado.Rol!);
            if (resultado.Cliente.HasValue)
                HttpContext.Session.SetInt32("Cliente", resultado.Cliente.Value);
        }
    }
}
