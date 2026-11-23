using libreria_aplicaciones.entidades;
using libreria_aplicaciones.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace servicios_tienda.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Authorize(Roles = "Cliente")]
    public class CarritosController : ControllerBase
    {
        private ICarritosAplicacion ICarritosAplicacion;

        public CarritosController(ICarritosAplicacion ICarritosAplicacion)
        {
            this.ICarritosAplicacion = ICarritosAplicacion;
        }

        [HttpPost]
        public IActionResult Consultar()
        {
            try { return Ok(this.ICarritosAplicacion.Consultar(ObtenerIdCliente())); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost]
        public IActionResult AgregarProducto(DetalleCarritos entidad)
        {
            try { return Ok(this.ICarritosAplicacion.AgregarProducto(ObtenerIdCliente(), entidad)); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost]
        public IActionResult ActualizarProducto(DetalleCarritos entidad)
        {
            try { return Ok(this.ICarritosAplicacion.ActualizarProducto(ObtenerIdCliente(), entidad)); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost]
        public IActionResult BorrarProducto([FromBody] int idDetalleCarrito)
        {
            try { return Ok(this.ICarritosAplicacion.BorrarProducto(ObtenerIdCliente(), idDetalleCarrito)); }
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
