using libreria_aplicaciones.entidades;
using libreria_aplicaciones.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace servicios_tienda.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Authorize(Roles = "Administrador")]
    public class AuditoriasController : ControllerBase
    {
        private IAuditoriasAplicacion IAuditoriasAplicacion;

        public AuditoriasController(IAuditoriasAplicacion IAuditoriasAplicacion)
        {
            this.IAuditoriasAplicacion = IAuditoriasAplicacion;
        }

        [HttpPost]
        public List<Auditorias> Consultar()
        {
            return this.IAuditoriasAplicacion.Consultar();
        }
    }
}
