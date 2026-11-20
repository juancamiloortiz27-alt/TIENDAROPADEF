namespace libreria_aplicaciones.entidades
{
    public class CuponValidado
    {
        public string? Codigo { get; set; }
        public decimal PorcentajeDescuento { get; set; }
        public decimal Descuento { get; set; }
        public decimal TotalConDescuento { get; set; }
    }
}
