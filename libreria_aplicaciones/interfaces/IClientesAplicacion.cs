using libreria_aplicaciones.entidades;

namespace libreria_aplicaciones.interfaces
{
    public interface IClientesAplicacion
    {
        Clientes ConsultarPerfil(int idCliente);
        Clientes ActualizarPerfil(int idCliente, Clientes entidad);
    }
}
