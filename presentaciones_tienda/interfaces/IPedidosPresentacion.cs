using libreria_aplicaciones.entidades;

namespace presentaciones_tienda.interfaces
{
    public interface IPedidosPresentacion
    {
        List<Pedidos> ConsultarMios();
        CuponValidado ValidarCupon(string codigoCupon);
        Pedidos Confirmar(ConfirmarPedido entidad);
    }
}
