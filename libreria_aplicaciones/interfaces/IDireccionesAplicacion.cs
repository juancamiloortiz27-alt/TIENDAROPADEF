using libreria_aplicaciones.entidades;

namespace libreria_aplicaciones.interfaces
{
    public interface IDireccionesAplicacion
    {
        List<Direcciones> Consultar(int idCliente);
        Direcciones Insertar(int idCliente, Direcciones entidad);
        Direcciones Actualizar(int idCliente, Direcciones entidad);
        void Borrar(int idCliente, Direcciones entidad);
    }
}
