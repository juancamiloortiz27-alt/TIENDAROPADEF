using libreria_aplicaciones.entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using presentaciones_tienda.interfaces;

namespace front_tienda.Pages.Administracion
{
    public class ProveedoresModel : PageModel
    {
        private IProveedoresPresentacion IProveedoresPresentacion;

        public ProveedoresModel(IProveedoresPresentacion IProveedoresPresentacion)
        {
            this.IProveedoresPresentacion = IProveedoresPresentacion;
        }

        [BindProperty] public Proveedores Entidad { get; set; } = new();
        [BindProperty] public Proveedores Filtro { get; set; } = new();
        public List<Proveedores> Lista { get; set; } = new();
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
            Entidad = new Proveedores();
            Filtro = new Proveedores();
            ModelState.Clear();
            Consultar();
            return Page();
        }

        public IActionResult OnPostBuscar()
        {
            if (!EsAdministrador()) return Forbid();
            try { Lista = this.IProveedoresPresentacion.Buscar(Filtro); }
            catch (Exception ex) { Mensaje = ex.Message; }
            return Page();
        }

        public IActionResult OnPostSeleccionar(int id)
        {
            if (!EsAdministrador()) return Forbid();
            Consultar();
            Entidad = Lista.FirstOrDefault(x => x.IdProveedor == id) ?? new Proveedores();
            return Page();
        }

        public IActionResult OnPostInsertar()
        {
            if (!EsAdministrador()) return Forbid();
            try
            {
                this.IProveedoresPresentacion.Insertar(Entidad);
                Mensaje = "Proveedor insertado";
                Entidad = new Proveedores();
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
                this.IProveedoresPresentacion.Actualizar(Entidad);
                Mensaje = "Proveedor actualizado";
                Entidad = new Proveedores();
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
                this.IProveedoresPresentacion.Borrar(new Proveedores { IdProveedor = id });
                Mensaje = "Proveedor borrado";
                Entidad = new Proveedores();
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
            try { Lista = this.IProveedoresPresentacion.Consultar(); }
            catch (Exception ex) { Mensaje = ex.Message; }
        }
    }
}
