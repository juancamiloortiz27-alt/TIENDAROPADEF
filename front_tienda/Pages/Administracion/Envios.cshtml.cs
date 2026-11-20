using libreria_aplicaciones.entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using presentaciones_tienda.interfaces;

namespace front_tienda.Pages.Administracion
{
    public class EnviosModel : PageModel
    {
        private IEnviosPresentacion IEnviosPresentacion;
        private ITransportadorasPresentacion ITransportadorasPresentacion;

        public EnviosModel(IEnviosPresentacion IEnviosPresentacion,
            ITransportadorasPresentacion ITransportadorasPresentacion)
        {
            this.IEnviosPresentacion = IEnviosPresentacion;
            this.ITransportadorasPresentacion = ITransportadorasPresentacion;
        }

        [BindProperty] public Envios Entidad { get; set; } = new();
        [BindProperty] public string? EstadoActualizado { get; set; }
        public List<Envios> Lista { get; set; } = new();
        public List<Pedidos> PedidosSinEnvio { get; set; } = new();
        public List<Transportadoras> Transportadoras { get; set; } = new();
        public List<string> Estados { get; } = new() { "Preparando", "Enviado", "En tránsito", "Entregado" };
        public string? Mensaje { get; set; }

        public IActionResult OnGet()
        {
            if (!EsAdministrador()) return Forbid();
            Cargar();
            return Page();
        }

        public IActionResult OnPostCrear()
        {
            if (!EsAdministrador()) return Forbid();
            try
            {
                var envio = this.IEnviosPresentacion.Insertar(Entidad);
                Mensaje = $"Envío creado. Número de guía: {envio.NumeroGuia}";
                Entidad = new Envios();
                ModelState.Clear();
            }
            catch (Exception ex) { Mensaje = ex.Message; }
            Cargar();
            return Page();
        }

        public IActionResult OnPostActualizarEstado(int id, string estado)
        {
            if (!EsAdministrador()) return Forbid();
            try
            {
                this.IEnviosPresentacion.ActualizarEstado(new Envios
                {
                    IdEnvio = id,
                    Estado = estado
                });
                Mensaje = "Estado del envío actualizado";
            }
            catch (Exception ex) { Mensaje = ex.Message; }
            Cargar();
            return Page();
        }

        private bool EsAdministrador()
        {
            return HttpContext.Session.GetString("Rol") == "Administrador";
        }

        private void Cargar()
        {
            try { Lista = this.IEnviosPresentacion.Consultar(); }
            catch (Exception ex) { Mensaje = ex.Message; }
            try { PedidosSinEnvio = this.IEnviosPresentacion.ConsultarPedidosSinEnvio(); }
            catch (Exception ex) { Mensaje = ex.Message; }
            try { Transportadoras = this.ITransportadorasPresentacion.Consultar(); }
            catch (Exception ex) { Mensaje = ex.Message; }
        }
    }
}
