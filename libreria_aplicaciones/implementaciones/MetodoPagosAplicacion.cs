using libreria_aplicaciones.entidades;
using libreria_aplicaciones.interfaces;
using Microsoft.EntityFrameworkCore;

namespace libreria_aplicaciones.implementaciones
{
    public class MetodoPagosAplicacion : IMetodoPagosAplicacion
    {
        private IConexion IConexion;

        public MetodoPagosAplicacion(IConexion IConexion)
        {
            this.IConexion = IConexion;
        }

        public List<MetodoPagos> Consultar()
        {
            return this.IConexion.MetodoPagos!.OrderBy(x => x.Nombre).ToList();
        }

        public List<MetodoPagos> ConsultarActivos()
        {
            return this.IConexion.MetodoPagos!
                .Where(x => x.Estado)
                .OrderBy(x => x.Nombre)
                .ToList();
        }

        public List<MetodoPagos> Buscar(MetodoPagos entidad)
        {
            if (string.IsNullOrWhiteSpace(entidad.Nombre))
                return Consultar();
            var nombre = entidad.Nombre.Trim();
            return this.IConexion.MetodoPagos!
                .Where(x => x.Nombre!.Contains(nombre))
                .Take(20)
                .ToList();
        }

        public MetodoPagos Insertar(MetodoPagos entidad)
        {
            if (entidad.IdMetodoPago != 0)
                throw new Exception("El método de pago ya fue guardado");
            Validar(entidad);
            entidad.Pago = null;
            this.IConexion.MetodoPagos!.Add(entidad);
            this.IConexion.SaveChanges();
            return entidad;
        }

        public MetodoPagos Actualizar(MetodoPagos entidad)
        {
            if (entidad.IdMetodoPago == 0)
                throw new Exception("Selecciona un método de pago para actualizar");
            Validar(entidad);
            entidad.Pago = null;
            if (!this.IConexion.MetodoPagos!.Any(x => x.IdMetodoPago == entidad.IdMetodoPago))
                throw new Exception("No se encontró el método de pago");
            this.IConexion.Entry<MetodoPagos>(entidad).State = EntityState.Modified;
            this.IConexion.SaveChanges();
            return entidad;
        }

        public MetodoPagos Borrar(MetodoPagos entidad)
        {
            if (entidad.IdMetodoPago == 0)
                throw new Exception("Selecciona un método de pago para borrar");
            var metodo = this.IConexion.MetodoPagos!
                .FirstOrDefault(x => x.IdMetodoPago == entidad.IdMetodoPago);
            if (metodo == null)
                throw new Exception("No se encontró el método de pago");
            if (this.IConexion.Pagos!.Any(x => x.MetodoPago == entidad.IdMetodoPago))
                throw new Exception("Este método ya fue usado en un pago. Desactívalo para conservar el historial");
            this.IConexion.MetodoPagos!.Remove(metodo);
            this.IConexion.SaveChanges();
            return metodo;
        }

        private void Validar(MetodoPagos entidad)
        {
            if (string.IsNullOrWhiteSpace(entidad.Nombre) || entidad.Nombre.Trim().Length > 30 ||
                string.IsNullOrWhiteSpace(entidad.Descripcion) || entidad.Descripcion.Trim().Length > 100 ||
                string.IsNullOrWhiteSpace(entidad.Tipo) || entidad.Tipo.Trim().Length > 20)
                throw new Exception("Completa nombre, descripción y tipo con las longitudes permitidas");
            if (entidad.Comision < 0 || entidad.Comision > 100)
                throw new Exception("La comisión debe estar entre 0 y 100 %");
            entidad.Nombre = entidad.Nombre.Trim();
            entidad.Descripcion = entidad.Descripcion.Trim();
            entidad.Tipo = entidad.Tipo.Trim();
        }
    }
}
