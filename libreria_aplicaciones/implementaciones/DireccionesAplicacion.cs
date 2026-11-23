using libreria_aplicaciones.entidades;
using libreria_aplicaciones.interfaces;
using Microsoft.EntityFrameworkCore;

namespace libreria_aplicaciones.implementaciones
{
    public class DireccionesAplicacion : IDireccionesAplicacion
    {
        private IConexion IConexion;

        public DireccionesAplicacion(IConexion IConexion)
        {
            this.IConexion = IConexion;
        }

        public List<Direcciones> Consultar(int idCliente)
        {
            return this.IConexion.Direcciones!
                .Where(x => x.Cliente == idCliente)
                .ToList();
        }

        public Direcciones Insertar(int idCliente, Direcciones entidad)
        {
            Validar(entidad);
            if (entidad.IdDireccion != 0)
                throw new Exception("Ya se guardó");
            entidad.Ciudad = entidad.Ciudad!.Trim();
            entidad.Direccion = entidad.Direccion!.Trim();
            entidad.CodigoPostal = entidad.CodigoPostal!.Trim();
            entidad.Cliente = idCliente;
            entidad._Cliente = null;
            this.IConexion.Direcciones!.Add(entidad);
            this.IConexion.SaveChanges();
            return entidad;
        }

        public Direcciones Actualizar(int idCliente, Direcciones entidad)
        {
            if (entidad.IdDireccion == 0)
                throw new Exception("Selecciona una dirección válida");
            Validar(entidad);
            var direccion = ObtenerPropia(idCliente, entidad.IdDireccion);
            direccion.Ciudad = entidad.Ciudad!.Trim();
            direccion.Direccion = entidad.Direccion!.Trim();
            direccion.CodigoPostal = entidad.CodigoPostal!.Trim();
            this.IConexion.Entry<Direcciones>(direccion).State = EntityState.Modified;
            this.IConexion.SaveChanges();
            return direccion;
        }

        public void Borrar(int idCliente, Direcciones entidad)
        {
            var direccion = ObtenerPropia(idCliente, entidad.IdDireccion);
            this.IConexion.Direcciones!.Remove(direccion);
            this.IConexion.SaveChanges();
        }

        private Direcciones ObtenerPropia(int idCliente, int idDireccion)
        {
            var direccion = this.IConexion.Direcciones!
                .FirstOrDefault(x => x.IdDireccion == idDireccion && x.Cliente == idCliente);
            if (direccion == null)
                throw new Exception("No se encontró esa dirección en tu cuenta");
            return direccion;
        }

        private void Validar(Direcciones entidad)
        {
            if (string.IsNullOrWhiteSpace(entidad.Ciudad) ||
                string.IsNullOrWhiteSpace(entidad.Direccion) ||
                string.IsNullOrWhiteSpace(entidad.CodigoPostal))
                throw new Exception("Completa la ciudad, la dirección y el código postal");
            if (entidad.Ciudad.Trim().Length > 30 || entidad.Direccion.Trim().Length > 100 ||
                entidad.CodigoPostal.Trim().Length > 10)
                throw new Exception("Revisa la longitud de los datos de la dirección");
        }
    }
}
