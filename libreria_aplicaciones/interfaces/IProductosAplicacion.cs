using libreria_aplicaciones.entidades;

namespace libreria_aplicaciones.interfaces
{
    public interface IProductosAplicacion
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
