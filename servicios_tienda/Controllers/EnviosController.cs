using libreria_aplicaciones.entidades;
using libreria_aplicaciones.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace servicios_tienda.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Authorize(Roles = "Administrador")]
    public class EnviosController : ControllerBase
    {
        private IEnviosAplicacion IEnviosAplicacion;

        public EnviosController(IEnviosAplicacion IEnviosAplicacion)
        {
            this.IEnviosAplicacion = IEnviosAplicacion;
        }

        [HttpPost]
        public List<Envios> Consultar() => this.IEnviosAplicacion.Consultar();

        [HttpPost]
        public List<Pedidos> ConsultarPedidosSinEnvio() => this.IEnviosAplicacion.ConsultarPedidosSinEnvio();

        [HttpPost]
        public Envios Insertar(Envios entidad) => this.IEnviosAplicacion.Insertar(entidad);

        [HttpPost]
        public Envios ActualizarEstado(Envios entidad) => this.IEnviosAplicacion.ActualizarEstado(entidad);
    }
}
