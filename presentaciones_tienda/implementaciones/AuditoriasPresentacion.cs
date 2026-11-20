using libreria_aplicaciones.entidades;
using presentaciones_tienda.interfaces;

namespace presentaciones_tienda.implementaciones
{
    public class AuditoriasPresentacion : IAuditoriasPresentacion
    {
        private IComunicaciones IComunicaciones;

        public AuditoriasPresentacion(IComunicaciones IComunicaciones)
        {
            this.IComunicaciones = IComunicaciones;
        }

        public List<Auditorias> Consultar()
        {
            var task = this.IComunicaciones.Ejecutar<List<Auditorias>>("Auditorias/Consultar", null);
            task.Wait();
            return task.Result;
        }
    }
}
