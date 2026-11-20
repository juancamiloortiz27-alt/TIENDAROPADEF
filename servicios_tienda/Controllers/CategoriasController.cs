using libreria_aplicaciones.entidades;
using libreria_aplicaciones.interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace servicios_tienda.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class CategoriasController : ControllerBase
    {
        private ICategoriasAplicacion ICategoriasAplicacion;

        public CategoriasController(ICategoriasAplicacion ICategoriasAplicacion)
        {
            this.ICategoriasAplicacion = ICategoriasAplicacion;
        }

        [HttpPost]
        [AllowAnonymous]
        public List<Categorias> Consultar()
        {
            return this.ICategoriasAplicacion.Consultar();
        }

        [HttpPost]
        [AllowAnonymous]
        public List<Categorias> Buscar(Categorias entidad)
        {
            return this.ICategoriasAplicacion.Buscar(entidad);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public Categorias Insertar(Categorias entidad)
        {
            return this.ICategoriasAplicacion.Insertar(entidad);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public Categorias Actualizar(Categorias entidad)
        {
            return this.ICategoriasAplicacion.Actualizar(entidad);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public Categorias Borrar(Categorias entidad)
        {
            return this.ICategoriasAplicacion.Borrar(entidad);
        }
    }
}
