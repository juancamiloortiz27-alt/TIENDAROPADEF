using libreria_aplicaciones.entidades;

namespace presentaciones_tienda.interfaces
{
    public interface IEnviosPresentacion
    {
        List<Envios> Consultar();
        List<Pedidos> ConsultarPedidosSinEnvio();
        Envios Insertar(Envios entidad);
        Envios ActualizarEstado(Envios entidad);
    }
}
