using libreria_aplicaciones.entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using presentaciones_tienda.interfaces;

namespace front_tienda.Pages.Administracion
{
    public class MetodosPagoModel : PageModel
    {
        private IMetodoPagosPresentacion IMetodoPagosPresentacion;

        public MetodosPagoModel(IMetodoPagosPresentacion IMetodoPagosPresentacion)
        {
            this.IMetodoPagosPresentacion = IMetodoPagosPresentacion;
        }

        [BindProperty] public MetodoPagos Entidad { get; set; } = new() { Estado = true };
        [BindProperty] public MetodoPagos Filtro { get; set; } = new();
        public List<MetodoPagos> Lista { get; set; } = new();
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
            Entidad = new MetodoPagos { Estado = true };
            Filtro = new MetodoPagos();
            ModelState.Clear();
            Consultar();
            return Page();
        }

        public IActionResult OnPostBuscar()
        {
            if (!EsAdministrador()) return Forbid();
            Entidad = new MetodoPagos { Estado = true };
            try { Lista = this.IMetodoPagosPresentacion.Buscar(Filtro); }
            catch (Exception ex) { Mensaje = ex.Message; }
            return Page();
        }

        public IActionResult OnPostSeleccionar(int id)
        {
            if (!EsAdministrador()) return Forbid();
            Consultar();
            Entidad = Lista.FirstOrDefault(x => x.IdMetodoPago == id) ?? new MetodoPagos { Estado = true };
            return Page();
        }

        public IActionResult OnPostInsertar()
        {
            if (!EsAdministrador()) return Forbid();
            try
            {
                this.IMetodoPagosPresentacion.Insertar(Entidad);
                Mensaje = "Método de pago creado";
                Entidad = new MetodoPagos { Estado = true };
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
                this.IMetodoPagosPresentacion.Actualizar(Entidad);
                Mensaje = "Método de pago actualizado";
                Entidad = new MetodoPagos { Estado = true };
                ModelState.Clear();
            }
            catch (Exception ex) { Mensaje = ex.Message; }
            Consultar();
            return Page();
        }

        public IActionResult OnPostBorrar(int id)
        {
            if (!EsAdministrador()) return Forbid();
            Entidad = new MetodoPagos { Estado = true };
            try
            {
                this.IMetodoPagosPresentacion.Borrar(new MetodoPagos { IdMetodoPago = id });
                Mensaje = "Método de pago eliminado";
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
            try { Lista = this.IMetodoPagosPresentacion.Consultar(); }
            catch (Exception ex) { Mensaje = ex.Message; }
        }
    }
}
