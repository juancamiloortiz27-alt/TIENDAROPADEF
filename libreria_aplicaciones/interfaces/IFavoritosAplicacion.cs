using libreria_aplicaciones.entidades;

namespace libreria_aplicaciones.interfaces
{
    public interface IFavoritosAplicacion
    {
        List<Favoritos> Consultar(int idCliente);
        Favoritos Agregar(int idCliente, Favoritos entidad);
        Favoritos Borrar(int idCliente, Favoritos entidad);
    }
}
