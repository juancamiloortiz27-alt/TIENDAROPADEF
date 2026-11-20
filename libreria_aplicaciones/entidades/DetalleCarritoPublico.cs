namespace libreria_aplicaciones.entidades
{
    public class DetalleCarritoPublico
    {
        public int IdDetalleCarrito { get; set; }
        public int IdProducto { get; set; }
        public string? Nombre { get; set; }
        public string? Imagen { get; set; }
        public string? NombreColor { get; set; }
        public string? CodigoHexadecimal { get; set; }
        public string? NombreTalla { get; set; }
        public string? SistemaMedida { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int Cantidad { get; set; }
        public decimal Subtotal { get; set; }
        public int StockDisponible { get; set; }
    }
}
