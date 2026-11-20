using libreria_aplicaciones.entidades;

namespace presentaciones_tienda.interfaces
{
    public interface IProveedoresPresentacion
    {
        List<Proveedores> Consultar();
        List<Proveedores> Buscar(Proveedores entidad);
        Proveedores Insertar(Proveedores entidad);
        Proveedores Actualizar(Proveedores entidad);
        Proveedores Borrar(Proveedores entidad);
    }
}
