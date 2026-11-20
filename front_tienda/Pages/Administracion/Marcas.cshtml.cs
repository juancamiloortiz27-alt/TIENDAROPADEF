using libreria_aplicaciones.entidades;
using front_tienda.Utilidades;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using presentaciones_tienda.interfaces;

namespace front_tienda.Pages.Administracion
{
    public class MarcasModel : PageModel
    {
        private IMarcasPresentacion IMarcasPresentacion;
        private IWebHostEnvironment Entorno;

        public MarcasModel(IMarcasPresentacion IMarcasPresentacion, IWebHostEnvironment Entorno)
        {
            this.IMarcasPresentacion = IMarcasPresentacion;
            this.Entorno = Entorno;
        }

        [BindProperty] public Marcas Entidad { get; set; } = new();
        [BindProperty] public Marcas Filtro { get; set; } = new();
        [BindProperty] public IFormFile? ArchivoImagen { get; set; }
        public List<Marcas> Lista { get; set; } = new();
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
            Entidad = new Marcas();
            Filtro = new Marcas();
            ModelState.Clear();
            Consultar();
            return Page();
        }

        public IActionResult OnPostBuscar()
        {
            if (!EsAdministrador()) return Forbid();
            try { Lista = this.IMarcasPresentacion.Buscar(Filtro); }
            catch (Exception ex) { Mensaje = ex.Message; }
            return Page();
        }

        public IActionResult OnPostSeleccionar(int id)
        {
            if (!EsAdministrador()) return Forbid();
            Consultar();
            Entidad = Lista.FirstOrDefault(x => x.IdMarca == id) ?? new Marcas();
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
                    imagenGuardada = AlmacenImagenes.Guardar(ArchivoImagen, Entorno, "marcas");
                    Entidad.Imagen = imagenGuardada;
                }
                this.IMarcasPresentacion.Insertar(Entidad);
                Mensaje = "Marca insertada";
                Entidad = new Marcas();
                ModelState.Clear();
            }
            catch (Exception ex)
            {
                AlmacenImagenes.Eliminar(imagenGuardada, Entorno, "marcas");
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
                    imagenGuardada = AlmacenImagenes.Guardar(ArchivoImagen, Entorno, "marcas");
                    Entidad.Imagen = imagenGuardada;
                }
                this.IMarcasPresentacion.Actualizar(Entidad);
                Mensaje = "Marca actualizada";
                Entidad = new Marcas();
                ModelState.Clear();
            }
            catch (Exception ex)
            {
                AlmacenImagenes.Eliminar(imagenGuardada, Entorno, "marcas");
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
                this.IMarcasPresentacion.Borrar(new Marcas { IdMarca = id });
                Mensaje = "Marca borrada";
                Entidad = new Marcas();
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
            try { Lista = this.IMarcasPresentacion.Consultar(); }
            catch (Exception ex) { Mensaje = ex.Message; }
        }
    }
}
