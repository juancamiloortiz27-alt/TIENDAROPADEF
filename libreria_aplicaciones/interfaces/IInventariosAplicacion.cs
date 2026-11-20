using libreria_aplicaciones.entidades;

namespace libreria_aplicaciones.interfaces
{
    public interface IInventariosAplicacion
    {
        List<Inventarios> Consultar();
        List<Inventarios> Buscar(Inventarios entidad);
        Inventarios Insertar(Inventarios entidad);
        Inventarios Actualizar(Inventarios entidad);
        Inventarios Borrar(Inventarios entidad);
    }
}
