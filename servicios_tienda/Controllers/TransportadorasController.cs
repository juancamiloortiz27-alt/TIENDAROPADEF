using libreria_aplicaciones.entidades;
using libreria_aplicaciones.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace servicios_tienda.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Authorize(Roles = "Administrador")]
    public class TransportadorasController : ControllerBase
    {
        private ITransportadorasAplicacion ITransportadorasAplicacion;

        public TransportadorasController(ITransportadorasAplicacion ITransportadorasAplicacion)
        {
            this.ITransportadorasAplicacion = ITransportadorasAplicacion;
        }

        [HttpPost]
        public List<Transportadoras> Consultar() => this.ITransportadorasAplicacion.Consultar();

        [HttpPost]
        public List<Transportadoras> Buscar(Transportadoras entidad) => this.ITransportadorasAplicacion.Buscar(entidad);

        [HttpPost]
        public Transportadoras Insertar(Transportadoras entidad) => this.ITransportadorasAplicacion.Insertar(entidad);

        [HttpPost]
        public Transportadoras Actualizar(Transportadoras entidad) => this.ITransportadorasAplicacion.Actualizar(entidad);

        [HttpPost]
        public Transportadoras Borrar(Transportadoras entidad) => this.ITransportadorasAplicacion.Borrar(entidad);
    }
}
