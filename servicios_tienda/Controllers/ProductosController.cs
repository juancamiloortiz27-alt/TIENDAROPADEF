using libreria_aplicaciones.entidades;
using libreria_aplicaciones.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace servicios_tienda.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ProductosController : ControllerBase
    {
        private IProductosAplicacion IProductosAplicacion;

        public ProductosController(IProductosAplicacion IProductosAplicacion)
        {
            this.IProductosAplicacion = IProductosAplicacion;
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public List<Productos> Consultar()
        {
            return this.IProductosAplicacion.Consultar();
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public List<Productos> Buscar(Productos entidad)
        {
            return this.IProductosAplicacion.Buscar(entidad);
        }

        [HttpPost]
        [AllowAnonymous]
        public List<ProductoPublico> ConsultarPublicos()
        {
            return this.IProductosAplicacion.ConsultarPublicos();
        }

        [HttpPost]
        [AllowAnonymous]
        public List<ProductoPublico> BuscarPublicos(Productos entidad)
        {
            return this.IProductosAplicacion.BuscarPublicos(entidad);
        }

        [HttpPost]
        [AllowAnonymous]
        public List<ProductoPublico> ConsultarVariantesPublicas(Productos entidad)
        {
            return this.IProductosAplicacion.ConsultarVariantesPublicas(entidad.CodigoModelo ?? "");
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public Productos Insertar(Productos entidad)
        {
            return this.IProductosAplicacion.Insertar(entidad);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public Productos Actualizar(Productos entidad)
        {
            return this.IProductosAplicacion.Actualizar(entidad);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public Productos Borrar(Productos entidad)
        {
            return this.IProductosAplicacion.Borrar(entidad);
        }
    }
}
