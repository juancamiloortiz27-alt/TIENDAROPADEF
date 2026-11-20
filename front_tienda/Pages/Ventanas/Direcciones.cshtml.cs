using libreria_aplicaciones.entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using presentaciones_tienda.interfaces;

namespace front_tienda.Pages.Ventanas
{
    public class DireccionesModel : PageModel
    {
        private IDireccionesPresentacion IDireccionesPresentacion;

        public DireccionesModel(IDireccionesPresentacion IDireccionesPresentacion)
        {
            this.IDireccionesPresentacion = IDireccionesPresentacion;
        }

        public List<Direcciones> Direcciones { get; set; } = new();
        [BindProperty] public Direcciones Formulario { get; set; } = new();
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
                if (Formulario.IdDireccion == 0)
                {
                    this.IDireccionesPresentacion.Insertar(Formulario);
                    Mensaje = "Dirección guardada";
                }
                else
                {
                    this.IDireccionesPresentacion.Actualizar(Formulario);
                    Mensaje = "Dirección actualizada";
                }
                Formulario = new Direcciones();
            }
            catch (Exception ex) { Mensaje = ex.Message; Error = true; }
            Cargar();
            return Page();
        }

        public IActionResult OnPostEditar(int id)
        {
            if (!EsCliente()) return Forbid();
            try
            {
                Formulario = this.IDireccionesPresentacion.Consultar()
                    .FirstOrDefault(x => x.IdDireccion == id) ?? new Direcciones();
                if (Formulario.IdDireccion == 0) { Mensaje = "No se encontró esa dirección"; Error = true; }
            }
            catch (Exception ex) { Mensaje = ex.Message; Error = true; }
            Cargar();
            return Page();
        }

        public IActionResult OnPostBorrar(int id)
        {
            if (!EsCliente()) return Forbid();
            try
            {
                this.IDireccionesPresentacion.Borrar(new Direcciones { IdDireccion = id });
                Mensaje = "Dirección eliminada";
            }
            catch (Exception ex) { Mensaje = ex.Message; Error = true; }
            Formulario = new Direcciones();
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
            try { Direcciones = this.IDireccionesPresentacion.Consultar(); }
            catch (Exception ex) { Mensaje = ex.Message; Error = true; }
        }
    }
}
