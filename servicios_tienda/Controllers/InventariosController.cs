using libreria_aplicaciones.entidades;
using libreria_aplicaciones.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace servicios_tienda.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Authorize(Roles = "Administrador")]
    public class InventariosController : ControllerBase
    {
        private IInventariosAplicacion IInventariosAplicacion;

        public InventariosController(IInventariosAplicacion IInventariosAplicacion)
        {
            this.IInventariosAplicacion = IInventariosAplicacion;
        }

        [HttpPost]
        public List<Inventarios> Consultar()
        {
            return this.IInventariosAplicacion.Consultar();
        }

        [HttpPost]
        public List<Inventarios> Buscar(Inventarios entidad)
        {
            return this.IInventariosAplicacion.Buscar(entidad);
        }

        [HttpPost]
        public Inventarios Insertar(Inventarios entidad)
        {
            return this.IInventariosAplicacion.Insertar(entidad);
        }

        [HttpPost]
        public Inventarios Actualizar(Inventarios entidad)
        {
            return this.IInventariosAplicacion.Actualizar(entidad);
        }

        [HttpPost]
        public Inventarios Borrar(Inventarios entidad)
        {
            return this.IInventariosAplicacion.Borrar(entidad);
        }
    }
}
