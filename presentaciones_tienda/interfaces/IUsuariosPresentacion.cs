using libreria_aplicaciones.entidades;

namespace presentaciones_tienda.interfaces
{
    public interface IUsuariosPresentacion
    {
        ResultadoAutenticacion IniciarSesion(CredencialesUsuario entidad);
        ResultadoAutenticacion RegistrarCliente(RegistroCliente entidad);
        List<UsuarioResumen> ConsultarUsuarios();
        void CambiarRol(CambioRolUsuario entidad);
        void CambiarEstado(EstadoUsuario entidad);
    }
}
