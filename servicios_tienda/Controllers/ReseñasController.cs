using libreria_aplicaciones.entidades;
using libreria_aplicaciones.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace servicios_tienda.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ReseñasController : ControllerBase
    {
        private IReseñasAplicacion IReseñasAplicacion;

        public ReseñasController(IReseñasAplicacion IReseñasAplicacion)
        {
            this.IReseñasAplicacion = IReseñasAplicacion;
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult ConsultarPorProducto(Reseñas entidad)
        {
            try { return Ok(this.IReseñasAplicacion.ConsultarPorProducto(entidad.Producto)); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost]
        [Authorize(Roles = "Cliente")]
        public IActionResult Insertar(Reseñas entidad)
        {
            try
            {
                var cliente = User.FindFirst("Cliente")?.Value;
                if (!int.TryParse(cliente, out var idCliente))
                    throw new Exception("La sesión no tiene un cliente asociado");
                return Ok(this.IReseñasAplicacion.Insertar(idCliente, entidad));
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public IActionResult Borrar(Reseñas entidad)
        {
            try { return Ok(this.IReseñasAplicacion.Borrar(entidad)); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }
    }
}
