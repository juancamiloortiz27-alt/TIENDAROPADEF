using libreria_aplicaciones.entidades;
using presentaciones_tienda.interfaces;

namespace presentaciones_tienda.implementaciones
{
    public class EnviosPresentacion : IEnviosPresentacion
    {
        private IComunicaciones IComunicaciones;

        public EnviosPresentacion(IComunicaciones IComunicaciones)
        {
            this.IComunicaciones = IComunicaciones;
        }

        public List<Envios> Consultar()
        {
            var task = this.IComunicaciones.Ejecutar<List<Envios>>("Envios/Consultar", null);
            task.Wait();
            return task.Result;
        }

        public List<Pedidos> ConsultarPedidosSinEnvio()
        {
            var task = this.IComunicaciones.Ejecutar<List<Pedidos>>("Envios/ConsultarPedidosSinEnvio", null);
            task.Wait();
            return task.Result;
        }

        public Envios Insertar(Envios entidad)
        {
            var task = this.IComunicaciones.Ejecutar<Envios>("Envios/Insertar", entidad);
            task.Wait();
            return task.Result;
        }

        public Envios ActualizarEstado(Envios entidad)
        {
            var task = this.IComunicaciones.Ejecutar<Envios>("Envios/ActualizarEstado", entidad);
            task.Wait();
            return task.Result;
        }
    }
}
