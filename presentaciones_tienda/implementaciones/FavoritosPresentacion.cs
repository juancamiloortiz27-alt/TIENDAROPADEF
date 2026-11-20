using libreria_aplicaciones.entidades;
using presentaciones_tienda.interfaces;

namespace presentaciones_tienda.implementaciones
{
    public class FavoritosPresentacion : IFavoritosPresentacion
    {
        private IComunicaciones IComunicaciones;

        public FavoritosPresentacion(IComunicaciones IComunicaciones)
        {
            this.IComunicaciones = IComunicaciones;
        }

        public List<Favoritos> Consultar()
        {
            var task = this.IComunicaciones.Ejecutar<List<Favoritos>>("Favoritos/Consultar", null);
            task.Wait();
            return task.Result;
        }

        public Favoritos Agregar(Favoritos entidad)
        {
            var task = this.IComunicaciones.Ejecutar<Favoritos>("Favoritos/Agregar", entidad);
            task.Wait();
            return task.Result;
        }

        public Favoritos Borrar(Favoritos entidad)
        {
            var task = this.IComunicaciones.Ejecutar<Favoritos>("Favoritos/Borrar", entidad);
            task.Wait();
            return task.Result;
        }
    }
}
