using libreria_aplicaciones.entidades;
using presentaciones_tienda.interfaces;

namespace presentaciones_tienda.implementaciones
{
    public class ClientesPresentacion : IClientesPresentacion
    {
        private IComunicaciones IComunicaciones;

        public ClientesPresentacion(IComunicaciones IComunicaciones)
        {
            this.IComunicaciones = IComunicaciones;
        }

        public Clientes ConsultarPerfil()
        {
            var task = this.IComunicaciones.Ejecutar<Clientes>("Clientes/ConsultarPerfil", null);
            task.Wait();
            return task.Result;
        }

        public Clientes ActualizarPerfil(Clientes entidad)
        {
            var task = this.IComunicaciones.Ejecutar<Clientes>("Clientes/ActualizarPerfil", entidad);
            task.Wait();
            return task.Result;
        }
    }
}
