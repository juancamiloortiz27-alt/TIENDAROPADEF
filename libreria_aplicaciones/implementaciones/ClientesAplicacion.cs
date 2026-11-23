using libreria_aplicaciones.entidades;
using libreria_aplicaciones.interfaces;
using Microsoft.EntityFrameworkCore;

namespace libreria_aplicaciones.implementaciones
{
    public class ClientesAplicacion : IClientesAplicacion
    {
        private IConexion IConexion;

        public ClientesAplicacion(IConexion IConexion)
        {
            this.IConexion = IConexion;
        }

        public Clientes ConsultarPerfil(int idCliente)
        {
            var cliente = this.IConexion.Clientes!
                .FirstOrDefault(x => x.IdCliente == idCliente);
            if (cliente == null)
                throw new Exception("No se encontró el perfil del cliente");
            return CrearPerfil(cliente);
        }

        public Clientes ActualizarPerfil(int idCliente, Clientes entidad)
        {
            if (string.IsNullOrWhiteSpace(entidad.Nombre) ||
                string.IsNullOrWhiteSpace(entidad.Apellido) ||
                string.IsNullOrWhiteSpace(entidad.Telefono))
                throw new Exception("Completa el nombre, apellido y teléfono");
            if (entidad.Nombre.Trim().Length > 50 ||
                entidad.Apellido.Trim().Length > 50 ||
                entidad.Telefono.Trim().Length > 12)
                throw new Exception("El nombre y apellido admiten hasta 50 caracteres y el teléfono hasta 12");

            var cliente = this.IConexion.Clientes!
                .FirstOrDefault(x => x.IdCliente == idCliente);
            if (cliente == null)
                throw new Exception("No se encontró el perfil del cliente");

            cliente.Nombre = entidad.Nombre.Trim();
            cliente.Apellido = entidad.Apellido.Trim();
            cliente.Telefono = entidad.Telefono.Trim();
            this.IConexion.Entry<Clientes>(cliente).State = EntityState.Modified;
            this.IConexion.SaveChanges();
            return CrearPerfil(cliente);
        }

        private Clientes CrearPerfil(Clientes cliente)
        {
            return new Clientes
            {
                IdCliente = cliente.IdCliente,
                Nombre = cliente.Nombre,
                Apellido = cliente.Apellido,
                Correo = cliente.Correo,
                Telefono = cliente.Telefono
            };
        }
    }
}
