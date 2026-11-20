using libreria_aplicaciones.entidades;
using presentaciones_tienda.interfaces;

namespace presentaciones_tienda.implementaciones
{
    public class ProductosPresentacion : IProductosPresentacion
    {
        private IComunicaciones IComunicaciones;

        public ProductosPresentacion(IComunicaciones IComunicaciones)
        {
            this.IComunicaciones = IComunicaciones;
        }

        public List<Productos> Consultar()
        {
            var task = this.IComunicaciones.Ejecutar<List<Productos>>("Productos/Consultar", null);
            task.Wait();
            return task.Result;
        }

        public List<Productos> Buscar(Productos entidad)
        {
            var task = this.IComunicaciones.Ejecutar<List<Productos>>("Productos/Buscar", entidad);
            task.Wait();
            return task.Result;
        }

        public List<ProductoPublico> ConsultarPublicos()
        {
            var task = this.IComunicaciones.Ejecutar<List<ProductoPublico>>("Productos/ConsultarPublicos", null);
            task.Wait();
            return task.Result;
        }

        public List<ProductoPublico> BuscarPublicos(Productos entidad)
        {
            var task = this.IComunicaciones.Ejecutar<List<ProductoPublico>>("Productos/BuscarPublicos", entidad);
            task.Wait();
            return task.Result;
        }

        public List<ProductoPublico> ConsultarVariantesPublicas(string codigoModelo)
        {
            var task = this.IComunicaciones.Ejecutar<List<ProductoPublico>>(
                "Productos/ConsultarVariantesPublicas", new Productos { CodigoModelo = codigoModelo });
            task.Wait();
            return task.Result;
        }

        public Productos Insertar(Productos entidad)
        {
            if (entidad.IdProducto != 0)
                throw new Exception("Ya se guardó");
            Validar(entidad);
            var task = this.IComunicaciones.Ejecutar<Productos>("Productos/Insertar", entidad);
            task.Wait();
            return task.Result;
        }

        public Productos Actualizar(Productos entidad)
        {
            if (entidad.IdProducto == 0)
                throw new Exception("No se ha guardado");
            Validar(entidad);
            var task = this.IComunicaciones.Ejecutar<Productos>("Productos/Actualizar", entidad);
            task.Wait();
            return task.Result;
        }

        public Productos Borrar(Productos entidad)
        {
            if (entidad.IdProducto == 0)
                throw new Exception("No se ha guardado");
            var task = this.IComunicaciones.Ejecutar<Productos>("Productos/Borrar", entidad);
            task.Wait();
            return task.Result;
        }

        private void Validar(Productos entidad)
        {
            if (string.IsNullOrWhiteSpace(entidad.CodigoModelo) ||
                entidad.CodigoModelo.Length > 40 ||
                string.IsNullOrEmpty(entidad.Nombre) ||
                string.IsNullOrEmpty(entidad.Descripcion) ||
                entidad.Precio <= 0 ||
                entidad.Categoria == 0 ||
                entidad.Marca == 0 ||
                entidad.Color == 0 ||
                entidad.Proveedor == 0 ||
                entidad.Talla == 0)
                throw new Exception("Falta información");
        }
    }
}
