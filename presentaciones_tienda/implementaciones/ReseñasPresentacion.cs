using libreria_aplicaciones.entidades;
using presentaciones_tienda.interfaces;

namespace presentaciones_tienda.implementaciones
{
    public class ReseñasPresentacion : IReseñasPresentacion
    {
        private IComunicaciones IComunicaciones;

        public ReseñasPresentacion(IComunicaciones IComunicaciones)
        {
            this.IComunicaciones = IComunicaciones;
        }

        public List<Reseñas> ConsultarPorProducto(int idProducto)
        {
            var task = this.IComunicaciones.Ejecutar<List<Reseñas>>("Reseñas/ConsultarPorProducto", new Reseñas
            {
                Producto = idProducto
            });
            task.Wait();
            return task.Result;
        }

        public Reseñas Insertar(Reseñas entidad)
        {
            var task = this.IComunicaciones.Ejecutar<Reseñas>("Reseñas/Insertar", entidad);
            task.Wait();
            return task.Result;
        }

        public Reseñas Borrar(Reseñas entidad)
        {
            var task = this.IComunicaciones.Ejecutar<Reseñas>("Reseñas/Borrar", entidad);
            task.Wait();
            return task.Result;
        }
    }
}
