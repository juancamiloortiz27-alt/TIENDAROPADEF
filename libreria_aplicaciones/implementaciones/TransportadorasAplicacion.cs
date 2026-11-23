using libreria_aplicaciones.entidades;
using libreria_aplicaciones.interfaces;
using Microsoft.EntityFrameworkCore;

namespace libreria_aplicaciones.implementaciones
{
    public class TransportadorasAplicacion : ITransportadorasAplicacion
    {
        private IConexion IConexion;

        public TransportadorasAplicacion(IConexion IConexion)
        {
            this.IConexion = IConexion;
        }

        public List<Transportadoras> Consultar()
        {
            return this.IConexion.Transportadoras!.OrderBy(x => x.Nombre).ToList();
        }

        public List<Transportadoras> Buscar(Transportadoras entidad)
        {
            if (string.IsNullOrWhiteSpace(entidad.Nombre) &&
                string.IsNullOrWhiteSpace(entidad.Correo) &&
                string.IsNullOrWhiteSpace(entidad.Ciudad))
                return Consultar();

            return this.IConexion.Transportadoras!
                .Where(x => (!string.IsNullOrWhiteSpace(entidad.Nombre) && x.Nombre!.Contains(entidad.Nombre)) ||
                            (!string.IsNullOrWhiteSpace(entidad.Correo) && x.Correo!.Contains(entidad.Correo)) ||
                            (!string.IsNullOrWhiteSpace(entidad.Ciudad) && x.Ciudad!.Contains(entidad.Ciudad)))
                .Take(20)
                .ToList();
        }

        public Transportadoras Insertar(Transportadoras entidad)
        {
            if (entidad.IdTransportadoras != 0)
                throw new Exception("La transportadora ya fue guardada");
            Validar(entidad);
            entidad.Nombre = entidad.Nombre!.Trim();
            entidad.Telefono = entidad.Telefono!.Trim();
            entidad.Correo = entidad.Correo!.Trim();
            entidad.Ciudad = entidad.Ciudad!.Trim();
            entidad.Envio = null;
            this.IConexion.Transportadoras!.Add(entidad);
            this.IConexion.SaveChanges();
            return entidad;
        }

        public Transportadoras Actualizar(Transportadoras entidad)
        {
            if (entidad.IdTransportadoras == 0)
                throw new Exception("Selecciona una transportadora para actualizar");
            Validar(entidad);
            entidad.Nombre = entidad.Nombre!.Trim();
            entidad.Telefono = entidad.Telefono!.Trim();
            entidad.Correo = entidad.Correo!.Trim();
            entidad.Ciudad = entidad.Ciudad!.Trim();
            entidad.Envio = null;
            if (!this.IConexion.Transportadoras!.Any(x => x.IdTransportadoras == entidad.IdTransportadoras))
                throw new Exception("No se encontró la transportadora");
            this.IConexion.Entry<Transportadoras>(entidad).State = EntityState.Modified;
            this.IConexion.SaveChanges();
            return entidad;
        }

        public Transportadoras Borrar(Transportadoras entidad)
        {
            if (entidad.IdTransportadoras == 0)
                throw new Exception("Selecciona una transportadora para borrar");
            var transportadora = this.IConexion.Transportadoras!
                .FirstOrDefault(x => x.IdTransportadoras == entidad.IdTransportadoras);
            if (transportadora == null)
                throw new Exception("No se encontró la transportadora");
            if (this.IConexion.Envios!.Any(x => x.Transportadora == entidad.IdTransportadoras))
                throw new Exception("Esta transportadora ya está asociada a envíos y no se puede borrar");
            this.IConexion.Transportadoras!.Remove(transportadora);
            this.IConexion.SaveChanges();
            return transportadora;
        }

        private void Validar(Transportadoras entidad)
        {
            if (string.IsNullOrWhiteSpace(entidad.Nombre) || entidad.Nombre.Trim().Length > 30 ||
                string.IsNullOrWhiteSpace(entidad.Telefono) || entidad.Telefono.Trim().Length > 12 ||
                string.IsNullOrWhiteSpace(entidad.Correo) || entidad.Correo.Trim().Length > 50 ||
                string.IsNullOrWhiteSpace(entidad.Ciudad) || entidad.Ciudad.Trim().Length > 15)
                throw new Exception("Completa los datos de la transportadora y revisa sus longitudes");
        }
    }
}
