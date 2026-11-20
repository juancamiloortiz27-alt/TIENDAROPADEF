using libreria_aplicaciones.entidades;

namespace libreria_aplicaciones.interfaces
{
    public interface ITransportadorasAplicacion
    {
        List<Transportadoras> Consultar();
        List<Transportadoras> Buscar(Transportadoras entidad);
        Transportadoras Insertar(Transportadoras entidad);
        Transportadoras Actualizar(Transportadoras entidad);
        Transportadoras Borrar(Transportadoras entidad);
    }
}
