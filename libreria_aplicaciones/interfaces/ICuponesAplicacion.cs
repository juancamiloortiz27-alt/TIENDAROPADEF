using libreria_aplicaciones.entidades;

namespace libreria_aplicaciones.interfaces
{
    public interface ICuponesAplicacion
    {
        List<Cupones> Consultar();
        List<Cupones> Buscar(Cupones entidad);
        Cupones Insertar(Cupones entidad);
        Cupones Actualizar(Cupones entidad);
        Cupones Borrar(Cupones entidad);
    }
}
