using libreria_aplicaciones.entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using presentaciones_tienda.interfaces;

namespace front_tienda.Pages.Ventanas
{
    public class MisFavoritosModel : PageModel
    {
        private IFavoritosPresentacion IFavoritosPresentacion;

        public MisFavoritosModel(IFavoritosPresentacion IFavoritosPresentacion)
        {
            this.IFavoritosPresentacion = IFavoritosPresentacion;
        }

        public List<Favoritos> Lista { get; set; } = new();
        public string? Mensaje { get; set; }

        public IActionResult OnGet()
        {
            if (!EsCliente())
                return RedirectToPage("/Cuenta/Acceso", new { returnUrl = Request.Path.ToString() });
            Cargar();
            return Page();
        }

        public IActionResult OnPostBorrar(int idProducto)
        {
            if (!EsCliente()) return Forbid();
            try
            {
                this.IFavoritosPresentacion.Borrar(new Favoritos { Producto = idProducto });
                Mensaje = "Producto quitado de favoritos";
            }
            catch (Exception ex) { Mensaje = ex.Message; }
            Cargar();
            return Page();
        }

        private bool EsCliente()
        {
            return HttpContext.Session.GetString("Rol") == "Cliente" &&
                HttpContext.Session.GetInt32("Cliente").HasValue;
        }

        private void Cargar()
        {
            try { Lista = this.IFavoritosPresentacion.Consultar(); }
            catch (Exception ex) { Mensaje = ex.Message; }
        }
    }
}
