using libreria_aplicaciones.entidades;
using libreria_aplicaciones.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace servicios_tienda.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Authorize(Roles = "Cliente")]
    public class ClientesController : ControllerBase
    {
        private IClientesAplicacion IClientesAplicacion;

        public ClientesController(IClientesAplicacion IClientesAplicacion)
        {
            this.IClientesAplicacion = IClientesAplicacion;
        }

        [HttpPost]
        public IActionResult ConsultarPerfil()
        {
            try { return Ok(this.IClientesAplicacion.ConsultarPerfil(ObtenerIdCliente())); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost]
        public IActionResult ActualizarPerfil(Clientes entidad)
        {
            try { return Ok(this.IClientesAplicacion.ActualizarPerfil(ObtenerIdCliente(), entidad)); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        private int ObtenerIdCliente()
        {
            var cliente = User.FindFirst("Cliente")?.Value;
            if (!int.TryParse(cliente, out var idCliente))
                throw new Exception("La sesión no tiene un cliente asociado");
            return idCliente;
        }
    }
}
