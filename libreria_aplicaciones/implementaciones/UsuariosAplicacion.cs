using libreria_aplicaciones.entidades;
using libreria_aplicaciones.interfaces;
using Microsoft.AspNetCore.Identity;

namespace libreria_aplicaciones.implementaciones
{
    public class UsuariosAplicacion : IUsuariosAplicacion
    {
        private IConexion IConexion;
        private PasswordHasher<Usuarios> PasswordHasher;

        public UsuariosAplicacion(IConexion IConexion)
        {
            this.IConexion = IConexion;
            this.PasswordHasher = new PasswordHasher<Usuarios>();
        }

        public Usuarios IniciarSesion(string correo, string contrasena)
        {
            var usuario = this.IConexion.Usuarios!
                .FirstOrDefault(x => x.Correo == correo && x.Estado);

            if (usuario == null || string.IsNullOrEmpty(usuario.ContrasenaHash) ||
                this.PasswordHasher.VerifyHashedPassword(usuario, usuario.ContrasenaHash, contrasena)
                    == PasswordVerificationResult.Failed)
                throw new UnauthorizedAccessException("Correo o contraseña incorrectos");

            return usuario;
        }

        public Usuarios RegistrarCliente(RegistroCliente entidad)
        {
            if (string.IsNullOrWhiteSpace(entidad.Nombre) ||
                string.IsNullOrWhiteSpace(entidad.Apellido) ||
                string.IsNullOrWhiteSpace(entidad.Correo) ||
                string.IsNullOrWhiteSpace(entidad.Telefono) ||
                string.IsNullOrWhiteSpace(entidad.Contrasena))
                throw new Exception("Falta informacion");

            if (entidad.Contrasena.Length < 8)
                throw new Exception("La contraseña debe tener al menos 8 caracteres");

            if (this.IConexion.Usuarios!.Any(x => x.Correo == entidad.Correo))
                throw new Exception("Ya existe un usuario con ese correo");

            var cliente = this.IConexion.Clientes!
                .FirstOrDefault(x => x.Correo == entidad.Correo);
            if (cliente == null)
            {
                cliente = new Clientes
                {
                    Nombre = entidad.Nombre,
                    Apellido = entidad.Apellido,
                    Correo = entidad.Correo,
                    Telefono = entidad.Telefono
                };
                this.IConexion.Clientes!.Add(cliente);
                this.IConexion.SaveChanges();
            }

            var usuario = new Usuarios
            {
                Correo = entidad.Correo,
                Rol = "Cliente",
                Estado = true,
                Cliente = cliente.IdCliente
            };
            usuario.ContrasenaHash = this.PasswordHasher.HashPassword(usuario, entidad.Contrasena);

            this.IConexion.Usuarios!.Add(usuario);
            this.IConexion.SaveChanges();
            return usuario;
        }

        public List<UsuarioResumen> ConsultarUsuarios()
        {
            var lista = this.IConexion.Usuarios!
                .Select(x => new UsuarioResumen
                {
                    IdUsuario = x.IdUsuario,
                    Correo = x.Correo,
                    Rol = x.Rol,
                    Estado = x.Estado,
                    Cliente = x.Cliente,
                    TieneCuenta = true
                })
                .ToList();

            var clientesSinCuenta = this.IConexion.Clientes!
                .Where(cliente => !this.IConexion.Usuarios!.Any(usuario => usuario.Correo == cliente.Correo))
                .Select(cliente => new UsuarioResumen
                {
                    IdUsuario = 0,
                    Correo = cliente.Correo,
                    Rol = "Sin cuenta",
                    Estado = false,
                    Cliente = cliente.IdCliente,
                    TieneCuenta = false
                })
                .ToList();

            lista.AddRange(clientesSinCuenta);
            return lista;
        }

        public void CambiarRol(int idUsuario, string rol, int idAdministrador)
        {
            if (rol != "Cliente" && rol != "Administrador")
                throw new Exception("El rol indicado no es válido");

            if (idUsuario == idAdministrador)
                throw new Exception("No puedes cambiar tu propio rol");

            var usuario = this.IConexion.Usuarios!.FirstOrDefault(x => x.IdUsuario == idUsuario);
            if (usuario == null)
                throw new Exception("No se encontró el usuario");

            if (usuario.Rol == "Administrador" && rol == "Cliente" &&
                this.IConexion.Usuarios!.Count(x => x.Rol == "Administrador" && x.Estado) <= 1)
                throw new Exception("Debe quedar al menos un administrador activo");

            usuario.Rol = rol;
            var entry = this.IConexion.Entry<Usuarios>(usuario);
            entry.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            this.IConexion.SaveChanges();
        }

        public void CambiarEstado(int idUsuario, bool estado, int idAdministrador)
        {
            if (idUsuario == idAdministrador)
                throw new Exception("No puedes desactivar tu propia cuenta");

            var usuario = this.IConexion.Usuarios!.FirstOrDefault(x => x.IdUsuario == idUsuario);
            if (usuario == null)
                throw new Exception("No se encontró el usuario");

            if (usuario.Rol == "Administrador" && !estado &&
                this.IConexion.Usuarios!.Count(x => x.Rol == "Administrador" && x.Estado) <= 1)
                throw new Exception("Debe quedar al menos un administrador activo");

            usuario.Estado = estado;
            this.IConexion.Entry<Usuarios>(usuario).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            this.IConexion.SaveChanges();
        }

        public void CrearAdministradorInicial(string correo, string contrasena)
        {
            var administrador = this.IConexion.Usuarios!.FirstOrDefault(x => x.Correo == correo);
            if (administrador != null)
            {
                if (administrador.Rol != "Administrador")
                {
                    administrador.Rol = "Administrador";
                    administrador.Estado = true;
                    administrador.ContrasenaHash = this.PasswordHasher.HashPassword(administrador, contrasena);
                    this.IConexion.Entry<Usuarios>(administrador).State =
                        Microsoft.EntityFrameworkCore.EntityState.Modified;
                    this.IConexion.SaveChanges();
                }
                return;
            }

            administrador = new Usuarios
            {
                Correo = correo,
                Rol = "Administrador",
                Estado = true
            };
            administrador.ContrasenaHash = this.PasswordHasher.HashPassword(administrador, contrasena);
            this.IConexion.Usuarios!.Add(administrador);
            this.IConexion.SaveChanges();
        }
    }
}
