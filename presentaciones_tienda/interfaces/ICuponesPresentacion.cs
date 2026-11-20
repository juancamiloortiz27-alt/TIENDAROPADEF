using libreria_aplicaciones.entidades;

namespace presentaciones_tienda.interfaces
{
    public interface ICuponesPresentacion
    {
        List<Cupones> Consultar();
        List<Cupones> Buscar(Cupones entidad);
        Cupones Insertar(Cupones entidad);
        Cupones Actualizar(Cupones entidad);
        Cupones Borrar(Cupones entidad);
    }
}
