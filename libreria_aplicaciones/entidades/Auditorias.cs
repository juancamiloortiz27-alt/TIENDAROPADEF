using System.ComponentModel.DataAnnotations;

namespace libreria_aplicaciones.entidades
{
    public class Auditorias
    {
        [Key] public int IdAuditoria { get; set; }
        public DateTime FechaHoraUtc { get; set; }
        public int? UsuarioId { get; set; }
        public string? CorreoUsuario { get; set; }
        public string? RolUsuario { get; set; }
        public string? Accion { get; set; }
        public string? Entidad { get; set; }
        public string? IdRegistro { get; set; }
        public string? Cambios { get; set; }
    }
}
