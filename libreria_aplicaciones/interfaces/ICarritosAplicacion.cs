using libreria_aplicaciones.entidades;

namespace libreria_aplicaciones.interfaces
{
    public interface ICarritosAplicacion
    {
        CarritoPublico Consultar(int idCliente);
        CarritoPublico AgregarProducto(int idCliente, DetalleCarritos entidad);
        CarritoPublico ActualizarProducto(int idCliente, DetalleCarritos entidad);
        CarritoPublico BorrarProducto(int idCliente, int idDetalleCarrito);
    }
}
