using libreria_aplicaciones.entidades;

namespace presentaciones_tienda.interfaces
{
    public interface IDireccionesPresentacion
    {
        List<Direcciones> Consultar();
        Direcciones Insertar(Direcciones entidad);
        Direcciones Actualizar(Direcciones entidad);
        void Borrar(Direcciones entidad);
    }
}
