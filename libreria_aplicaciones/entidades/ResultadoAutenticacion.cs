namespace libreria_aplicaciones.entidades
{
    public class ResultadoAutenticacion
    {
        public string? Token { get; set; }
        public string? Correo { get; set; }
        public string? Rol { get; set; }
        public int? Cliente { get; set; }
    }
}
