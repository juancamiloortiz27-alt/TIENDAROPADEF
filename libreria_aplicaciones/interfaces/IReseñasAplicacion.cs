using libreria_aplicaciones.entidades;

namespace libreria_aplicaciones.interfaces
{
    public interface IReseñasAplicacion
    {
        List<Reseñas> ConsultarPorProducto(int idProducto);
        Reseñas Insertar(int idCliente, Reseñas entidad);
        Reseñas Borrar(Reseñas entidad);
    }
}
