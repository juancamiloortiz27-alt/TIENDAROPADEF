using libreria_aplicaciones.entidades;

namespace presentaciones_tienda.interfaces
{
    public interface IReseñasPresentacion
    {
        List<Reseñas> ConsultarPorProducto(int idProducto);
        Reseñas Insertar(Reseñas entidad);
        Reseñas Borrar(Reseñas entidad);
    }
}
