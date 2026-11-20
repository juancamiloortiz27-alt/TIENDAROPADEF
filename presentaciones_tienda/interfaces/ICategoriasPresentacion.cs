using libreria_aplicaciones.entidades;

namespace presentaciones_tienda.interfaces
{
    public interface ICategoriasPresentacion
    {
        List<Categorias> Consultar();
        List<Categorias> Buscar(Categorias entidad);
        Categorias Insertar(Categorias entidad);
        Categorias Actualizar(Categorias entidad);
        Categorias Borrar(Categorias entidad);
    }
}
