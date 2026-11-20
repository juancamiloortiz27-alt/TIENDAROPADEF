namespace libreria_aplicaciones.entidades
{
    public class ProductoPublico
    {
        public string? CodigoModelo { get; set; }
        public int IdProducto { get; set; }
        public string? Nombre { get; set; }
        public decimal Precio { get; set; }
        public string? Descripcion { get; set; }
        public string? Imagen { get; set; }
        public string? NombreCategoria { get; set; }
        public string? NombreMarca { get; set; }
        public string? NombreColor { get; set; }
        public string? CodigoHexadecimal { get; set; }
        public string? NombreTalla { get; set; }
        public string? SistemaMedida { get; set; }
        public int StockDisponible { get; set; }
        public bool Disponible { get; set; }
    }
}
