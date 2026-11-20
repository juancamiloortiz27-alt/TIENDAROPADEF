using libreria_aplicaciones.entidades;

namespace libreria_aplicaciones.interfaces
{
    public interface IColoresAplicacion
    {
        List<Colores> Consultar();
        List<Colores> Buscar(Colores entidad);
        Colores Insertar(Colores entidad);
        Colores Actualizar(Colores entidad);
        Colores Borrar(Colores entidad);
    }
}
