using libreria_aplicaciones.entidades;

namespace libreria_aplicaciones.interfaces
{
    public interface IUsuariosAplicacion
    {
        Usuarios IniciarSesion(string correo, string contrasena);
        Usuarios RegistrarCliente(RegistroCliente entidad);
        List<UsuarioResumen> ConsultarUsuarios();
        void CambiarRol(int idUsuario, string rol, int idAdministrador);
        void CambiarEstado(int idUsuario, bool estado, int idAdministrador);
        void CrearAdministradorInicial(string correo, string contrasena);
    }
}
