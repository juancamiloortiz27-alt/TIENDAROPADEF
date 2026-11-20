using libreria_aplicaciones.entidades;

namespace libreria_aplicaciones.interfaces
{
    public interface IMetodoPagosAplicacion
    {
        List<MetodoPagos> Consultar();
        List<MetodoPagos> ConsultarActivos();
        List<MetodoPagos> Buscar(MetodoPagos entidad);
        MetodoPagos Insertar(MetodoPagos entidad);
        MetodoPagos Actualizar(MetodoPagos entidad);
        MetodoPagos Borrar(MetodoPagos entidad);
    }
}
