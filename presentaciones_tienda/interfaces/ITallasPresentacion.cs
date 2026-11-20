using libreria_aplicaciones.entidades;

namespace presentaciones_tienda.interfaces
{
    public interface ITallasPresentacion
    {
        List<Tallas> Consultar();
        List<Tallas> Buscar(Tallas entidad);
        Tallas Insertar(Tallas entidad);
        Tallas Actualizar(Tallas entidad);
        Tallas Borrar(Tallas entidad);
    }
}
