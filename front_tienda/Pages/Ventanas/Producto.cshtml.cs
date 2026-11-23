using libreria_aplicaciones.entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using presentaciones_tienda.interfaces;

namespace front_tienda.Pages.Ventanas
{
    public class ProductoModel : PageModel
    {
        private IProductosPresentacion IProductosPresentacion;
        private ICarritosPresentacion ICarritosPresentacion;
        private IReseñasPresentacion IReseñasPresentacion;

        public ProductoModel(
            IProductosPresentacion IProductosPresentacion,
            ICarritosPresentacion ICarritosPresentacion,
            IReseñasPresentacion IReseñasPresentacion)
        {
            this.IProductosPresentacion = IProductosPresentacion;
            this.ICarritosPresentacion = ICarritosPresentacion;
            this.IReseñasPresentacion = IReseñasPresentacion;
        }

        [BindProperty(SupportsGet = true)] public string? CodigoModelo { get; set; }
        [BindProperty] public int IdProductoSeleccionado { get; set; }
        [BindProperty] public int Cantidad { get; set; } = 1;
        [BindProperty] public Reseñas NuevaReseña { get; set; } = new();
        public List<ProductoPublico> Variantes { get; set; } = new();
        public List<ProductoPublico> TallasDisponibles => Variantes.Where(x => x.Disponible).ToList();
        public List<Reseñas> ListaReseñas { get; set; } = new();
        public ProductoPublico? Producto { get; set; }
        public string? Mensaje { get; set; }
        public bool ClienteAutenticado => HttpContext.Session.GetString("Rol") == "Cliente";
        public bool EsAdministrador => HttpContext.Session.GetString("Rol") == "Administrador";
        public string RutaActual => HttpContext.Request.Path.ToString() + HttpContext.Request.QueryString.ToString();
        public decimal CalificacionPromedio => ListaReseñas.Count == 0 ? 0 : Math.Round(ListaReseñas.Average(x => x.Calificacion), 1);

        public IActionResult OnGet()
        {
            CargarVariantes();
            if (Variantes.Count == 0) return NotFound();
            CargarReseñas();
            return Page();
        }

        public IActionResult OnPostAgregarAlCarrito()
        {
            if (HttpContext.Session.GetString("Rol") != "Cliente")
                return RedirectToPage("/Cuenta/Acceso", new { returnUrl = Request.Path.ToString() + Request.QueryString.ToString() });

            if (IdProductoSeleccionado == 0)
                Mensaje = "Selecciona una talla";
            else if (Cantidad <= 0)
                Mensaje = "La cantidad debe ser mayor que cero";
            else
            {
                try
                {
                    this.ICarritosPresentacion.AgregarProducto(new DetalleCarritos
                    {
                        Producto = IdProductoSeleccionado,
                        Cantidad = Cantidad
                    });
                    Mensaje = "Producto agregado al carrito";
                }
                catch (Exception ex) { Mensaje = ex.Message; }
            }

            CargarVariantes();
            CargarReseñas();
            return Page();
        }

        public IActionResult OnPostAgregarReseña()
        {
            if (!ClienteAutenticado)
                return RedirectToPage("/Cuenta/Acceso", new { returnUrl = RutaActual });
            CargarVariantes();
            try
            {
                NuevaReseña.Producto = Producto?.IdProducto ?? 0;
                this.IReseñasPresentacion.Insertar(NuevaReseña);
                Mensaje = "Tu reseña fue publicada";
                NuevaReseña = new Reseñas();
                ModelState.Clear();
            }
            catch (Exception ex) { Mensaje = ex.Message; }
            CargarVariantes();
            CargarReseñas();
            return Page();
        }

        public IActionResult OnPostBorrarReseña(int id)
        {
            if (!EsAdministrador) return Forbid();
            try
            {
                this.IReseñasPresentacion.Borrar(new Reseñas { IdReseña = id });
                Mensaje = "Reseña eliminada";
            }
            catch (Exception ex) { Mensaje = ex.Message; }
            CargarVariantes();
            CargarReseñas();
            return Page();
        }

        private void CargarVariantes()
        {
            if (string.IsNullOrWhiteSpace(CodigoModelo)) return;

            try
            {
                Variantes = this.IProductosPresentacion.ConsultarVariantesPublicas(CodigoModelo);
                Producto = Variantes.FirstOrDefault();
                if (Producto != null)
                    Producto.Disponible = Variantes.Any(x => x.Disponible);
            }
            catch (Exception ex) { Mensaje = ex.Message; }
        }

        private void CargarReseñas()
        {
            if (Producto == null) return;
            try { ListaReseñas = this.IReseñasPresentacion.ConsultarPorProducto(Producto.IdProducto); }
            catch (Exception ex) { Mensaje = ex.Message; }
        }
    }
}
