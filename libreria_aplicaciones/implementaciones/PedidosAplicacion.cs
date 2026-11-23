using libreria_aplicaciones.entidades;
using libreria_aplicaciones.interfaces;

namespace libreria_aplicaciones.implementaciones
{
    public class PedidosAplicacion : IPedidosAplicacion
    {
        private IConexion IConexion;

        public PedidosAplicacion(IConexion IConexion)
        {
            this.IConexion = IConexion;
        }

        public List<Pedidos> ConsultarMios(int idCliente)
        {
            var pedidos = this.IConexion.Pedidos!
                .Where(x => x.Cliente == idCliente)
                .OrderByDescending(x => x.Fecha)
                .ThenByDescending(x => x.IdPedido)
                .ToList();

            foreach (var pedido in pedidos)
            {
                pedido._Cliente = null;
                pedido._Cupon = pedido.Cupon.HasValue
                    ? this.IConexion.Cupones!
                        .Where(x => x.IdCupon == pedido.Cupon.Value)
                        .Select(x => new Cupones
                        {
                            IdCupon = x.IdCupon,
                            Codigo = x.Codigo,
                            PorcentajeDescuento = x.PorcentajeDescuento
                        })
                        .FirstOrDefault()
                    : null;

                pedido.DetallePedidos = this.IConexion.DetallePedidos!
                    .Where(x => x.Pedido == pedido.IdPedido)
                    .ToList();
                foreach (var detalle in pedido.DetallePedidos)
                {
                    detalle._Pedido = null;
                    var producto = this.IConexion.Productos!
                        .FirstOrDefault(x => x.IdProducto == detalle.Producto);
                    if (producto == null) continue;

                    detalle._Producto = new Productos
                    {
                        IdProducto = producto.IdProducto,
                        Nombre = producto.Nombre,
                        CodigoModelo = producto.CodigoModelo,
                        Color = producto.Color,
                        Talla = producto.Talla,
                        _Color = this.IConexion.Colores!
                            .Where(x => x.IdColor == producto.Color)
                            .Select(x => new Colores
                            {
                                IdColor = x.IdColor,
                                Nombre = x.Nombre,
                                CodigoHexadecimal = x.CodigoHexadecimal
                            })
                            .FirstOrDefault(),
                        _Talla = this.IConexion.Tallas!
                            .Where(x => x.IdTalla == producto.Talla)
                            .Select(x => new Tallas
                            {
                                IdTalla = x.IdTalla,
                                Nombre = x.Nombre,
                                SistemaMedida = x.SistemaMedida
                            })
                            .FirstOrDefault()
                    };
                }

                pedido.Pagos = this.IConexion.Pagos!
                    .Where(x => x.Pedido == pedido.IdPedido)
                    .ToList();
                foreach (var pago in pedido.Pagos)
                {
                    pago._Pedido = null;
                    pago._MetodoPago = this.IConexion.MetodoPagos!
                        .Where(x => x.IdMetodoPago == pago.MetodoPago)
                        .Select(x => new MetodoPagos
                        {
                            IdMetodoPago = x.IdMetodoPago,
                            Nombre = x.Nombre,
                            Tipo = x.Tipo
                        })
                        .FirstOrDefault();
                }
                pedido.Envios = this.IConexion.Envios!
                    .Where(x => x.Pedido == pedido.IdPedido)
                    .ToList();
                foreach (var envio in pedido.Envios)
                {
                    envio._Pedido = null;
                    envio._Transportadora = this.IConexion.Transportadoras!
                        .Where(x => x.IdTransportadoras == envio.Transportadora)
                        .Select(x => new Transportadoras
                        {
                            IdTransportadoras = x.IdTransportadoras,
                            Nombre = x.Nombre,
                            Telefono = x.Telefono,
                            Correo = x.Correo,
                            Ciudad = x.Ciudad
                        })
                        .FirstOrDefault();
                }
            }

            return pedidos;
        }

