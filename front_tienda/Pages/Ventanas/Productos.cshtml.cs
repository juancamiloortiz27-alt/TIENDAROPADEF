using libreria_aplicaciones.entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using presentaciones_tienda.interfaces;

namespace front_tienda.Pages.Ventanas
{
    public class ProductosModel : PageModel
    {
        private IProductosPresentacion IProductosPresentacion;
        private IFavoritosPresentacion IFavoritosPresentacion;
        private ICategoriasPresentacion ICategoriasPresentacion;
        private IMarcasPresentacion IMarcasPresentacion;

        public ProductosModel(IProductosPresentacion IProductosPresentacion,
            IFavoritosPresentacion IFavoritosPresentacion,
            ICategoriasPresentacion ICategoriasPresentacion,
            IMarcasPresentacion IMarcasPresentacion)
        {
            this.IProductosPresentacion = IProductosPresentacion;
            this.IFavoritosPresentacion = IFavoritosPresentacion;
            this.ICategoriasPresentacion = ICategoriasPresentacion;
            this.IMarcasPresentacion = IMarcasPresentacion;
        }

        [BindProperty] public Productos Filtro { get; set; } = new();
        [BindProperty] public string? FiltroCategoria { get; set; }
        [BindProperty] public string? FiltroMarca { get; set; }
        public List<ProductoPublico> Lista { get; set; } = new();
        public List<Favoritos> ListaFavoritos { get; set; } = new();
        public List<Categorias> Categorias { get; set; } = new();
        public List<Marcas> Marcas { get; set; } = new();
        public string? Mensaje { get; set; }
        public bool ClienteAutenticado => HttpContext.Session.GetString("Rol") == "Cliente";
        public string RutaProducto(string? codigoModelo) => "/Ventanas/Producto?codigoModelo=" + Uri.EscapeDataString(codigoModelo ?? "");

        public void OnGet(string? nombre, string? marca, string? categoria)
        {
            Filtro.Nombre = nombre;
            FiltroMarca = marca;
            FiltroCategoria = categoria;
            ConsultarFiltros();
            ConsultarCatalogo();
            ConsultarFavoritos();
        }

        public void OnPostBuscar()
        {
            ConsultarFiltros();
            ConsultarCatalogo();
            ConsultarFavoritos();
        }

        public IActionResult OnPostCambiarFavorito(int idProducto, string? codigoModelo)
        {
            if (!ClienteAutenticado)
                return RedirectToPage("/Cuenta/Acceso", new { returnUrl = Request.Path.ToString() + Request.QueryString.ToString() });

            ConsultarFiltros();
            ConsultarFavoritos();
            var estaGuardado = ListaFavoritos.Any(x => x._Producto?.CodigoModelo == codigoModelo);
            try
            {
                if (estaGuardado)
                {
                    this.IFavoritosPresentacion.Borrar(new Favoritos { Producto = idProducto });
                    Mensaje = "Producto quitado de favoritos";
                }
                else
                {
                    this.IFavoritosPresentacion.Agregar(new Favoritos { Producto = idProducto });
                    Mensaje = "Producto agregado a favoritos";
                }
            }
            catch (Exception ex) { Mensaje = ex.Message; }

            ConsultarCatalogo();
            ConsultarFavoritos();
            return Page();
        }

        private void ConsultarCatalogo()
        {
            try
            {
                var productos = string.IsNullOrWhiteSpace(Filtro.Nombre)
                    ? this.IProductosPresentacion.ConsultarPublicos()
                    : this.IProductosPresentacion.BuscarPublicos(Filtro);

                Lista = Agrupar(productos)
                    .Where(x => string.IsNullOrWhiteSpace(FiltroCategoria) || x.NombreCategoria == FiltroCategoria)
                    .Where(x => string.IsNullOrWhiteSpace(FiltroMarca) || x.NombreMarca == FiltroMarca)
                    .ToList();
            }
            catch (Exception ex) { Mensaje = ex.Message; }
        }

        private void ConsultarFiltros()
        {
            try { Categorias = this.ICategoriasPresentacion.Consultar().Where(x => x.Estado).ToList(); }
            catch (Exception ex) { Mensaje = ex.Message; }
            try { Marcas = this.IMarcasPresentacion.ConsultarActivas(); }
            catch (Exception ex) { Mensaje = ex.Message; }
        }

        private void ConsultarFavoritos()
        {
            if (!ClienteAutenticado) return;
            try { ListaFavoritos = this.IFavoritosPresentacion.Consultar(); }
            catch (Exception ex) { Mensaje = ex.Message; }
        }

        private List<ProductoPublico> Agrupar(List<ProductoPublico> variantes)
        {
            return variantes
                .GroupBy(x => x.CodigoModelo)
                .Select(grupo =>
                {
                    var producto = grupo.First();
                    producto.Disponible = grupo.Any(x => x.Disponible);
                    return producto;
                })
                .ToList();
        }
    }
}
