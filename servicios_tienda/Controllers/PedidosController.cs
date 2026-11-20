using libreria_aplicaciones.entidades;
using libreria_aplicaciones.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace servicios_tienda.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Authorize(Roles = "Cliente")]
    public class PedidosController : ControllerBase
    {
        private IPedidosAplicacion IPedidosAplicacion;

        public PedidosController(IPedidosAplicacion IPedidosAplicacion)
        {
            this.IPedidosAplicacion = IPedidosAplicacion;
        }

        [HttpPost]
        public IActionResult ConsultarMios()
        {
            try
            {
                var cliente = User.FindFirst("Cliente")?.Value;
                if (!int.TryParse(cliente, out var idCliente))
                    throw new Exception("La sesión no tiene un cliente asociado");
                return Ok(this.IPedidosAplicacion.ConsultarMios(idCliente));
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost]
        public IActionResult ValidarCupon(ConfirmarPedido entidad)
        {
            try
            {
                var cliente = User.FindFirst("Cliente")?.Value;
                if (!int.TryParse(cliente, out var idCliente))
                    throw new Exception("La sesión no tiene un cliente asociado");
                return Ok(this.IPedidosAplicacion.ValidarCupon(idCliente, entidad.CodigoCupon));
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost]
        public IActionResult Confirmar(ConfirmarPedido entidad)
        {
            try
            {
                var cliente = User.FindFirst("Cliente")?.Value;
                if (!int.TryParse(cliente, out var idCliente))
                    throw new Exception("La sesión no tiene un cliente asociado");
                return Ok(this.IPedidosAplicacion.Confirmar(idCliente, entidad));
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }
    }
}
