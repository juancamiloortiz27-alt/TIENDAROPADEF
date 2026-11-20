using libreria_aplicaciones.entidades;
using libreria_aplicaciones.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace servicios_tienda.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class MarcasController : ControllerBase
    {
        private IMarcasAplicacion IMarcasAplicacion;

        public MarcasController(IMarcasAplicacion IMarcasAplicacion)
        {
            this.IMarcasAplicacion = IMarcasAplicacion;
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public List<Marcas> Consultar()
        {
            return this.IMarcasAplicacion.Consultar();
        }

        [HttpPost]
        [AllowAnonymous]
        public List<Marcas> ConsultarActivas()
        {
            return this.IMarcasAplicacion.ConsultarActivas();
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public List<Marcas> Buscar(Marcas entidad)
        {
            return this.IMarcasAplicacion.Buscar(entidad);
        }

        [HttpPost]
        [AllowAnonymous]
        public List<Marcas> BuscarActivas(Marcas entidad)
        {
            return this.IMarcasAplicacion.BuscarActivas(entidad);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public Marcas Insertar(Marcas entidad)
        {
            return this.IMarcasAplicacion.Insertar(entidad);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public Marcas Actualizar(Marcas entidad)
        {
            return this.IMarcasAplicacion.Actualizar(entidad);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public Marcas Borrar(Marcas entidad)
        {
            return this.IMarcasAplicacion.Borrar(entidad);
        }
    }
}
