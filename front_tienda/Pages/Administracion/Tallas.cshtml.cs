using libreria_aplicaciones.entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using presentaciones_tienda.interfaces;

namespace front_tienda.Pages.Administracion
{
    public class TallasModel : PageModel
    {
        private ITallasPresentacion ITallasPresentacion;

        public TallasModel(ITallasPresentacion ITallasPresentacion)
        {
            this.ITallasPresentacion = ITallasPresentacion;
        }

        [BindProperty] public Tallas Entidad { get; set; } = new();
        [BindProperty] public Tallas Filtro { get; set; } = new();
        public List<Tallas> Lista { get; set; } = new();
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
            Entidad = new Tallas();
            Filtro = new Tallas();
            ModelState.Clear();
            Consultar();
            return Page();
        }

        public IActionResult OnPostBuscar()
        {
            if (!EsAdministrador()) return Forbid();
            try { Lista = this.ITallasPresentacion.Buscar(Filtro); }
            catch (Exception ex) { Mensaje = ex.Message; }
            return Page();
        }

        public IActionResult OnPostSeleccionar(int id)
        {
            if (!EsAdministrador()) return Forbid();
            Consultar();
            Entidad = Lista.FirstOrDefault(x => x.IdTalla == id) ?? new Tallas();
            return Page();
        }

        public IActionResult OnPostInsertar()
        {
            if (!EsAdministrador()) return Forbid();
            try
            {
                this.ITallasPresentacion.Insertar(Entidad);
                Mensaje = "Talla insertada";
                Entidad = new Tallas();
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
                this.ITallasPresentacion.Actualizar(Entidad);
                Mensaje = "Talla actualizada";
                Entidad = new Tallas();
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
                this.ITallasPresentacion.Borrar(new Tallas { IdTalla = id });
                Mensaje = "Talla borrada";
                Entidad = new Tallas();
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
            try { Lista = this.ITallasPresentacion.Consultar(); }
            catch (Exception ex) { Mensaje = ex.Message; }
        }
    }
}
