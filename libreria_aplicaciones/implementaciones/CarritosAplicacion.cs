using libreria_aplicaciones.entidades;
using libreria_aplicaciones.interfaces;
using Microsoft.EntityFrameworkCore;

namespace libreria_aplicaciones.implementaciones
{
    public class CarritosAplicacion : ICarritosAplicacion
    {
        private const string EstadoAbierto = "Abierto";
        private IConexion IConexion;

        public CarritosAplicacion(IConexion IConexion)
        {
            this.IConexion = IConexion;
        }

        public CarritoPublico Consultar(int idCliente)
        {
            var carrito = ObtenerCarritoAbierto(idCliente);
            return carrito == null ? new CarritoPublico() : CrearResumen(carrito);
        }

        public CarritoPublico AgregarProducto(int idCliente, DetalleCarritos entidad)
        {
            if (entidad.Producto == 0 || entidad.Cantidad <= 0)
                throw new Exception("Selecciona un producto y una cantidad válida");

            var producto = this.IConexion.Productos!
                .FirstOrDefault(x => x.IdProducto == entidad.Producto);
            if (producto == null ||
                !this.IConexion.Categorias!.Any(x => x.IdCategoria == producto.Categoria && x.Estado) ||
                !this.IConexion.Marcas!.Any(x => x.IdMarca == producto.Marca && x.Estado))
                throw new Exception("El producto no está disponible");

            var stock = ObtenerStock(producto.IdProducto);
            if (stock == 0)
                throw new Exception("Esta talla está agotada");

            var carrito = ObtenerOCrearCarrito(idCliente);
            var detalle = this.IConexion.DetalleCarritos!
                .FirstOrDefault(x => x.Carrito == carrito.IdCarrito && x.Producto == producto.IdProducto);
            var cantidadFinal = entidad.Cantidad + (detalle?.Cantidad ?? 0);
            if (cantidadFinal > stock)
                throw new Exception($"Solo hay {stock} unidades disponibles de esta talla");

            if (detalle == null)
            {
                detalle = new DetalleCarritos
                {
                    Carrito = carrito.IdCarrito,
                    Producto = producto.IdProducto,
                    Cantidad = cantidadFinal,
                    Subtotal = producto.Precio * cantidadFinal
                };
                this.IConexion.DetalleCarritos!.Add(detalle);
            }
            else
            {
                detalle.Cantidad = cantidadFinal;
                detalle.Subtotal = producto.Precio * cantidadFinal;
                this.IConexion.Entry<DetalleCarritos>(detalle).State = EntityState.Modified;
            }

            this.IConexion.SaveChanges();
            ActualizarTotal(carrito);
            this.IConexion.SaveChanges();
            return CrearResumen(carrito);
        }

        public CarritoPublico ActualizarProducto(int idCliente, DetalleCarritos entidad)
        {
            if (entidad.IdDetalleCarrito == 0 || entidad.Cantidad <= 0)
                throw new Exception("La cantidad debe ser mayor que cero");

            var carrito = ObtenerCarritoAbierto(idCliente);
            var detalle = carrito == null ? null : this.IConexion.DetalleCarritos!
                .FirstOrDefault(x => x.IdDetalleCarrito == entidad.IdDetalleCarrito && x.Carrito == carrito.IdCarrito);
            if (carrito == null || detalle == null)
                throw new Exception("No se encontró ese producto en tu carrito");

            var producto = this.IConexion.Productos!.FirstOrDefault(x => x.IdProducto == detalle.Producto);
            if (producto == null)
                throw new Exception("El producto ya no está disponible");

            var stock = ObtenerStock(producto.IdProducto);
            if (stock == 0)
                throw new Exception("Esta talla está agotada");
            if (entidad.Cantidad > stock)
                throw new Exception($"Solo hay {stock} unidades disponibles de esta talla");

            detalle.Cantidad = entidad.Cantidad;
            detalle.Subtotal = producto.Precio * entidad.Cantidad;
            this.IConexion.Entry<DetalleCarritos>(detalle).State = EntityState.Modified;
            this.IConexion.SaveChanges();
            ActualizarTotal(carrito);
            this.IConexion.SaveChanges();
            return CrearResumen(carrito);
        }

        public CarritoPublico BorrarProducto(int idCliente, int idDetalleCarrito)
        {
            var carrito = ObtenerCarritoAbierto(idCliente);
            var detalle = carrito == null ? null : this.IConexion.DetalleCarritos!
                .FirstOrDefault(x => x.IdDetalleCarrito == idDetalleCarrito && x.Carrito == carrito.IdCarrito);
            if (carrito == null || detalle == null)
                throw new Exception("No se encontró ese producto en tu carrito");

            this.IConexion.DetalleCarritos!.Remove(detalle);
            this.IConexion.SaveChanges();
            ActualizarTotal(carrito);
            this.IConexion.SaveChanges();
            return CrearResumen(carrito);
        }

        private Carritos? ObtenerCarritoAbierto(int idCliente)
        {
            return this.IConexion.Carritos!
                .FirstOrDefault(x => x.Cliente == idCliente && x.Estado == EstadoAbierto);
        }

        private Carritos ObtenerOCrearCarrito(int idCliente)
        {
            var carrito = ObtenerCarritoAbierto(idCliente);
            if (carrito != null) return carrito;

            carrito = new Carritos
            {
                FechaCreacion = DateTime.Now,
                Estado = EstadoAbierto,
                Total = 0,
                Cliente = idCliente
            };
            this.IConexion.Carritos!.Add(carrito);
            this.IConexion.SaveChanges();
            return carrito;
        }

        private int ObtenerStock(int idProducto)
        {
            return this.IConexion.Inventarios!
                .Where(x => x.Producto == idProducto)
                .Sum(x => (int?)x.Cantidad) ?? 0;
        }

        private void ActualizarTotal(Carritos carrito)
        {
            carrito.Total = this.IConexion.DetalleCarritos!
                .Where(x => x.Carrito == carrito.IdCarrito)
                .Sum(x => (decimal?)x.Subtotal) ?? 0;
            this.IConexion.Entry<Carritos>(carrito).State = EntityState.Modified;
        }

        private CarritoPublico CrearResumen(Carritos carrito)
        {
            return new CarritoPublico
            {
                IdCarrito = carrito.IdCarrito,
                Total = carrito.Total,
                Detalles = this.IConexion.DetalleCarritos!
                    .Where(x => x.Carrito == carrito.IdCarrito)
                    .Select(x => new DetalleCarritoPublico
                    {
                        IdDetalleCarrito = x.IdDetalleCarrito,
                        IdProducto = x.Producto,
                        Nombre = x._Producto!.Nombre,
                        Imagen = x._Producto!.Imagen,
                        NombreColor = x._Producto!._Color!.Nombre,
                        CodigoHexadecimal = x._Producto!._Color!.CodigoHexadecimal,
                        NombreTalla = x._Producto!._Talla!.Nombre,
                        SistemaMedida = x._Producto!._Talla!.SistemaMedida,
                        PrecioUnitario = x._Producto!.Precio,
                        Cantidad = x.Cantidad,
                        Subtotal = x.Subtotal,
                        StockDisponible = this.IConexion.Inventarios!
                            .Where(inventario => inventario.Producto == x.Producto)
                            .Sum(inventario => (int?)inventario.Cantidad) ?? 0
                    })
                    .ToList()
            };
        }
    }
}
