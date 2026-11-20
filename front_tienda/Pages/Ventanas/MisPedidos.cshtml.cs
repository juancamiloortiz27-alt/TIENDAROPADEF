using libreria_aplicaciones.entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using presentaciones_tienda.interfaces;
using front_tienda.Utilidades;

namespace front_tienda.Pages.Ventanas
{
    public class MisPedidosModel : PageModel
    {
        private IPedidosPresentacion IPedidosPresentacion;

        public MisPedidosModel(IPedidosPresentacion IPedidosPresentacion)
        {
            this.IPedidosPresentacion = IPedidosPresentacion;
        }

        public List<Pedidos> Lista { get; set; } = new();
        public string? Mensaje { get; set; }

        public IActionResult OnGet()
        {
            if (!EsCliente())
                return RedirectToPage("/Cuenta/Acceso", new { returnUrl = Request.Path.ToString() });
            try { Lista = this.IPedidosPresentacion.ConsultarMios(); }
            catch (Exception ex) { Mensaje = ex.Message; }
            return Page();
        }

        public IActionResult OnGetDescargarExcel()
        {
            if (!EsCliente())
                return RedirectToPage("/Cuenta/Acceso", new { returnUrl = Request.Path.ToString() });

            try
            {
                var pedidos = this.IPedidosPresentacion.ConsultarMios();
                var filas = new List<IReadOnlyList<object?>>();
                foreach (var pedido in pedidos)
                {
                    var detalles = pedido.DetallePedidos ?? new List<libreria_aplicaciones.entidades.DetallePedidos>();
                    if (detalles.Count == 0)
                    {
                        filas.Add(new object?[] { pedido.IdPedido, pedido.Fecha.ToString("yyyy-MM-dd HH:mm:ss"), pedido.Estado, null, null, null, null, null, null, pedido.Total, null, null, pedido._Cupon?.Codigo, pedido._Cupon?.PorcentajeDescuento, null, null, null, pedido.DireccionEnvio, pedido.CiudadEnvio, pedido.CodigoPostalEnvio });
                        continue;
                    }

                    foreach (var detalle in detalles)
                    {
                        var pago = pedido.Pagos?.FirstOrDefault();
                        var envio = pedido.Envios?.FirstOrDefault();
                        filas.Add(new object?[]
                        {
                            pedido.IdPedido, pedido.Fecha.ToString("yyyy-MM-dd HH:mm:ss"), pedido.Estado,
                            detalle._Producto?.Nombre, detalle._Producto?._Color?.Nombre,
                            detalle._Producto?._Talla?.Nombre, detalle.Precio, detalle.Cantidad,
                            detalle.Precio * detalle.Cantidad, pedido.Total,
                            pago?._MetodoPago?.Nombre, pago?.Monto,
                            pedido._Cupon?.Codigo, pedido._Cupon?.PorcentajeDescuento,
                            envio?.Estado, envio?._Transportadora?.Nombre, envio?.NumeroGuia,
                            pedido.DireccionEnvio, pedido.CiudadEnvio, pedido.CodigoPostalEnvio
                        });
                    }
                }

                var encabezados = new[] { "Pedido", "Fecha", "Estado pedido", "Producto", "Color", "Talla", "Precio unitario (COP)", "Cantidad", "Subtotal (COP)", "Total pedido (COP)", "Método de pago", "Pago (COP)", "Cupón", "Descuento (%)", "Estado envío", "Transportadora", "Número de guía", "Dirección", "Ciudad", "Código postal" };
                var contenido = ExcelExportador.Crear("Mis compras", encabezados, filas);
                return File(contenido, ExcelExportador.TipoContenido, $"mis-compras-{DateTime.Now:yyyyMMdd}.xlsx");
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
                return Page();
            }
        }

        private bool EsCliente()
        {
            return HttpContext.Session.GetString("Rol") == "Cliente" &&
                HttpContext.Session.GetInt32("Cliente").HasValue;
        }
    }
}
