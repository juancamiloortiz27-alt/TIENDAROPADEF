using libreria_aplicaciones.entidades;
using libreria_aplicaciones.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace servicios_tienda.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Authorize(Roles = "Administrador")]
    public class TallasController : ControllerBase
    {
        private ITallasAplicacion ITallasAplicacion;

        public TallasController(ITallasAplicacion ITallasAplicacion)
        {
            this.ITallasAplicacion = ITallasAplicacion;
        }

        [HttpPost]
        public List<Tallas> Consultar()
        {
            return this.ITallasAplicacion.Consultar();
        }

        [HttpPost]
        public List<Tallas> Buscar(Tallas entidad)
        {
            return this.ITallasAplicacion.Buscar(entidad);
        }

        [HttpPost]
        public Tallas Insertar(Tallas entidad)
        {
            return this.ITallasAplicacion.Insertar(entidad);
        }

        [HttpPost]
        public Tallas Actualizar(Tallas entidad)
        {
            return this.ITallasAplicacion.Actualizar(entidad);
        }

        [HttpPost]
        public Tallas Borrar(Tallas entidad)
        {
            return this.ITallasAplicacion.Borrar(entidad);
        }
    }
}
