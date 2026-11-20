using libreria_aplicaciones.entidades;
using Microsoft.AspNetCore.Mvc.RazorPages;
using presentaciones_tienda.interfaces;

namespace front_tienda.Pages
{
    public class IndexModel : PageModel
    {
        private IProductosPresentacion IProductosPresentacion;
        private IMarcasPresentacion IMarcasPresentacion;

        public IndexModel(IProductosPresentacion IProductosPresentacion, IMarcasPresentacion IMarcasPresentacion)
        {
            this.IProductosPresentacion = IProductosPresentacion;
            this.IMarcasPresentacion = IMarcasPresentacion;
        }

        public List<ProductoPublico> Productos { get; set; } = new();
        public List<ProductoPublico> ProductosMarca { get; set; } = new();
        public List<Marcas> Marcas { get; set; } = new();
        public string? Mensaje { get; set; }

        public void OnGet()
        {
            try
            {
                var productos = this.IProductosPresentacion.ConsultarPublicos()
                    .GroupBy(x => x.CodigoModelo)
                    .Select(grupo =>
                    {
                        var producto = grupo.First();
                        producto.Disponible = grupo.Any(x => x.Disponible);
                        producto.Imagen = grupo.Select(x => x.Imagen).FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
                        return producto;
                    })
                    .ToList();
                Productos = productos.Where(x => x.Disponible).Take(4).ToList();
                ProductosMarca = productos
                    .Where(x => x.Disponible)
                    .GroupBy(x => x.NombreMarca)
                    .Select(grupo => grupo.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.Imagen)) ?? grupo.First())
                    .ToList();
            }
            catch (Exception ex) { Mensaje = ex.Message; }

            try
            {
                Marcas = this.IMarcasPresentacion.ConsultarActivas().Take(6).ToList();
            }
            catch (Exception ex) { Mensaje = ex.Message; }
        }
    }
}
