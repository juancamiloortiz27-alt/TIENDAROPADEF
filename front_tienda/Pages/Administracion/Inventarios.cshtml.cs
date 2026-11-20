using libreria_aplicaciones.entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using presentaciones_tienda.interfaces;

namespace front_tienda.Pages.Administracion
{
    public class InventariosModel : PageModel
    {
        private const int StockMinimo = 5;
        private IInventariosPresentacion IInventariosPresentacion;
        private IProductosPresentacion IProductosPresentacion;

        public InventariosModel(
            IInventariosPresentacion IInventariosPresentacion,
            IProductosPresentacion IProductosPresentacion)
        {
            this.IInventariosPresentacion = IInventariosPresentacion;
            this.IProductosPresentacion = IProductosPresentacion;
        }

        [BindProperty] public Inventarios Entidad { get; set; } = new();
        [BindProperty] public Inventarios Filtro { get; set; } = new();
        public List<Inventarios> Lista { get; set; } = new();
        public List<Productos> ProductosLista { get; set; } = new();
        public string? Mensaje { get; set; }
        public string? NotificacionStock { get; set; }

        public IActionResult OnGet()
        {
            if (!EsAdministrador()) return Forbid();
            Consultar();
            return Page();
        }

        public IActionResult OnPostLimpiar()
        {
            if (!EsAdministrador()) return Forbid();
            Entidad = new Inventarios();
            Filtro = new Inventarios();
            ModelState.Clear();
            Consultar();
            return Page();
        }

        public IActionResult OnPostBuscar()
        {
            if (!EsAdministrador()) return Forbid();
            try { Lista = this.IInventariosPresentacion.Buscar(Filtro); }
            catch (Exception ex) { Mensaje = ex.Message; }
            CargarProductos();
            NotificarStockBajo();
            return Page();
        }

        public IActionResult OnPostSeleccionar(int id)
        {
            if (!EsAdministrador()) return Forbid();
            Consultar();
            Entidad = Lista.FirstOrDefault(x => x.IdInventario == id) ?? new Inventarios();
            return Page();
        }

        public IActionResult OnPostInsertar()
        {
            if (!EsAdministrador()) return Forbid();
            try
            {
                this.IInventariosPresentacion.Insertar(Entidad);
                Mensaje = "Existencias guardadas";
                Entidad = new Inventarios();
                ModelState.Clear();
            }
            catch (Exception ex) { Mensaje = ex.Message; }
            Consultar();
            return Page();
        }

        public IActionResult OnPostActualizar()
        {
            if (!EsAdministrador()) return Forbid();
            try
            {
                this.IInventariosPresentacion.Actualizar(Entidad);
                Mensaje = "Existencias actualizadas";
                Entidad = new Inventarios();
                ModelState.Clear();
            }
            catch (Exception ex) { Mensaje = ex.Message; }
            Consultar();
            return Page();
        }

        public IActionResult OnPostBorrar(int id)
        {
            if (!EsAdministrador()) return Forbid();
            try
            {
                this.IInventariosPresentacion.Borrar(new Inventarios { IdInventario = id });
                Mensaje = "Registro de inventario borrado";
                Entidad = new Inventarios();
                ModelState.Clear();
            }
            catch (Exception ex) { Mensaje = ex.Message; }
            Consultar();
            return Page();
        }

        private bool EsAdministrador()
        {
            return HttpContext.Session.GetString("Rol") == "Administrador";
        }

        private void Consultar()
        {
            try { Lista = this.IInventariosPresentacion.Consultar(); }
            catch (Exception ex) { Mensaje = ex.Message; }
            CargarProductos();
            NotificarStockBajo();
        }

        private void CargarProductos()
        {
            try { ProductosLista = this.IProductosPresentacion.Consultar(); }
            catch (Exception ex) { Mensaje = ex.Message; }
        }

        private void NotificarStockBajo()
        {
            var productosBajos = Lista
                .Where(x => x.Cantidad <= StockMinimo)
                .Select(x => $"{x._Producto?.Nombre}: {x.Cantidad} unidades")
                .ToList();

            if (productosBajos.Count > 0)
                NotificacionStock = "Se necesitan productos para reponer el inventario: " +
                    string.Join(", ", productosBajos) +
                    ". Este aviso no bloquea las operaciones.";
        }
    }
}
