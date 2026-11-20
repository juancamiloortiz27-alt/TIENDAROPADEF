using libreria_aplicaciones.entidades;
using libreria_aplicaciones.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace servicios_tienda.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Authorize(Roles = "Cliente")]
    public class DireccionesController : ControllerBase
    {
        private IDireccionesAplicacion IDireccionesAplicacion;

        public DireccionesController(IDireccionesAplicacion IDireccionesAplicacion)
        {
            this.IDireccionesAplicacion = IDireccionesAplicacion;
        }

        [HttpPost]
        public IActionResult Consultar()
        {
            try { return Ok(this.IDireccionesAplicacion.Consultar(ObtenerIdCliente())); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost]
        public IActionResult Insertar(Direcciones entidad)
        {
            try { return Ok(this.IDireccionesAplicacion.Insertar(ObtenerIdCliente(), entidad)); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost]
        public IActionResult Actualizar(Direcciones entidad)
        {
            try { return Ok(this.IDireccionesAplicacion.Actualizar(ObtenerIdCliente(), entidad)); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost]
        public IActionResult Borrar(Direcciones entidad)
        {
            try
            {
                this.IDireccionesAplicacion.Borrar(ObtenerIdCliente(), entidad);
                return Ok(true);
            }
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
