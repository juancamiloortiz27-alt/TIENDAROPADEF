using libreria_aplicaciones.interfaces;
using libreria_aplicaciones.entidades;

namespace libreria_aplicaciones.implementaciones
{
    public class AuditoriasAplicacion
    {
        private IConexion IConexion;

        public AuditoriasAplicacion(IConexion IConexion)
        {
            this.IConexion = IConexion;
        }

        public List<Auditorias> Consultar()
        {
            return this.IConexion.Auditorias!
                .OrderByDescending(x => x.FechaHoraUtc)
                .Take(200)
                .ToList();
        }
    }
}
