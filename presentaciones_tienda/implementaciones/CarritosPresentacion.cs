using libreria_aplicaciones.entidades;
using presentaciones_tienda.interfaces;

namespace presentaciones_tienda.implementaciones
{
    public class CarritosPresentacion : ICarritosPresentacion
    {
        private IComunicaciones IComunicaciones;

        public CarritosPresentacion(IComunicaciones IComunicaciones)
        {
            this.IComunicaciones = IComunicaciones;
        }

        public CarritoPublico Consultar()
        {
            var task = this.IComunicaciones.Ejecutar<CarritoPublico>("Carritos/Consultar", null);
            task.Wait();
            return task.Result;
        }

        public CarritoPublico AgregarProducto(AgregarProductoCarrito entidad)
        {
            if (entidad.IdProducto == 0 || entidad.Cantidad <= 0)
                throw new Exception("Selecciona un producto y una cantidad válida");
            var task = this.IComunicaciones.Ejecutar<CarritoPublico>("Carritos/AgregarProducto", entidad);
            task.Wait();
            return task.Result;
        }

        public CarritoPublico ActualizarProducto(ActualizarDetalleCarrito entidad)
        {
            if (entidad.IdDetalleCarrito == 0 || entidad.Cantidad <= 0)
                throw new Exception("La cantidad debe ser mayor que cero");
            var task = this.IComunicaciones.Ejecutar<CarritoPublico>("Carritos/ActualizarProducto", entidad);
            task.Wait();
            return task.Result;
        }

        public CarritoPublico BorrarProducto(EliminarDetalleCarrito entidad)
        {
            if (entidad.IdDetalleCarrito == 0)
                throw new Exception("No se ha seleccionado un producto");
            var task = this.IComunicaciones.Ejecutar<CarritoPublico>("Carritos/BorrarProducto", entidad);
            task.Wait();
            return task.Result;
        }
    }
}
