using libreria_aplicaciones.entidades;

namespace presentaciones_tienda.interfaces
{
    public interface IInventariosPresentacion
    {
        List<Inventarios> Consultar();
        List<Inventarios> Buscar(Inventarios entidad);
        Inventarios Insertar(Inventarios entidad);
        Inventarios Actualizar(Inventarios entidad);
        Inventarios Borrar(Inventarios entidad);
    }
}
