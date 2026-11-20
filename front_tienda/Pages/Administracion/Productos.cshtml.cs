using libreria_aplicaciones.entidades;
using front_tienda.Utilidades;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using presentaciones_tienda.interfaces;

namespace front_tienda.Pages.Administracion
{
    public class ProductosModel : PageModel
    {
        private IProductosPresentacion IProductosPresentacion;
        private ICategoriasPresentacion ICategoriasPresentacion;
        private IMarcasPresentacion IMarcasPresentacion;
        private IColoresPresentacion IColoresPresentacion;
        private ITallasPresentacion ITallasPresentacion;
        private IProveedoresPresentacion IProveedoresPresentacion;
        private IWebHostEnvironment Entorno;

        public ProductosModel(
            IProductosPresentacion IProductosPresentacion,
            ICategoriasPresentacion ICategoriasPresentacion,
            IMarcasPresentacion IMarcasPresentacion,
            IColoresPresentacion IColoresPresentacion,
            ITallasPresentacion ITallasPresentacion,
            IProveedoresPresentacion IProveedoresPresentacion,
            IWebHostEnvironment Entorno)
        {
            this.IProductosPresentacion = IProductosPresentacion;
            this.ICategoriasPresentacion = ICategoriasPresentacion;
            this.IMarcasPresentacion = IMarcasPresentacion;
            this.IColoresPresentacion = IColoresPresentacion;
            this.ITallasPresentacion = ITallasPresentacion;
            this.IProveedoresPresentacion = IProveedoresPresentacion;
            this.Entorno = Entorno;
        }

        [BindProperty] public Productos Entidad { get; set; } = new();
        [BindProperty] public Productos Filtro { get; set; } = new();
        [BindProperty] public IFormFile? ArchivoImagen { get; set; }
        public List<Productos> Lista { get; set; } = new();
        public List<Categorias> CategoriasLista { get; set; } = new();
        public List<Marcas> MarcasLista { get; set; } = new();
        public List<Colores> ColoresLista { get; set; } = new();
        public List<Tallas> TallasLista { get; set; } = new();
        public List<Proveedores> ProveedoresLista { get; set; } = new();
        public string? Mensaje { get; set; }

        public IActionResult OnGet()
        {
            if (!EsAdministrador()) return Forbid();
            Consultar();
            return Page();
        }

        public IActionResult OnPostLimpiar()
        {
            if (!EsAdministrador()) return Forbid();
            Entidad = new Productos();
            Filtro = new Productos();
            ModelState.Clear();
            Consultar();
            return Page();
        }

        public IActionResult OnPostBuscar()
        {
            if (!EsAdministrador()) return Forbid();
            try { Lista = this.IProductosPresentacion.Buscar(Filtro); }
            catch (Exception ex) { Mensaje = ex.Message; }
            CargarListas();
            return Page();
        }

        public IActionResult OnPostSeleccionar(int id)
        {
            if (!EsAdministrador()) return Forbid();
            Consultar();
            Entidad = Lista.FirstOrDefault(x => x.IdProducto == id) ?? new Productos();
            return Page();
        }

        public IActionResult OnPostNuevaTalla(int id)
        {
            if (!EsAdministrador()) return Forbid();
            Consultar();
            var producto = Lista.FirstOrDefault(x => x.IdProducto == id);
            if (producto == null)
            {
                Mensaje = "No se encontró el producto";
                return Page();
            }

            Entidad = new Productos
            {
                CodigoModelo = producto.CodigoModelo,
                Nombre = producto.Nombre,
                Precio = producto.Precio,
                Descripcion = producto.Descripcion,
                Imagen = producto.Imagen,
                Categoria = producto.Categoria,
                Marca = producto.Marca,
                Color = producto.Color,
                Proveedor = producto.Proveedor
            };
            Mensaje = "El modelo se copió. Selecciona una talla nueva y guarda la variante.";
            ModelState.Clear();
            return Page();
        }

        public IActionResult OnPostInsertar()
        {
            if (!EsAdministrador()) return Forbid();
            var imagenAnterior = Entidad.Imagen;
            string? imagenGuardada = null;
            try
            {
                if (ArchivoImagen != null)
                {
                    imagenGuardada = AlmacenImagenes.Guardar(ArchivoImagen, Entorno, "productos");
                    Entidad.Imagen = imagenGuardada;
                }
                this.IProductosPresentacion.Insertar(Entidad);
                Mensaje = "Producto insertado";
                Entidad = new Productos();
                ModelState.Clear();
            }
            catch (Exception ex)
            {
                AlmacenImagenes.Eliminar(imagenGuardada, Entorno, "productos");
                Entidad.Imagen = imagenAnterior;
                Mensaje = ex.Message;
            }
            Consultar();
            return Page();
        }

        public IActionResult OnPostActualizar()
        {
            if (!EsAdministrador()) return Forbid();
            var imagenAnterior = Entidad.Imagen;
            string? imagenGuardada = null;
            try
            {
                if (ArchivoImagen != null)
                {
                    imagenGuardada = AlmacenImagenes.Guardar(ArchivoImagen, Entorno, "productos");
                    Entidad.Imagen = imagenGuardada;
                }
                this.IProductosPresentacion.Actualizar(Entidad);
                Mensaje = "Producto actualizado";
                Entidad = new Productos();
                ModelState.Clear();
            }
            catch (Exception ex)
            {
                AlmacenImagenes.Eliminar(imagenGuardada, Entorno, "productos");
                Entidad.Imagen = imagenAnterior;
                Mensaje = ex.Message;
            }
            Consultar();
            return Page();
        }

        public IActionResult OnPostBorrar(int id)
        {
            if (!EsAdministrador()) return Forbid();
            try
            {
                this.IProductosPresentacion.Borrar(new Productos { IdProducto = id });
                Mensaje = "Producto borrado";
                Entidad = new Productos();
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
            try { Lista = this.IProductosPresentacion.Consultar(); }
            catch (Exception ex) { Mensaje = ex.Message; }
            CargarListas();
        }

        private void CargarListas()
        {
            try { CategoriasLista = this.ICategoriasPresentacion.Consultar(); }
            catch (Exception ex) { Mensaje = ex.Message; }
            try { MarcasLista = this.IMarcasPresentacion.Consultar(); }
            catch (Exception ex) { Mensaje = ex.Message; }
            try { ColoresLista = this.IColoresPresentacion.Consultar(); }
            catch (Exception ex) { Mensaje = ex.Message; }
            try { TallasLista = this.ITallasPresentacion.Consultar(); }
            catch (Exception ex) { Mensaje = ex.Message; }
            try { ProveedoresLista = this.IProveedoresPresentacion.Consultar(); }
            catch (Exception ex) { Mensaje = ex.Message; }
        }
    }
}
