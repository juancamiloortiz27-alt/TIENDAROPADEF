using libreria_aplicaciones.entidades;

namespace presentaciones_tienda.interfaces
{
    public interface IFavoritosPresentacion
    {
        List<Favoritos> Consultar();
        Favoritos Agregar(Favoritos entidad);
        Favoritos Borrar(Favoritos entidad);
    }
}
