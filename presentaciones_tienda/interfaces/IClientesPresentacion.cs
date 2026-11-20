using libreria_aplicaciones.entidades;

namespace presentaciones_tienda.interfaces
{
    public interface IClientesPresentacion
    {
        Clientes ConsultarPerfil();
        Clientes ActualizarPerfil(Clientes entidad);
    }
}
