using libreria_aplicaciones.entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using presentaciones_tienda.interfaces;

namespace front_tienda.Pages.Administracion
{
    public class TransportadorasModel : PageModel
    {
        private ITransportadorasPresentacion ITransportadorasPresentacion;

        public TransportadorasModel(ITransportadorasPresentacion ITransportadorasPresentacion)
        {
            this.ITransportadorasPresentacion = ITransportadorasPresentacion;
        }

        [BindProperty] public Transportadoras Entidad { get; set; } = new();
        [BindProperty] public Transportadoras Filtro { get; set; } = new();
        public List<Transportadoras> Lista { get; set; } = new();
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
            Entidad = new Transportadoras();
            Filtro = new Transportadoras();
            ModelState.Clear();
            Consultar();
            return Page();
        }

        public IActionResult OnPostBuscar()
        {
            if (!EsAdministrador()) return Forbid();
            Entidad = new Transportadoras();
            try { Lista = this.ITransportadorasPresentacion.Buscar(Filtro); }
            catch (Exception ex) { Mensaje = ex.Message; }
            return Page();
        }

        public IActionResult OnPostSeleccionar(int id)
        {
            if (!EsAdministrador()) return Forbid();
            Consultar();
            Entidad = Lista.FirstOrDefault(x => x.IdTransportadoras == id) ?? new Transportadoras();
            return Page();
        }

        public IActionResult OnPostInsertar()
        {
            if (!EsAdministrador()) return Forbid();
            try
            {
                this.ITransportadorasPresentacion.Insertar(Entidad);
                Mensaje = "Transportadora guardada";
                Entidad = new Transportadoras();
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
                this.ITransportadorasPresentacion.Actualizar(Entidad);
                Mensaje = "Transportadora actualizada";
                Entidad = new Transportadoras();
                ModelState.Clear();
            }
            catch (Exception ex) { Mensaje = ex.Message; }
            Consultar();
            return Page();
        }

        public IActionResult OnPostBorrar(int id)
        {
            if (!EsAdministrador()) return Forbid();
            Entidad = new Transportadoras();
            try
            {
                this.ITransportadorasPresentacion.Borrar(new Transportadoras { IdTransportadoras = id });
                Mensaje = "Transportadora eliminada";
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
            try { Lista = this.ITransportadorasPresentacion.Consultar(); }
            catch (Exception ex) { Mensaje = ex.Message; }
        }
    }
}
