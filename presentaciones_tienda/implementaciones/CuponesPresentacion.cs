using libreria_aplicaciones.entidades;
using presentaciones_tienda.interfaces;

namespace presentaciones_tienda.implementaciones
{
    public class CuponesPresentacion : ICuponesPresentacion
    {
        private IComunicaciones IComunicaciones;

        public CuponesPresentacion(IComunicaciones IComunicaciones)
        {
            this.IComunicaciones = IComunicaciones;
        }

        public List<Cupones> Consultar()
        {
            var task = this.IComunicaciones.Ejecutar<List<Cupones>>("Cupones/Consultar", null);
            task.Wait();
            return task.Result;
        }

        public List<Cupones> Buscar(Cupones entidad)
        {
            var task = this.IComunicaciones.Ejecutar<List<Cupones>>("Cupones/Buscar", entidad);
            task.Wait();
            return task.Result;
        }

        public Cupones Insertar(Cupones entidad)
        {
            if (entidad.IdCupon != 0)
                throw new Exception("El cupón ya fue guardado");
            var task = this.IComunicaciones.Ejecutar<Cupones>("Cupones/Insertar", entidad);
            task.Wait();
            return task.Result;
        }

        public Cupones Actualizar(Cupones entidad)
        {
            if (entidad.IdCupon == 0)
                throw new Exception("Selecciona un cupón para actualizar");
            var task = this.IComunicaciones.Ejecutar<Cupones>("Cupones/Actualizar", entidad);
            task.Wait();
            return task.Result;
        }

        public Cupones Borrar(Cupones entidad)
        {
            if (entidad.IdCupon == 0)
                throw new Exception("Selecciona un cupón para borrar");
            var task = this.IComunicaciones.Ejecutar<Cupones>("Cupones/Borrar", entidad);
            task.Wait();
            return task.Result;
        }
    }
}
