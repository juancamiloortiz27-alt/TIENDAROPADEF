using libreria_aplicaciones.entidades;

namespace presentaciones_tienda.interfaces
{
    public interface ICarritosPresentacion
    {
        CarritoPublico Consultar();
        CarritoPublico AgregarProducto(DetalleCarritos entidad);
        CarritoPublico ActualizarProducto(DetalleCarritos entidad);
        CarritoPublico BorrarProducto(int idDetalleCarrito);
    }
}
