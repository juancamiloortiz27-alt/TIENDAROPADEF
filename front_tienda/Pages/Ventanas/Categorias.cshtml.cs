using libreria_aplicaciones.entidades;
using front_tienda.Utilidades;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using presentaciones_tienda.interfaces;

namespace front_tienda.Pages.Ventanas
{
    public class CategoriasModel : PageModel
    {
        private ICategoriasPresentacion ICategoriasPresentacion;
        private IWebHostEnvironment Entorno;

        public CategoriasModel(ICategoriasPresentacion ICategoriasPresentacion, IWebHostEnvironment Entorno)
        {
            this.ICategoriasPresentacion = ICategoriasPresentacion;
            this.Entorno = Entorno;
        }

        [BindProperty] public Categorias Entidad { get; set; } = new();
        [BindProperty] public IFormFile? ArchivoImagen { get; set; }
        [BindProperty] public List<Categorias> Lista { get; set; } = new();
        [BindProperty] public string? TextoBusqueda { get; set; }
        [BindProperty(SupportsGet = true)] public bool Gestionar { get; set; }
        public string? Mensaje { get; set; }
        public bool EsAdministrador => HttpContext.Session.GetString("Rol") == "Administrador";
        public bool VistaGestion => Gestionar && EsAdministrador;

        public void OnGet()
        {
            Consultar();
        }

        public void OnPostConsultar()
        {
            Consultar();
        }

        public IActionResult OnPostLimpiar()
        {
            if (!EsAdministrador) return Forbid();
            Entidad = new Categorias();
            TextoBusqueda = null;
            ModelState.Clear();
            Consultar();
            return Page();
        }

        public void OnPostBuscar()
        {
            try
            {
                Lista = this.ICategoriasPresentacion.Buscar(new Categorias { Nombre = TextoBusqueda })
                    .Where(x => VistaGestion || x.Estado).ToList();
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
            }
        }

        public IActionResult OnPostInsertar()
        {
            if (!EsAdministrador) return Forbid();
            string? imagenGuardada = null;
            try
            {
                if (ArchivoImagen != null)
                {
                    imagenGuardada = AlmacenImagenes.Guardar(ArchivoImagen, Entorno, "categorias");
                    Entidad.Imagen = imagenGuardada;
                }
                this.ICategoriasPresentacion.Insertar(Entidad);
                Mensaje = "Categoría insertada";
                Entidad = new Categorias();
                ModelState.Clear();
            }
            catch (Exception ex)
            {
                AlmacenImagenes.Eliminar(imagenGuardada, Entorno, "categorias");
                if (imagenGuardada != null) Entidad.Imagen = null;
                Mensaje = ex.Message;
            }
            Consultar();
            return Page();
        }

        public IActionResult OnPostSeleccionar(int id)
        {
            if (!EsAdministrador) return Forbid();
            Consultar();
            Entidad = Lista.FirstOrDefault(x => x.IdCategoria == id) ?? new Categorias();
            ModelState.Clear();
            return Page();
        }

        public IActionResult OnPostActualizar()
        {
            if (!EsAdministrador) return Forbid();
            var imagenAnterior = Entidad.Imagen;
            string? imagenGuardada = null;
            try
            {
                if (ArchivoImagen != null)
                {
                    imagenGuardada = AlmacenImagenes.Guardar(ArchivoImagen, Entorno, "categorias");
                    Entidad.Imagen = imagenGuardada;
                }
                this.ICategoriasPresentacion.Actualizar(Entidad);
                Mensaje = "Categoría actualizada";
                Entidad = new Categorias();
                ModelState.Clear();
            }
            catch (Exception ex)
            {
                AlmacenImagenes.Eliminar(imagenGuardada, Entorno, "categorias");
                Entidad.Imagen = imagenAnterior;
                Mensaje = ex.Message;
            }
            Consultar();
            return Page();
        }

        public IActionResult OnPostBorrar(int id)
        {
            if (!EsAdministrador) return Forbid();
            try
            {
                this.ICategoriasPresentacion.Borrar(new Categorias { IdCategoria = id });
                Mensaje = "Categoría borrada";
                Entidad = new Categorias();
                ModelState.Clear();
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
            }
            Consultar();
            return Page();
        }

        private void Consultar()
        {
            try
            {
                Lista = this.ICategoriasPresentacion.Consultar()
                    .Where(x => VistaGestion || x.Estado).ToList();
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
            }
        }
    }
}
