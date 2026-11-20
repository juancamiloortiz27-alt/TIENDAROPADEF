using libreria_aplicaciones.entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using presentaciones_tienda.interfaces;

namespace front_tienda.Pages.Cuenta
{
    public class RegistroModel : PageModel
    {
        private IUsuariosPresentacion IUsuariosPresentacion;

        public RegistroModel(IUsuariosPresentacion IUsuariosPresentacion)
        {
            this.IUsuariosPresentacion = IUsuariosPresentacion;
        }

        [BindProperty] public RegistroCliente Registro { get; set; } = new();
        public string? Mensaje { get; set; }

        public void OnGet() { }

        public IActionResult OnPostRegistrar()
        {
            try
            {
                var resultado = this.IUsuariosPresentacion.RegistrarCliente(Registro);
                HttpContext.Session.SetString("Token", resultado.Token!);
                HttpContext.Session.SetString("Correo", resultado.Correo!);
                HttpContext.Session.SetString("Rol", resultado.Rol!);
                if (resultado.Cliente.HasValue)
                    HttpContext.Session.SetInt32("Cliente", resultado.Cliente.Value);
                return RedirectToPage("/Index");
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
                return Page();
            }
        }
    }
}
