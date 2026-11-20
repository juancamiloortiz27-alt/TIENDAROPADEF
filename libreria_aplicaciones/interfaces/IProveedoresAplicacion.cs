using libreria_aplicaciones.entidades;

namespace libreria_aplicaciones.interfaces
{
    public interface IProveedoresAplicacion
    {
        List<Proveedores> Consultar();
        List<Proveedores> Buscar(Proveedores entidad);
        Proveedores Insertar(Proveedores entidad);
        Proveedores Actualizar(Proveedores entidad);
        Proveedores Borrar(Proveedores entidad);
    }
}
