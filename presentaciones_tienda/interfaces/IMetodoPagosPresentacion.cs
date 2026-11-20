using libreria_aplicaciones.entidades;

namespace presentaciones_tienda.interfaces
{
    public interface IMetodoPagosPresentacion
    {
        List<MetodoPagos> Consultar();
        List<MetodoPagos> ConsultarActivos();
        List<MetodoPagos> Buscar(MetodoPagos entidad);
        MetodoPagos Insertar(MetodoPagos entidad);
        MetodoPagos Actualizar(MetodoPagos entidad);
        MetodoPagos Borrar(MetodoPagos entidad);
    }
}
