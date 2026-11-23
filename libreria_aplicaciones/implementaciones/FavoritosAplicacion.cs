using libreria_aplicaciones.entidades;
using libreria_aplicaciones.interfaces;
using Microsoft.EntityFrameworkCore;

namespace libreria_aplicaciones.implementaciones
{
    public class FavoritosAplicacion : IFavoritosAplicacion
    {
        private IConexion IConexion;

        public FavoritosAplicacion(IConexion IConexion)
        {
            this.IConexion = IConexion;
        }

        public List<Favoritos> Consultar(int idCliente)
        {
            var favoritos = this.IConexion.Favoritos!
                .Where(x => x.Cliente == idCliente && x.Estado)
                .OrderByDescending(x => x.IdFavorito)
                .ToList();
            foreach (var favorito in favoritos)
            {
                favorito._Cliente = null;
                favorito._Producto = ObtenerProductoPublico(favorito.Producto);
            }
            return favoritos;
        }

        public Favoritos Agregar(int idCliente, Favoritos entidad)
        {
            var producto = ObtenerProducto(entidad.Producto);
            ValidarDisponible(producto);
            var idsDelModelo = this.IConexion.Productos!
                .Where(x => x.CodigoModelo == producto.CodigoModelo)
                .Select(x => x.IdProducto);
            var favorito = this.IConexion.Favoritos!
                .FirstOrDefault(x => x.Cliente == idCliente && idsDelModelo.Contains(x.Producto));

            if (favorito == null)
            {
                favorito = new Favoritos
                {
                    Cliente = idCliente,
                    Producto = producto.IdProducto,
                    Estado = true
                };
                this.IConexion.Favoritos!.Add(favorito);
            }
            else
            {
                favorito.Estado = true;
                this.IConexion.Entry<Favoritos>(favorito).State = EntityState.Modified;
            }
            this.IConexion.SaveChanges();
            favorito._Cliente = null;
            favorito._Producto = ObtenerProductoPublico(producto.IdProducto);
            return favorito;
        }

        public Favoritos Borrar(int idCliente, Favoritos entidad)
        {
            var producto = ObtenerProducto(entidad.Producto);
            var idsDelModelo = this.IConexion.Productos!
                .Where(x => x.CodigoModelo == producto.CodigoModelo)
                .Select(x => x.IdProducto);
            var favorito = this.IConexion.Favoritos!
                .FirstOrDefault(x => x.Cliente == idCliente && x.Estado && idsDelModelo.Contains(x.Producto));
            if (favorito == null)
                throw new Exception("El producto no está en tus favoritos");

            favorito.Estado = false;
            this.IConexion.Entry<Favoritos>(favorito).State = EntityState.Modified;
            this.IConexion.SaveChanges();
            favorito._Cliente = null;
            favorito._Producto = ObtenerProductoPublico(producto.IdProducto);
            return favorito;
        }

        private Productos ObtenerProducto(int idProducto)
        {
            if (idProducto == 0)
                throw new Exception("Selecciona un producto");
            var producto = this.IConexion.Productos!
                .FirstOrDefault(x => x.IdProducto == idProducto);
            if (producto == null)
                throw new Exception("No se encontró el producto");
            return producto;
        }

        private void ValidarDisponible(Productos producto)
        {
            if (!this.IConexion.Categorias!.Any(x => x.IdCategoria == producto.Categoria && x.Estado) ||
                !this.IConexion.Marcas!.Any(x => x.IdMarca == producto.Marca && x.Estado))
                throw new Exception("Este producto no está disponible para guardar en favoritos");
        }

        private Productos ObtenerProductoPublico(int idProducto)
        {
            var producto = this.IConexion.Productos!
                .First(x => x.IdProducto == idProducto);
            return new Productos
            {
                IdProducto = producto.IdProducto,
                CodigoModelo = producto.CodigoModelo,
                Nombre = producto.Nombre,
                Precio = producto.Precio,
                Descripcion = producto.Descripcion,
                Imagen = producto.Imagen,
                _Categoria = this.IConexion.Categorias!
                    .Where(x => x.IdCategoria == producto.Categoria)
                    .Select(x => new Categorias { Nombre = x.Nombre })
                    .FirstOrDefault(),
                _Marca = this.IConexion.Marcas!
                    .Where(x => x.IdMarca == producto.Marca)
                    .Select(x => new Marcas { Nombre = x.Nombre })
                    .FirstOrDefault(),
                _Color = this.IConexion.Colores!
                    .Where(x => x.IdColor == producto.Color)
                    .Select(x => new Colores { Nombre = x.Nombre, CodigoHexadecimal = x.CodigoHexadecimal })
                    .FirstOrDefault()
            };
        }
    }
}
