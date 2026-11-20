using libreria_aplicaciones.entidades;

namespace presentaciones_tienda.interfaces
{
    public interface IColoresPresentacion
    {
        List<Colores> Consultar();
        List<Colores> Buscar(Colores entidad);
        Colores Insertar(Colores entidad);
        Colores Actualizar(Colores entidad);
        Colores Borrar(Colores entidad);
    }
}
