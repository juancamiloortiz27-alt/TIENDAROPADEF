using libreria_aplicaciones.entidades;
using presentaciones_tienda.interfaces;

namespace presentaciones_tienda.implementaciones
{
    public class ProveedoresPresentacion : IProveedoresPresentacion
    {
        private IComunicaciones IComunicaciones;

        public ProveedoresPresentacion(IComunicaciones IComunicaciones)
        {
            this.IComunicaciones = IComunicaciones;
        }

        public List<Proveedores> Consultar()
        {
            var task = this.IComunicaciones.Ejecutar<List<Proveedores>>("Proveedores/Consultar", null);
            task.Wait();
            return task.Result;
        }

        public List<Proveedores> Buscar(Proveedores entidad)
        {
            var task = this.IComunicaciones.Ejecutar<List<Proveedores>>("Proveedores/Buscar", entidad);
            task.Wait();
            return task.Result;
        }

        public Proveedores Insertar(Proveedores entidad)
        {
            if (entidad.IdProveedor != 0)
                throw new Exception("Ya se guardó");
            Validar(entidad);
            var task = this.IComunicaciones.Ejecutar<Proveedores>("Proveedores/Insertar", entidad);
            task.Wait();
            return task.Result;
        }

        public Proveedores Actualizar(Proveedores entidad)
        {
            if (entidad.IdProveedor == 0)
                throw new Exception("No se ha guardado");
            Validar(entidad);
            var task = this.IComunicaciones.Ejecutar<Proveedores>("Proveedores/Actualizar", entidad);
            task.Wait();
            return task.Result;
        }

        public Proveedores Borrar(Proveedores entidad)
        {
            if (entidad.IdProveedor == 0)
                throw new Exception("No se ha guardado");
            var task = this.IComunicaciones.Ejecutar<Proveedores>("Proveedores/Borrar", entidad);
            task.Wait();
            return task.Result;
        }

        private void Validar(Proveedores entidad)
        {
            if (string.IsNullOrEmpty(entidad.Nombre) ||
                string.IsNullOrEmpty(entidad.Telefono) ||
                string.IsNullOrEmpty(entidad.Correo) ||
                string.IsNullOrEmpty(entidad.Ciudad))
                throw new Exception("Falta información");
        }
    }
}
