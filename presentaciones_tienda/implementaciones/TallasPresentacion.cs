using libreria_aplicaciones.entidades;
using presentaciones_tienda.interfaces;

namespace presentaciones_tienda.implementaciones
{
    public class TallasPresentacion : ITallasPresentacion
    {
        private IComunicaciones IComunicaciones;

        public TallasPresentacion(IComunicaciones IComunicaciones)
        {
            this.IComunicaciones = IComunicaciones;
        }

        public List<Tallas> Consultar()
        {
            var task = this.IComunicaciones.Ejecutar<List<Tallas>>("Tallas/Consultar", null);
            task.Wait();
            return task.Result;
        }

        public List<Tallas> Buscar(Tallas entidad)
        {
            var task = this.IComunicaciones.Ejecutar<List<Tallas>>("Tallas/Buscar", entidad);
            task.Wait();
            return task.Result;
        }

        public Tallas Insertar(Tallas entidad)
        {
            if (entidad.IdTalla != 0)
                throw new Exception("Ya se guardó");
            Validar(entidad);
            var task = this.IComunicaciones.Ejecutar<Tallas>("Tallas/Insertar", entidad);
            task.Wait();
            return task.Result;
        }

        public Tallas Actualizar(Tallas entidad)
        {
            if (entidad.IdTalla == 0)
                throw new Exception("No se ha guardado");
            Validar(entidad);
            var task = this.IComunicaciones.Ejecutar<Tallas>("Tallas/Actualizar", entidad);
            task.Wait();
            return task.Result;
        }

        public Tallas Borrar(Tallas entidad)
        {
            if (entidad.IdTalla == 0)
                throw new Exception("No se ha guardado");
            var task = this.IComunicaciones.Ejecutar<Tallas>("Tallas/Borrar", entidad);
            task.Wait();
            return task.Result;
        }

        private void Validar(Tallas entidad)
        {
            if (string.IsNullOrEmpty(entidad.Nombre) ||
                string.IsNullOrEmpty(entidad.Tipo) ||
                string.IsNullOrEmpty(entidad.Descripcion) ||
                string.IsNullOrEmpty(entidad.SistemaMedida))
                throw new Exception("Falta información");
        }
    }
}
