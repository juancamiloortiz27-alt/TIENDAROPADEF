using libreria_aplicaciones.entidades;
using presentaciones_tienda.interfaces;

namespace presentaciones_tienda.implementaciones
{
    public class DireccionesPresentacion : IDireccionesPresentacion
    {
        private IComunicaciones IComunicaciones;

        public DireccionesPresentacion(IComunicaciones IComunicaciones)
        {
            this.IComunicaciones = IComunicaciones;
        }

        public List<Direcciones> Consultar()
        {
            var task = this.IComunicaciones.Ejecutar<List<Direcciones>>("Direcciones/Consultar", null);
            task.Wait();
            return task.Result;
        }

        public Direcciones Insertar(Direcciones entidad)
        {
            var task = this.IComunicaciones.Ejecutar<Direcciones>("Direcciones/Insertar", entidad);
            task.Wait();
            return task.Result;
        }

        public Direcciones Actualizar(Direcciones entidad)
        {
            var task = this.IComunicaciones.Ejecutar<Direcciones>("Direcciones/Actualizar", entidad);
            task.Wait();
            return task.Result;
        }

        public void Borrar(Direcciones entidad)
        {
            var task = this.IComunicaciones.Ejecutar<bool>("Direcciones/Borrar", entidad);
            task.Wait();
        }
    }
}
