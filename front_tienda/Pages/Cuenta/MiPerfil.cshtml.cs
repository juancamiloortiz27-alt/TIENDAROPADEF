using libreria_aplicaciones.entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using presentaciones_tienda.interfaces;

namespace front_tienda.Pages.Cuenta
{
    public class MiPerfilModel : PageModel
    {
        private IClientesPresentacion IClientesPresentacion;

        public MiPerfilModel(IClientesPresentacion IClientesPresentacion)
        {
            this.IClientesPresentacion = IClientesPresentacion;
        }

        [BindProperty] public Clientes Cliente { get; set; } = new();
        public string? Mensaje { get; set; }
        public bool Error { get; set; }

        public IActionResult OnGet()
        {
            if (!EsCliente())
                return RedirectToPage("/Cuenta/Acceso", new { returnUrl = Request.Path.ToString() });
            Cargar();
            return Page();
        }

        public IActionResult OnPostGuardar()
        {
            if (!EsCliente()) return Forbid();
            try
            {
                Cliente = this.IClientesPresentacion.ActualizarPerfil(Cliente);
                Mensaje = "Perfil actualizado";
                ModelState.Clear();
            }
            catch (Exception ex) { Mensaje = ex.Message; Error = true; }
            if (Error) Cargar();
            return Page();
        }

        private bool EsCliente()
        {
            return HttpContext.Session.GetString("Rol") == "Cliente" &&
                HttpContext.Session.GetInt32("Cliente").HasValue;
        }

        private void Cargar()
        {
            try { Cliente = this.IClientesPresentacion.ConsultarPerfil(); }
            catch (Exception ex) { Mensaje = ex.Message; Error = true; }
        }
    }
}
