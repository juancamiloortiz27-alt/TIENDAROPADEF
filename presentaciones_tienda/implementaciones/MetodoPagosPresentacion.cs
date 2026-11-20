using libreria_aplicaciones.entidades;
using presentaciones_tienda.interfaces;

namespace presentaciones_tienda.implementaciones
{
    public class MetodoPagosPresentacion : IMetodoPagosPresentacion
    {
        private IComunicaciones IComunicaciones;

        public MetodoPagosPresentacion(IComunicaciones IComunicaciones)
        {
            this.IComunicaciones = IComunicaciones;
        }

        public List<MetodoPagos> Consultar()
        {
            var task = this.IComunicaciones.Ejecutar<List<MetodoPagos>>("MetodoPagos/Consultar", null);
            task.Wait();
            return task.Result;
        }

        public List<MetodoPagos> ConsultarActivos()
        {
            var task = this.IComunicaciones.Ejecutar<List<MetodoPagos>>("MetodoPagos/ConsultarActivos", null);
            task.Wait();
            return task.Result;
        }

        public List<MetodoPagos> Buscar(MetodoPagos entidad)
        {
            var task = this.IComunicaciones.Ejecutar<List<MetodoPagos>>("MetodoPagos/Buscar", entidad);
            task.Wait();
            return task.Result;
        }

        public MetodoPagos Insertar(MetodoPagos entidad)
        {
            var task = this.IComunicaciones.Ejecutar<MetodoPagos>("MetodoPagos/Insertar", entidad);
            task.Wait();
            return task.Result;
        }

        public MetodoPagos Actualizar(MetodoPagos entidad)
        {
            var task = this.IComunicaciones.Ejecutar<MetodoPagos>("MetodoPagos/Actualizar", entidad);
            task.Wait();
            return task.Result;
        }

        public MetodoPagos Borrar(MetodoPagos entidad)
        {
            var task = this.IComunicaciones.Ejecutar<MetodoPagos>("MetodoPagos/Borrar", entidad);
            task.Wait();
            return task.Result;
        }
    }
}
