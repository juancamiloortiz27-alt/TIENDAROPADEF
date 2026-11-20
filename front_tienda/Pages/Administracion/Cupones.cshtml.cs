using libreria_aplicaciones.entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using presentaciones_tienda.interfaces;

namespace front_tienda.Pages.Administracion
{
    public class CuponesModel : PageModel
    {
        private ICuponesPresentacion ICuponesPresentacion;

        public CuponesModel(ICuponesPresentacion ICuponesPresentacion)
        {
            this.ICuponesPresentacion = ICuponesPresentacion;
        }

        [BindProperty] public Cupones Entidad { get; set; } = new();
        [BindProperty] public Cupones Filtro { get; set; } = new();
        public List<Cupones> Lista { get; set; } = new();
        public string? Mensaje { get; set; }

        public IActionResult OnGet()
        {
            if (!EsAdministrador()) return Forbid();
            NuevoCupon();
            Consultar();
            return Page();
        }

        public IActionResult OnPostLimpiar()
        {
            if (!EsAdministrador()) return Forbid();
            NuevoCupon();
            Filtro = new Cupones();
            ModelState.Clear();
            Consultar();
            return Page();
        }

        public IActionResult OnPostBuscar()
        {
            if (!EsAdministrador()) return Forbid();
            NuevoCupon();
            try { Lista = this.ICuponesPresentacion.Buscar(Filtro); }
            catch (Exception ex) { Mensaje = ex.Message; }
            return Page();
        }

        public IActionResult OnPostSeleccionar(int id)
        {
            if (!EsAdministrador()) return Forbid();
            Consultar();
            Entidad = Lista.FirstOrDefault(x => x.IdCupon == id) ?? Entidad;
            return Page();
        }

        public IActionResult OnPostInsertar()
        {
            if (!EsAdministrador()) return Forbid();
            try
            {
                this.ICuponesPresentacion.Insertar(Entidad);
                Mensaje = "Cupón creado";
                NuevoCupon();
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
                this.ICuponesPresentacion.Actualizar(Entidad);
                Mensaje = "Cupón actualizado";
                NuevoCupon();
                ModelState.Clear();
            }
            catch (Exception ex) { Mensaje = ex.Message; }
            Consultar();
            return Page();
        }

        public IActionResult OnPostBorrar(int id)
        {
            if (!EsAdministrador()) return Forbid();
            NuevoCupon();
            try
            {
                this.ICuponesPresentacion.Borrar(new Cupones { IdCupon = id });
                Mensaje = "Cupón eliminado";
                NuevoCupon();
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

        private void NuevoCupon()
        {
            Entidad = new Cupones
            {
                FechaVence = DateTime.Now.AddDays(7),
                Estado = true
            };
        }

        private void Consultar()
        {
            try { Lista = this.ICuponesPresentacion.Consultar(); }
            catch (Exception ex) { Mensaje = ex.Message; }
        }
    }
}
