namespace libreria_aplicaciones.entidades
{
    public class UsuarioResumen
    {
        public int IdUsuario { get; set; }
        public string? Correo { get; set; }
        public string? Rol { get; set; }
        public bool Estado { get; set; }
        public int? Cliente { get; set; }
        public bool TieneCuenta { get; set; }
    }
}
