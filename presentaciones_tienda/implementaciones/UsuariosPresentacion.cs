using libreria_aplicaciones.entidades;
using presentaciones_tienda.interfaces;

namespace presentaciones_tienda.implementaciones
{
    public class UsuariosPresentacion : IUsuariosPresentacion
    {
        private IComunicaciones IComunicaciones;

        public UsuariosPresentacion(IComunicaciones IComunicaciones)
        {
            this.IComunicaciones = IComunicaciones;
        }

        public ResultadoAutenticacion IniciarSesion(CredencialesUsuario entidad)
        {
            var task = this.IComunicaciones.Ejecutar<ResultadoAutenticacion>("Usuarios/IniciarSesion", entidad);
            task.Wait();
            return task.Result;
        }

        public ResultadoAutenticacion RegistrarCliente(RegistroCliente entidad)
        {
            var task = this.IComunicaciones.Ejecutar<ResultadoAutenticacion>("Usuarios/RegistrarCliente", entidad);
            task.Wait();
            return task.Result;
        }

        public List<UsuarioResumen> ConsultarUsuarios()
        {
            var task = this.IComunicaciones.Ejecutar<List<UsuarioResumen>>("Usuarios/ConsultarUsuarios", null);
            return task.GetAwaiter().GetResult();
        }

        public void CambiarRol(CambioRolUsuario entidad)
        {
            var task = this.IComunicaciones.Ejecutar<bool>("Usuarios/CambiarRol", entidad);
            task.Wait();
        }

        public void CambiarEstado(EstadoUsuario entidad)
        {
            var task = this.IComunicaciones.Ejecutar<bool>("Usuarios/CambiarEstado", entidad);
            task.Wait();
        }
    }
}
