using libreria_aplicaciones.entidades;

namespace presentaciones_tienda.interfaces
{
    public interface ICarritosPresentacion
    {
        CarritoPublico Consultar();
        CarritoPublico AgregarProducto(AgregarProductoCarrito entidad);
        CarritoPublico ActualizarProducto(ActualizarDetalleCarrito entidad);
        CarritoPublico BorrarProducto(EliminarDetalleCarrito entidad);
    }
}
