using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace libreria_aplicaciones.entidades
{
    public class Usuarios
    {
        [Key] public int IdUsuario { get; set; }
        public string? Correo { get; set; }
        public string? ContrasenaHash { get; set; }
        public string? Rol { get; set; }
        public bool Estado { get; set; }
        public int? Cliente { get; set; }
        [ForeignKey("Cliente")] public Clientes? _Cliente { get; set; }
    }
}
