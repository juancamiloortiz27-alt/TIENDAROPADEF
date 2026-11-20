using libreria_aplicaciones.entidades;

namespace libreria_aplicaciones.interfaces
{
    public interface ICategoriasAplicacion
    {
        List<Categorias> Consultar();
        List<Categorias> Buscar(Categorias entidad);
        Categorias Insertar(Categorias entidad);
        Categorias Actualizar(Categorias entidad);
        Categorias Borrar(Categorias entidad);
    }
}
