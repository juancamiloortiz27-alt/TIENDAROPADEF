namespace libreria_aplicaciones.entidades
{
    public class CarritoPublico
    {
        public int IdCarrito { get; set; }
        public decimal Total { get; set; }
        public List<DetalleCarritoPublico> Detalles { get; set; } = new();
    }
}
