using libreria_aplicaciones.entidades;
using libreria_aplicaciones.interfaces;

namespace libreria_aplicaciones.implementaciones
{
    public class ReseñasAplicacion : IReseñasAplicacion
    {
        private IConexion IConexion;

        public ReseñasAplicacion(IConexion IConexion)
        {
            this.IConexion = IConexion;
        }

        public List<Reseñas> ConsultarPorProducto(int idProducto)
        {
            var producto = this.IConexion.Productos!
                .FirstOrDefault(x => x.IdProducto == idProducto);
            if (producto == null)
                throw new Exception("No se encontró el producto");

            var productosModelo = this.IConexion.Productos!
                .Where(x => x.CodigoModelo == producto.CodigoModelo)
                .Select(x => x.IdProducto)
                .ToList();
            var reseñas = this.IConexion.Reseñas!
                .Where(x => productosModelo.Contains(x.Producto))
                .OrderByDescending(x => x.IdReseña)
                .ToList();

            foreach (var reseña in reseñas)
            {
                reseña._Cliente = this.IConexion.Clientes!
                    .Where(x => x.IdCliente == reseña.Cliente)
                    .Select(x => new Clientes { Nombre = x.Nombre })
                    .FirstOrDefault();
                reseña._Producto = null;
            }
            return reseñas;
        }

        public Reseñas Insertar(int idCliente, Reseñas entidad)
        {
            if (entidad.IdReseña != 0)
                throw new Exception("La reseña ya fue guardada");
            if (entidad.Calificacion < 1 || entidad.Calificacion > 5 || entidad.Calificacion % 1 != 0)
                throw new Exception("La calificación debe ser un número entero entre 1 y 5");
            if (string.IsNullOrWhiteSpace(entidad.Comentario) || entidad.Comentario.Trim().Length > 200)
                throw new Exception("Escribe un comentario de hasta 200 caracteres");

            var producto = this.IConexion.Productos!
                .FirstOrDefault(x => x.IdProducto == entidad.Producto);
            if (producto == null)
                throw new Exception("No se encontró el producto");

            var productoIds = this.IConexion.Productos!
                .Where(x => x.CodigoModelo == producto.CodigoModelo)
                .Select(x => x.IdProducto);
            var compraPagada = this.IConexion.DetallePedidos!.Any(detalle =>
                productoIds.Contains(detalle.Producto) &&
                this.IConexion.Pedidos!.Any(pedido =>
                    pedido.IdPedido == detalle.Pedido &&
                    pedido.Cliente == idCliente &&
                    pedido.Estado == "Pagado"));
            if (!compraPagada)
                throw new Exception("Solo puedes reseñar una talla que hayas comprado en un pedido pagado");

            if (this.IConexion.Reseñas!.Any(x => x.Cliente == idCliente && productoIds.Contains(x.Producto)))
                throw new Exception("Ya publicaste una reseña para este producto");

            entidad.Cliente = idCliente;
            entidad.Comentario = entidad.Comentario.Trim();
            entidad._Cliente = null;
            entidad._Producto = null;
            this.IConexion.Reseñas!.Add(entidad);
            this.IConexion.SaveChanges();
            return entidad;
        }

        public Reseñas Borrar(Reseñas entidad)
        {
            if (entidad.IdReseña == 0)
                throw new Exception("Selecciona una reseña para borrar");
            var reseña = this.IConexion.Reseñas!
                .FirstOrDefault(x => x.IdReseña == entidad.IdReseña);
            if (reseña == null)
                throw new Exception("No se encontró la reseña");
            this.IConexion.Reseñas!.Remove(reseña);
            this.IConexion.SaveChanges();
            return reseña;
        }
    }
}
