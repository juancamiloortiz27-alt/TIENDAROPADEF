using libreria_aplicaciones.entidades;

namespace presentaciones_tienda.interfaces
{
    public interface IProductosPresentacion
    {
        List<Productos> Consultar();
        List<Productos> Buscar(Productos entidad);
        List<ProductoPublico> ConsultarPublicos();
        List<ProductoPublico> BuscarPublicos(Productos entidad);
        List<ProductoPublico> ConsultarVariantesPublicas(string codigoModelo);
        Productos Insertar(Productos entidad);
        Productos Actualizar(Productos entidad);
        Productos Borrar(Productos entidad);
    }
}
