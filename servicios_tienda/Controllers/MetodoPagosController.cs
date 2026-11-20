using libreria_aplicaciones.entidades;
using libreria_aplicaciones.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace servicios_tienda.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class MetodoPagosController : ControllerBase
    {
        private IMetodoPagosAplicacion IMetodoPagosAplicacion;

        public MetodoPagosController(IMetodoPagosAplicacion IMetodoPagosAplicacion)
        {
            this.IMetodoPagosAplicacion = IMetodoPagosAplicacion;
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public List<MetodoPagos> Consultar() => this.IMetodoPagosAplicacion.Consultar();

        [HttpPost]
        [Authorize(Roles = "Cliente")]
        public List<MetodoPagos> ConsultarActivos() => this.IMetodoPagosAplicacion.ConsultarActivos();

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public List<MetodoPagos> Buscar(MetodoPagos entidad) => this.IMetodoPagosAplicacion.Buscar(entidad);

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public MetodoPagos Insertar(MetodoPagos entidad) => this.IMetodoPagosAplicacion.Insertar(entidad);

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public MetodoPagos Actualizar(MetodoPagos entidad) => this.IMetodoPagosAplicacion.Actualizar(entidad);

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public MetodoPagos Borrar(MetodoPagos entidad) => this.IMetodoPagosAplicacion.Borrar(entidad);
    }
}
