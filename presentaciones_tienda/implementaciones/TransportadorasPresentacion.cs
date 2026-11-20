using libreria_aplicaciones.entidades;
using presentaciones_tienda.interfaces;

namespace presentaciones_tienda.implementaciones
{
    public class TransportadorasPresentacion : ITransportadorasPresentacion
    {
        private IComunicaciones IComunicaciones;

        public TransportadorasPresentacion(IComunicaciones IComunicaciones)
        {
            this.IComunicaciones = IComunicaciones;
        }

        public List<Transportadoras> Consultar()
        {
            var task = this.IComunicaciones.Ejecutar<List<Transportadoras>>("Transportadoras/Consultar", null);
            task.Wait();
            return task.Result;
        }

        public List<Transportadoras> Buscar(Transportadoras entidad)
        {
            var task = this.IComunicaciones.Ejecutar<List<Transportadoras>>("Transportadoras/Buscar", entidad);
            task.Wait();
            return task.Result;
        }

        public Transportadoras Insertar(Transportadoras entidad)
        {
            var task = this.IComunicaciones.Ejecutar<Transportadoras>("Transportadoras/Insertar", entidad);
            task.Wait();
            return task.Result;
        }

        public Transportadoras Actualizar(Transportadoras entidad)
        {
            var task = this.IComunicaciones.Ejecutar<Transportadoras>("Transportadoras/Actualizar", entidad);
            task.Wait();
            return task.Result;
        }

        public Transportadoras Borrar(Transportadoras entidad)
        {
            var task = this.IComunicaciones.Ejecutar<Transportadoras>("Transportadoras/Borrar", entidad);
            task.Wait();
            return task.Result;
        }
    }
}
