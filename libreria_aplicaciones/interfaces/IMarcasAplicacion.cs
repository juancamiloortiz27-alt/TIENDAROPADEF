using libreria_aplicaciones.entidades;

namespace libreria_aplicaciones.interfaces
{
    public interface IMarcasAplicacion
    {
        List<Marcas> Consultar();
        List<Marcas> ConsultarActivas();
        List<Marcas> Buscar(Marcas entidad);
        List<Marcas> BuscarActivas(Marcas entidad);
        Marcas Insertar(Marcas entidad);
        Marcas Actualizar(Marcas entidad);
        Marcas Borrar(Marcas entidad);
    }
}
