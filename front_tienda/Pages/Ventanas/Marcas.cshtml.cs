using libreria_aplicaciones.entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using presentaciones_tienda.interfaces;

namespace front_tienda.Pages.Ventanas
{
    public class MarcasModel : PageModel
    {
        private IMarcasPresentacion IMarcasPresentacion;

        public MarcasModel(IMarcasPresentacion IMarcasPresentacion)
        {
            this.IMarcasPresentacion = IMarcasPresentacion;
        }

        [BindProperty] public string? TextoBusqueda { get; set; }
        public List<Marcas> Lista { get; set; } = new();

        public void OnGet()
        {
            Lista = this.IMarcasPresentacion.ConsultarActivas();
        }

        public void OnPostBuscar()
        {
            if (string.IsNullOrWhiteSpace(TextoBusqueda))
                Lista = this.IMarcasPresentacion.ConsultarActivas();
            else
                Lista = this.IMarcasPresentacion.BuscarActivas(new Marcas { Nombre = TextoBusqueda });
        }
    }
}
