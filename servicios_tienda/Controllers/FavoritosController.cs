using libreria_aplicaciones.entidades;
using libreria_aplicaciones.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace servicios_tienda.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Authorize(Roles = "Cliente")]
    public class FavoritosController : ControllerBase
    {
        private IFavoritosAplicacion IFavoritosAplicacion;

        public FavoritosController(IFavoritosAplicacion IFavoritosAplicacion)
        {
            this.IFavoritosAplicacion = IFavoritosAplicacion;
        }

        [HttpPost]
        public IActionResult Consultar()
        {
            try { return Ok(this.IFavoritosAplicacion.Consultar(ObtenerIdCliente())); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost]
        public IActionResult Agregar(Favoritos entidad)
        {
            try { return Ok(this.IFavoritosAplicacion.Agregar(ObtenerIdCliente(), entidad)); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost]
        public IActionResult Borrar(Favoritos entidad)
        {
            try { return Ok(this.IFavoritosAplicacion.Borrar(ObtenerIdCliente(), entidad)); }
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
