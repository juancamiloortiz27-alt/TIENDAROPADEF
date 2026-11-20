using libreria_aplicaciones.entidades;

namespace presentaciones_tienda.interfaces
{
    public interface ITransportadorasPresentacion
    {
        List<Transportadoras> Consultar();
        List<Transportadoras> Buscar(Transportadoras entidad);
        Transportadoras Insertar(Transportadoras entidad);
        Transportadoras Actualizar(Transportadoras entidad);
        Transportadoras Borrar(Transportadoras entidad);
    }
}
