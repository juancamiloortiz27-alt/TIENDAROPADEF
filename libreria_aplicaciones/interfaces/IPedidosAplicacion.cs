using libreria_aplicaciones.entidades;

namespace libreria_aplicaciones.interfaces
{
    public interface IPedidosAplicacion
    {
        List<Pedidos> ConsultarMios(int idCliente);
        CuponValidado ValidarCupon(int idCliente, string? codigoCupon);
        Pedidos Confirmar(int idCliente, ConfirmarPedido entidad);
    }
}
