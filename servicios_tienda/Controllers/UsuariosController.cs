using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using libreria_aplicaciones.entidades;
using libreria_aplicaciones.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace servicios_tienda.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class UsuariosController : ControllerBase
    {
        private IUsuariosAplicacion IUsuariosAplicacion;
        private IConfiguration Configuration;

        public UsuariosController(IUsuariosAplicacion IUsuariosAplicacion, IConfiguration Configuration)
        {
            this.IUsuariosAplicacion = IUsuariosAplicacion;
            this.Configuration = Configuration;
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult IniciarSesion(CredencialesUsuario entidad)
        {
            try
            {
                var usuario = this.IUsuariosAplicacion.IniciarSesion(entidad.Correo!, entidad.Contrasena!);
                return Ok(CrearResultado(usuario));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Correo o contraseña inválidos");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ResultadoAutenticacion RegistrarCliente(RegistroCliente entidad)
        {
            var usuario = this.IUsuariosAplicacion.RegistrarCliente(entidad);
            return CrearResultado(usuario);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public List<UsuarioResumen> ConsultarUsuarios()
        {
            return this.IUsuariosAplicacion.ConsultarUsuarios();
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public IActionResult CambiarRol(Usuarios entidad)
        {
            try
            {
                var idAdministrador = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                this.IUsuariosAplicacion.CambiarRol(entidad.IdUsuario, entidad.Rol!, idAdministrador);
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public IActionResult CambiarEstado(Usuarios entidad)
        {
            try
            {
                var idAdministrador = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                this.IUsuariosAplicacion.CambiarEstado(entidad.IdUsuario, entidad.Estado, idAdministrador);
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private ResultadoAutenticacion CrearResultado(Usuarios usuario)
        {
            var clave = this.Configuration["Autenticacion:Clave"]!;
            var emisor = this.Configuration["Autenticacion:Emisor"]!;
            var audiencia = this.Configuration["Autenticacion:Audiencia"]!;
            var credenciales = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(clave)),
                SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                new(ClaimTypes.Email, usuario.Correo!),
                new(ClaimTypes.Role, usuario.Rol!)
            };
            if (usuario.Cliente.HasValue)
                claims.Add(new Claim("Cliente", usuario.Cliente.Value.ToString()));

            var token = new JwtSecurityToken(emisor, audiencia, claims,
                expires: DateTime.UtcNow.AddMinutes(15), signingCredentials: credenciales);

            return new ResultadoAutenticacion
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Correo = usuario.Correo,
                Rol = usuario.Rol,
                Cliente = usuario.Cliente
            };
        }
    }
}
