using libreria_aplicaciones.entidades;
using libreria_aplicaciones.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace servicios_tienda.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Authorize(Roles = "Administrador")]
    public class CuponesController : ControllerBase
    {
        private ICuponesAplicacion ICuponesAplicacion;

        public CuponesController(ICuponesAplicacion ICuponesAplicacion)
        {
            this.ICuponesAplicacion = ICuponesAplicacion;
        }

        [HttpPost]
        public List<Cupones> Consultar() => this.ICuponesAplicacion.Consultar();

        [HttpPost]
        public List<Cupones> Buscar(Cupones entidad) => this.ICuponesAplicacion.Buscar(entidad);

        [HttpPost]
        public Cupones Insertar(Cupones entidad) => this.ICuponesAplicacion.Insertar(entidad);

        [HttpPost]
        public Cupones Actualizar(Cupones entidad) => this.ICuponesAplicacion.Actualizar(entidad);

        [HttpPost]
        public Cupones Borrar(Cupones entidad) => this.ICuponesAplicacion.Borrar(entidad);
    }
}
