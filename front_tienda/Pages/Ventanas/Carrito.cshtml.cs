using libreria_aplicaciones.entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using presentaciones_tienda.interfaces;

namespace front_tienda.Pages.Ventanas
{
    public class CarritoModel : PageModel
    {
        private ICarritosPresentacion ICarritosPresentacion;
        private IDireccionesPresentacion IDireccionesPresentacion;
        private IPedidosPresentacion IPedidosPresentacion;
        private IMetodoPagosPresentacion IMetodoPagosPresentacion;

        public CarritoModel(ICarritosPresentacion ICarritosPresentacion,
            IDireccionesPresentacion IDireccionesPresentacion,
            IPedidosPresentacion IPedidosPresentacion,
            IMetodoPagosPresentacion IMetodoPagosPresentacion)
        {
            this.ICarritosPresentacion = ICarritosPresentacion;
            this.IDireccionesPresentacion = IDireccionesPresentacion;
            this.IPedidosPresentacion = IPedidosPresentacion;
            this.IMetodoPagosPresentacion = IMetodoPagosPresentacion;
        }

        public CarritoPublico Carrito { get; set; } = new();
        public List<Direcciones> Direcciones { get; set; } = new();
        public List<MetodoPagos> MetodosPago { get; set; } = new();
        public Pedidos? PedidoConfirmado { get; set; }
        public CuponValidado? CuponAplicado { get; set; }
        [BindProperty] public int IdDireccion { get; set; }
        [BindProperty] public int IdMetodoPago { get; set; }
        [BindProperty] public string? CodigoCupon { get; set; }
        public string? Mensaje { get; set; }

        public IActionResult OnGet()
        {
            if (!EsCliente())
                return RedirectToPage("/Cuenta/Acceso", new { returnUrl = Request.Path + Request.QueryString });
            Cargar();
            return Page();
        }

        public IActionResult OnPostActualizar(int idDetalleCarrito, int cantidad)
        {
            if (!EsCliente()) return Forbid();
            try
            {
                this.ICarritosPresentacion.ActualizarProducto(new DetalleCarritos
                {
                    IdDetalleCarrito = idDetalleCarrito,
                    Cantidad = cantidad
                });
                Mensaje = "Cantidad actualizada";
            }
            catch (Exception ex) { Mensaje = ex.Message; }
            Cargar();
            return Page();
        }

        public IActionResult OnPostBorrar(int id)
        {
            if (!EsCliente()) return Forbid();
            try
            {
                this.ICarritosPresentacion.BorrarProducto(id);
                Mensaje = "Producto eliminado del carrito";
            }
            catch (Exception ex) { Mensaje = ex.Message; }
            Cargar();
            return Page();
        }

        public IActionResult OnPostConfirmar()
        {
            if (!EsCliente()) return Forbid();
            try
            {
                PedidoConfirmado = this.IPedidosPresentacion.Confirmar(new ConfirmarPedido
                {
                    IdDireccion = IdDireccion,
                    IdMetodoPago = IdMetodoPago,
                    CodigoCupon = CodigoCupon
                });
                Mensaje = "Pago aprobado. Tu pedido está pagado";
                CodigoCupon = null;
                IdDireccion = 0;
                IdMetodoPago = 0;
            }
            catch (Exception ex) { Mensaje = ex.Message; }
            Cargar();
            return Page();
        }

        public IActionResult OnPostAplicarCupon()
        {
            if (!EsCliente()) return Forbid();
            try
            {
                CuponAplicado = this.IPedidosPresentacion.ValidarCupon(CodigoCupon ?? "");
                Mensaje = $"Cupón {CuponAplicado.Codigo} aplicado: {CuponAplicado.PorcentajeDescuento}% de descuento";
            }
            catch (Exception ex) { Mensaje = ex.Message; }
            Cargar();
            return Page();
        }

        private bool EsCliente()
        {
            return HttpContext.Session.GetString("Rol") == "Cliente" &&
                HttpContext.Session.GetInt32("Cliente").HasValue;
        }

        private void Cargar()
        {
            try { Carrito = this.ICarritosPresentacion.Consultar(); }
            catch (Exception ex) { Mensaje = ex.Message; }
            try { Direcciones = this.IDireccionesPresentacion.Consultar(); }
            catch (Exception ex) { Mensaje = ex.Message; }
            try { MetodosPago = this.IMetodoPagosPresentacion.ConsultarActivos(); }
            catch (Exception ex) { Mensaje = ex.Message; }
        }
    }
}
