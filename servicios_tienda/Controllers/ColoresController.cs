using libreria_aplicaciones.entidades;
using libreria_aplicaciones.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace servicios_tienda.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Authorize(Roles = "Administrador")]
    public class ColoresController : ControllerBase
    {
        private IColoresAplicacion IColoresAplicacion;

        public ColoresController(IColoresAplicacion IColoresAplicacion)
        {
            this.IColoresAplicacion = IColoresAplicacion;
        }

        [HttpPost]
        public List<Colores> Consultar()
        {
            return this.IColoresAplicacion.Consultar();
        }

        [HttpPost]
        public List<Colores> Buscar(Colores entidad)
        {
            return this.IColoresAplicacion.Buscar(entidad);
        }

        [HttpPost]
        public Colores Insertar(Colores entidad)
        {
            return this.IColoresAplicacion.Insertar(entidad);
        }

        [HttpPost]
        public Colores Actualizar(Colores entidad)
        {
            return this.IColoresAplicacion.Actualizar(entidad);
        }

        [HttpPost]
        public Colores Borrar(Colores entidad)
        {
            return this.IColoresAplicacion.Borrar(entidad);
        }
    }
}