        public Pedidos Confirmar(int idCliente, ConfirmarPedido entidad)
        {
            if (entidad.IdDireccion == 0)
                throw new Exception("Selecciona una dirección de entrega");

            var direccion = this.IConexion.Direcciones!
                .FirstOrDefault(x => x.IdDireccion == entidad.IdDireccion && x.Cliente == idCliente);
            if (direccion == null)
                throw new Exception("La dirección seleccionada no pertenece a tu cuenta");

            var metodoPago = this.IConexion.MetodoPagos!
                .FirstOrDefault(x => x.IdMetodoPago == entidad.IdMetodoPago && x.Estado);
            if (metodoPago == null)
                throw new Exception("Selecciona un método de pago disponible");

            var carrito = this.IConexion.Carritos!
                .FirstOrDefault(x => x.Cliente == idCliente && x.Estado == "Abierto");
            if (carrito == null)
                throw new Exception("Tu carrito está vacío");

            var detalles = this.IConexion.DetalleCarritos!
                .Where(x => x.Carrito == carrito.IdCarrito)
                .ToList();
            if (detalles.Count == 0)
                throw new Exception("Agrega productos antes de confirmar el pedido");

            Cupones? cupon = null;
            if (!string.IsNullOrWhiteSpace(entidad.CodigoCupon))
            {
                cupon = ObtenerCuponActivo(entidad.CodigoCupon);
            }

            var productos = new Dictionary<int, Productos>();
            var inventarios = new Dictionary<int, List<Inventarios>>();
            foreach (var detalle in detalles)
            {
                var producto = this.IConexion.Productos!
                    .FirstOrDefault(x => x.IdProducto == detalle.Producto);
                if (producto == null ||
                    !this.IConexion.Categorias!.Any(x => x.IdCategoria == producto.Categoria && x.Estado) ||
                    !this.IConexion.Marcas!.Any(x => x.IdMarca == producto.Marca && x.Estado))
                    throw new Exception("Uno de los productos del carrito ya no está disponible");

                var listaInventario = this.IConexion.Inventarios!
                    .Where(x => x.Producto == producto.IdProducto && x.Cantidad > 0)
                    .OrderBy(x => x.IdInventario)
                    .ToList();
                var cantidadDisponible = listaInventario.Sum(x => x.Cantidad);
                if (detalle.Cantidad > cantidadDisponible)
                    throw new Exception($"Stock insuficiente para {producto.Nombre}. Disponibles: {cantidadDisponible}");

                productos[producto.IdProducto] = producto;
                inventarios[producto.IdProducto] = listaInventario;
            }

            var subtotal = detalles.Sum(x => productos[x.Producto].Precio * x.Cantidad);
            var total = cupon == null
                ? subtotal
                : decimal.Round(subtotal * (100 - cupon.PorcentajeDescuento) / 100, 2);

            using var transaccion = this.IConexion.Database.BeginTransaction();
            try
            {
                var pedido = new Pedidos
                {
                    Fecha = DateTime.Now,
                    Estado = "Pagado",
                    Total = total,
                    Cliente = idCliente,
                    Cupon = cupon?.IdCupon,
                    CiudadEnvio = direccion.Ciudad,
                    DireccionEnvio = direccion.Direccion,
                    CodigoPostalEnvio = direccion.CodigoPostal
                };
                this.IConexion.Pedidos!.Add(pedido);
                this.IConexion.SaveChanges();

                var pago = new Pagos
                {
                    FechaPago = DateTime.Now,
                    Monto = total,
                    Pedido = pedido.IdPedido,
                    MetodoPago = metodoPago.IdMetodoPago
                };
                this.IConexion.Pagos!.Add(pago);

                foreach (var detalle in detalles)
                {
                    this.IConexion.DetallePedidos!.Add(new DetallePedidos
                    {
                        Cantidad = detalle.Cantidad,
                        Precio = productos[detalle.Producto].Precio,
                        Producto = detalle.Producto,
                        Pedido = pedido.IdPedido
                    });

                    var restante = detalle.Cantidad;
                    foreach (var inventario in inventarios[detalle.Producto])
                    {
                        var descontar = Math.Min(restante, inventario.Cantidad);
                        inventario.Cantidad -= descontar;
                        this.IConexion.Entry<Inventarios>(inventario).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        restante -= descontar;
                        if (restante == 0) break;
                    }
                }

                carrito.Estado = "Convertido";
                this.IConexion.Entry<Carritos>(carrito).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                this.IConexion.SaveChanges();
                transaccion.Commit();

                pedido._Cliente = null;
                pedido._Cupon = null;
                pedido.DetallePedidos = null;
                pago._Pedido = null;
                pago._MetodoPago = metodoPago;
                metodoPago.Pago = null;
                pedido.Pagos = new List<Pagos> { pago };
                pedido.Envios = null;
                return pedido;
            }
            catch
            {
                transaccion.Rollback();
                throw;
            }
        }

        public CuponValidado ValidarCupon(int idCliente, string? codigoCupon)
        {
            var cupon = ObtenerCuponActivo(codigoCupon);
            var carrito = this.IConexion.Carritos!
                .FirstOrDefault(x => x.Cliente == idCliente && x.Estado == "Abierto");
            if (carrito == null)
                throw new Exception("Tu carrito está vacío");
            var detalles = this.IConexion.DetalleCarritos!
                .Where(x => x.Carrito == carrito.IdCarrito)
                .ToList();
            if (detalles.Count == 0)
                throw new Exception("Agrega productos antes de aplicar un cupón");

            var subtotal = detalles.Sum(detalle =>
            {
                var producto = this.IConexion.Productos!
                    .FirstOrDefault(x => x.IdProducto == detalle.Producto);
                if (producto == null)
                    throw new Exception("Uno de los productos del carrito ya no está disponible");
                return producto.Precio * detalle.Cantidad;
            });
            var descuento = decimal.Round(subtotal * cupon.PorcentajeDescuento / 100, 2);
            return new CuponValidado
            {
                Codigo = cupon.Codigo,
                PorcentajeDescuento = cupon.PorcentajeDescuento,
                Descuento = descuento,
                TotalConDescuento = subtotal - descuento
            };
        }

        private Cupones ObtenerCuponActivo(string? codigoIngresado)
        {
            if (string.IsNullOrWhiteSpace(codigoIngresado))
                throw new Exception("Escribe un código de cupón");
            var codigo = codigoIngresado.Trim();
            var cupon = this.IConexion.Cupones!
                .FirstOrDefault(x => x.Codigo == codigo && x.Estado && x.FechaVence.Date >= DateTime.Today);
            if (cupon == null)
                throw new Exception("El cupón no existe, está vencido o está desactivado");
            if (cupon.PorcentajeDescuento <= 0 || cupon.PorcentajeDescuento > 100)
                throw new Exception("El porcentaje de descuento del cupón no es válido");
            return cupon;
        }
    }
}
