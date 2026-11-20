using libreria_aplicaciones.entidades;
using presentaciones_tienda.interfaces;

namespace presentaciones_tienda.implementaciones
{
    public class MarcasPresentacion : IMarcasPresentacion
    {
        private IComunicaciones IComunicaciones;

        public MarcasPresentacion(IComunicaciones IComunicaciones)
        {
            this.IComunicaciones = IComunicaciones;
        }

        public List<Marcas> Consultar()
        {
            var task = this.IComunicaciones.Ejecutar<List<Marcas>>("Marcas/Consultar", null);
            task.Wait();
            return task.Result;
        }

        public List<Marcas> Buscar(Marcas entidad)
        {
            var task = this.IComunicaciones.Ejecutar<List<Marcas>>("Marcas/Buscar", entidad);
            task.Wait();
            return task.Result;
        }

        public List<Marcas> BuscarActivas(Marcas entidad)
        {
            var task = this.IComunicaciones.Ejecutar<List<Marcas>>("Marcas/BuscarActivas", entidad);
            task.Wait();
            return task.Result;
        }

        public List<Marcas> ConsultarActivas()
        {
            var task = this.IComunicaciones.Ejecutar<List<Marcas>>("Marcas/ConsultarActivas", null);
            task.Wait();
            return task.Result;
        }

        public Marcas Insertar(Marcas entidad)
        {
            if (entidad.IdMarca != 0)
                throw new Exception("Ya se guardó");
            Validar(entidad);
            var task = this.IComunicaciones.Ejecutar<Marcas>("Marcas/Insertar", entidad);
            task.Wait();
            return task.Result;
        }

        public Marcas Actualizar(Marcas entidad)
        {
            if (entidad.IdMarca == 0)
                throw new Exception("No se ha guardado");
            Validar(entidad);
            var task = this.IComunicaciones.Ejecutar<Marcas>("Marcas/Actualizar", entidad);
            task.Wait();
            return task.Result;
        }

        public Marcas Borrar(Marcas entidad)
        {
            if (entidad.IdMarca == 0)
                throw new Exception("No se ha guardado");
            var task = this.IComunicaciones.Ejecutar<Marcas>("Marcas/Borrar", entidad);
            task.Wait();
            return task.Result;
        }

        private void Validar(Marcas entidad)
        {
            if (string.IsNullOrEmpty(entidad.Nombre) ||
                string.IsNullOrEmpty(entidad.PaisOrigen) ||
                string.IsNullOrEmpty(entidad.SitioWeb))
                throw new Exception("Falta información");
        }
    }
}
