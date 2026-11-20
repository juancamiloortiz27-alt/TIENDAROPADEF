using libreria_aplicaciones.entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using presentaciones_tienda.interfaces;

namespace front_tienda.Pages.Administracion
{
    public class ColoresModel : PageModel
    {
        private IColoresPresentacion IColoresPresentacion;

        public ColoresModel(IColoresPresentacion IColoresPresentacion)
        {
            this.IColoresPresentacion = IColoresPresentacion;
        }

        [BindProperty] public Colores Entidad { get; set; } = new();
        [BindProperty] public Colores Filtro { get; set; } = new();
        public List<Colores> Lista { get; set; } = new();
        public string? Mensaje { get; set; }

        public IActionResult OnGet()
        {
            if (!EsAdministrador()) return Forbid();
            Consultar();
            return Page();
        }

        public IActionResult OnPostLimpiar()
        {
            if (!EsAdministrador()) return Forbid();
            Entidad = new Colores();
            Filtro = new Colores();
            ModelState.Clear();
            Consultar();
            return Page();
        }

        public IActionResult OnPostBuscar()
        {
            if (!EsAdministrador()) return Forbid();
            try { Lista = this.IColoresPresentacion.Buscar(Filtro); }
            catch (Exception ex) { Mensaje = ex.Message; }
            return Page();
        }

        public IActionResult OnPostSeleccionar(int id)
        {
            if (!EsAdministrador()) return Forbid();
            Consultar();
            Entidad = Lista.FirstOrDefault(x => x.IdColor == id) ?? new Colores();
            return Page();
        }

        public IActionResult OnPostInsertar()
        {
            if (!EsAdministrador()) return Forbid();
            try
            {
                this.IColoresPresentacion.Insertar(Entidad);
                Mensaje = "Color insertado";
                Entidad = new Colores();
                ModelState.Clear();
            }
            catch (Exception ex) { Mensaje = ex.Message; }
            Consultar();
            return Page();
        }

        public IActionResult OnPostActualizar()
        {
            if (!EsAdministrador()) return Forbid();
            try
            {
                this.IColoresPresentacion.Actualizar(Entidad);
                Mensaje = "Color actualizado";
                Entidad = new Colores();
                ModelState.Clear();
            }
            catch (Exception ex) { Mensaje = ex.Message; }
            Consultar();
            return Page();
        }

        public IActionResult OnPostBorrar(int id)
        {
            if (!EsAdministrador()) return Forbid();
            try
            {
                this.IColoresPresentacion.Borrar(new Colores { IdColor = id });
                Mensaje = "Color borrado";
                Entidad = new Colores();
                ModelState.Clear();
            }
            catch (Exception ex) { Mensaje = ex.Message; }
            Consultar();
            return Page();
        }

        private bool EsAdministrador()
        {
            return HttpContext.Session.GetString("Rol") == "Administrador";
        }

        private void Consultar()
        {
            try { Lista = this.IColoresPresentacion.Consultar(); }
            catch (Exception ex) { Mensaje = ex.Message; }
        }
    }
}
