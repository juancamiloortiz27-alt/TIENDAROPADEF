using libreria_aplicaciones.entidades;
using presentaciones_tienda.interfaces;

namespace presentaciones_tienda.implementaciones
{
    public class ColoresPresentacion : IColoresPresentacion
    {
        private IComunicaciones IComunicaciones;

        public ColoresPresentacion(IComunicaciones IComunicaciones)
        {
            this.IComunicaciones = IComunicaciones;
        }

        public List<Colores> Consultar()
        {
            var task = this.IComunicaciones.Ejecutar<List<Colores>>("Colores/Consultar", null);
            task.Wait();
            return task.Result;
        }

        public List<Colores> Buscar(Colores entidad)
        {
            var task = this.IComunicaciones.Ejecutar<List<Colores>>("Colores/Buscar", entidad);
            task.Wait();
            return task.Result;
        }

        public Colores Insertar(Colores entidad)
        {
            if (entidad.IdColor != 0)
                throw new Exception("Ya se guardó");
            Validar(entidad);
            var task = this.IComunicaciones.Ejecutar<Colores>("Colores/Insertar", entidad);
            task.Wait();
            return task.Result;
        }

        public Colores Actualizar(Colores entidad)
        {
            if (entidad.IdColor == 0)
                throw new Exception("No se ha guardado");
            Validar(entidad);
            var task = this.IComunicaciones.Ejecutar<Colores>("Colores/Actualizar", entidad);
            task.Wait();
            return task.Result;
        }

        public Colores Borrar(Colores entidad)
        {
            if (entidad.IdColor == 0)
                throw new Exception("No se ha guardado");
            var task = this.IComunicaciones.Ejecutar<Colores>("Colores/Borrar", entidad);
            task.Wait();
            return task.Result;
        }

        private void Validar(Colores entidad)
        {
            if (string.IsNullOrEmpty(entidad.Nombre) ||
                string.IsNullOrEmpty(entidad.CodigoHexadecimal) ||
                string.IsNullOrEmpty(entidad.Tipo) ||
                string.IsNullOrEmpty(entidad.Descripcion))
                throw new Exception("Falta información");
        }
    }
}
