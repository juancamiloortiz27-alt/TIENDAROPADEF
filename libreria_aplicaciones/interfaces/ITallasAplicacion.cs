using libreria_aplicaciones.entidades;

namespace libreria_aplicaciones.interfaces
{
    public interface ITallasAplicacion
    {
        List<Tallas> Consultar();
        List<Tallas> Buscar(Tallas entidad);
        Tallas Insertar(Tallas entidad);
        Tallas Actualizar(Tallas entidad);
        Tallas Borrar(Tallas entidad);
    }
}
