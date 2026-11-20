using libreria_aplicaciones.entidades;

namespace libreria_aplicaciones.interfaces
{
    public interface IEnviosAplicacion
    {
        List<Envios> Consultar();
        List<Pedidos> ConsultarPedidosSinEnvio();
        Envios Insertar(Envios entidad);
        Envios ActualizarEstado(Envios entidad);
    }
}
