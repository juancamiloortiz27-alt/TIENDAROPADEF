using libreria_aplicaciones.entidades;
using libreria_aplicaciones.interfaces;
using Microsoft.EntityFrameworkCore;

namespace libreria_aplicaciones.implementaciones
{
    public class ProveedoresAplicacion : IProveedoresAplicacion
    {
        private IConexion IConexion;

        public ProveedoresAplicacion(IConexion IConexion)
        {
            this.IConexion = IConexion;
        }

        public Proveedores Actualizar(Proveedores entidad)
        {
            if (entidad.IdProveedor == 0)
                throw new Exception("No se ha guardado");
            if (Validar(entidad))
                throw new Exception("Falta información");

            var entry = this.IConexion.Entry<Proveedores>(entidad);
            entry.State = EntityState.Modified;
            this.IConexion.SaveChanges();
            return entidad;
        }

        public Proveedores Borrar(Proveedores entidad)
        {
            if (entidad.IdProveedor == 0)
                throw new Exception("No se ha guardado");

            this.IConexion.Proveedores!.Remove(entidad);
            this.IConexion.SaveChanges();
            return entidad;
        }

        public List<Proveedores> Buscar(Proveedores entidad)
        {
            return this.IConexion.Proveedores!
                .Where(x => (!string.IsNullOrEmpty(entidad.Nombre) && x.Nombre!.Contains(entidad.Nombre)) ||
                            (!string.IsNullOrEmpty(entidad.Correo) && x.Correo!.Contains(entidad.Correo)) ||
                            (!string.IsNullOrEmpty(entidad.Ciudad) && x.Ciudad!.Contains(entidad.Ciudad)))
                .Take(5)
                .ToList();
        }

        public List<Proveedores> Consultar()
        {
            return this.IConexion.Proveedores!.ToList();
        }

        public Proveedores Insertar(Proveedores entidad)
        {
            if (entidad.IdProveedor != 0)
                throw new Exception("Ya se guardó");
            if (Validar(entidad))
                throw new Exception("Falta información");

            this.IConexion.Proveedores!.Add(entidad);
            this.IConexion.SaveChanges();
            return entidad;
        }

        private bool Validar(Proveedores entidad)
        {
            return string.IsNullOrEmpty(entidad.Nombre) ||
                string.IsNullOrEmpty(entidad.Telefono) ||
                string.IsNullOrEmpty(entidad.Correo) ||
                string.IsNullOrEmpty(entidad.Ciudad);
        }
    }
}
