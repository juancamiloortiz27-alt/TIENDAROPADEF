using libreria_aplicaciones.entidades;
using libreria_aplicaciones.interfaces;
using Microsoft.EntityFrameworkCore;

namespace libreria_aplicaciones.implementaciones
{
    public class ProductosAplicacion : IProductosAplicacion
    {
        private IConexion IConexion;

        public ProductosAplicacion(IConexion IConexion)
        {
            this.IConexion = IConexion;
        }

        public Productos Actualizar(Productos entidad)
        {
            if (entidad.IdProducto == 0)
                throw new Exception("No se ha guardado");
            NormalizarCodigoModelo(entidad);
            if (Validar(entidad))
                throw new Exception("Falta información");

            var codigoModeloAnterior = this.IConexion.Productos!
                .Where(x => x.IdProducto == entidad.IdProducto)
                .Select(x => x.CodigoModelo)
                .SingleOrDefault();
            var mismoModelo = string.Equals(codigoModeloAnterior, entidad.CodigoModelo, StringComparison.Ordinal);
            ValidarGrupoModelo(entidad, permitirCambioMarca: mismoModelo, permitirCambioCategoria: mismoModelo);

            var variantes = this.IConexion.Productos!
                .Where(x => x.CodigoModelo == entidad.CodigoModelo && x.IdProducto != entidad.IdProducto)
                .ToList();
            if (string.IsNullOrWhiteSpace(entidad.Imagen))
                entidad.Imagen = variantes.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.Imagen))?.Imagen;

            QuitarNavegaciones(entidad);
            var entry = this.IConexion.Entry<Productos>(entidad);
            entry.State = EntityState.Modified;
            foreach (var variante in variantes)
            {
                variante.Imagen = entidad.Imagen;
                if (mismoModelo)
                {
                    variante.Marca = entidad.Marca;
                    variante.Categoria = entidad.Categoria;
                }
            }
            this.IConexion.SaveChanges();
            QuitarNavegaciones(entidad);
            return entidad;
        }

        public Productos Borrar(Productos entidad)
        {
            if (entidad.IdProducto == 0)
                throw new Exception("No se ha guardado");

            QuitarNavegaciones(entidad);
            this.IConexion.Productos!.Remove(entidad);
            this.IConexion.SaveChanges();
            QuitarNavegaciones(entidad);
            return entidad;
        }

        public List<Productos> Buscar(Productos entidad)
        {
            if (string.IsNullOrWhiteSpace(entidad.Nombre) && string.IsNullOrWhiteSpace(entidad.CodigoModelo))
                return Consultar();

            return ConsultarProductos()
                .Where(x => (!string.IsNullOrEmpty(entidad.Nombre) && x.Nombre!.Contains(entidad.Nombre)) ||
                            (!string.IsNullOrEmpty(entidad.CodigoModelo) && x.CodigoModelo!.Contains(entidad.CodigoModelo)))
                .ToList();
        }

        public List<Productos> Consultar()
        {
            return ConsultarProductos().ToList();
        }

        public List<ProductoPublico> ConsultarPublicos()
        {
            return ConsultarProductosPublicos().ToList();
        }

        public List<ProductoPublico> BuscarPublicos(Productos entidad)
        {
            return ConsultarProductosPublicos()
                .Where(x => !string.IsNullOrEmpty(entidad.Nombre) && x.Nombre!.Contains(entidad.Nombre))
                .ToList();
        }

        public List<ProductoPublico> ConsultarVariantesPublicas(string codigoModelo)
        {
            codigoModelo = codigoModelo.Trim().ToUpperInvariant();
            return ConsultarProductosPublicos()
                .Where(x => x.CodigoModelo == codigoModelo)
                .ToList();
        }

        public Productos Insertar(Productos entidad)
        {
            if (entidad.IdProducto != 0)
                throw new Exception("Ya se guardó");
            NormalizarCodigoModelo(entidad);
            if (Validar(entidad))
                throw new Exception("Falta información");
            ValidarGrupoModelo(entidad);

            var variantes = this.IConexion.Productos!
                .Where(x => x.CodigoModelo == entidad.CodigoModelo)
                .ToList();
            if (string.IsNullOrWhiteSpace(entidad.Imagen))
                entidad.Imagen = variantes.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.Imagen))?.Imagen;
            else
                foreach (var variante in variantes)
                    variante.Imagen = entidad.Imagen;

            QuitarNavegaciones(entidad);
            this.IConexion.Productos!.Add(entidad);
            this.IConexion.SaveChanges();
            QuitarNavegaciones(entidad);
            return entidad;
        }

        private IQueryable<Productos> ConsultarProductos()
        {
            return this.IConexion.Productos!
                .Select(x => new Productos
                {
                    IdProducto = x.IdProducto,
                    CodigoModelo = x.CodigoModelo,
                    Nombre = x.Nombre,
                    Precio = x.Precio,
                    Descripcion = x.Descripcion,
                    Imagen = x.Imagen,
                    Categoria = x.Categoria,
                    Marca = x.Marca,
                    Color = x.Color,
                    Proveedor = x.Proveedor,
                    Talla = x.Talla,
                    _Categoria = new Categorias
                    {
                        IdCategoria = x._Categoria!.IdCategoria,
                        Nombre = x._Categoria.Nombre
                    },
                    _Marca = new Marcas
                    {
                        IdMarca = x._Marca!.IdMarca,
                        Nombre = x._Marca.Nombre
                    },
                    _Color = new Colores
                    {
                        IdColor = x._Color!.IdColor,
                        Nombre = x._Color.Nombre,
                        CodigoHexadecimal = x._Color.CodigoHexadecimal
                    },
                    _Talla = new Tallas
                    {
                        IdTalla = x._Talla!.IdTalla,
                        Nombre = x._Talla.Nombre,
                        SistemaMedida = x._Talla.SistemaMedida
                    },
                    _Proveedor = new Proveedores
                    {
                        IdProveedor = x._Proveedor!.IdProveedor,
                        Nombre = x._Proveedor.Nombre
                    }
                });
        }

        private IQueryable<ProductoPublico> ConsultarProductosPublicos()
        {
            return this.IConexion.Productos!
                .Where(x => x._Categoria!.Estado && x._Marca!.Estado)
                .Select(x => new ProductoPublico
            {
                IdProducto = x.IdProducto,
                CodigoModelo = x.CodigoModelo,
                Nombre = x.Nombre,
                Precio = x.Precio,
                Descripcion = x.Descripcion,
                Imagen = x.Imagen,
                NombreCategoria = x._Categoria!.Nombre,
                NombreMarca = x._Marca!.Nombre,
                NombreColor = x._Color!.Nombre,
                CodigoHexadecimal = x._Color!.CodigoHexadecimal,
                NombreTalla = x._Talla!.Nombre,
                SistemaMedida = x._Talla!.SistemaMedida,
                StockDisponible = this.IConexion.Inventarios!
                    .Where(inventario => inventario.Producto == x.IdProducto)
                    .Sum(inventario => (int?)inventario.Cantidad) ?? 0,
                Disponible = (this.IConexion.Inventarios!
                    .Where(inventario => inventario.Producto == x.IdProducto)
                    .Sum(inventario => (int?)inventario.Cantidad) ?? 0) > 0
            });
        }

        private bool Validar(Productos entidad)
        {
            return string.IsNullOrWhiteSpace(entidad.CodigoModelo) ||
                entidad.CodigoModelo!.Length > 40 ||
                string.IsNullOrEmpty(entidad.Nombre) ||
                string.IsNullOrEmpty(entidad.Descripcion) ||
                entidad.Precio <= 0 ||
                entidad.Categoria == 0 ||
                entidad.Marca == 0 ||
                entidad.Color == 0 ||
                entidad.Proveedor == 0 ||
                entidad.Talla == 0;
        }

        private void NormalizarCodigoModelo(Productos entidad)
        {
            if (!string.IsNullOrWhiteSpace(entidad.CodigoModelo))
                entidad.CodigoModelo = entidad.CodigoModelo.Trim().ToUpperInvariant();
        }

        private void ValidarGrupoModelo(Productos entidad, bool permitirCambioMarca = false, bool permitirCambioCategoria = false)
        {
            var variantesExistentes = this.IConexion.Productos!
                .Where(x => x.CodigoModelo == entidad.CodigoModelo && x.IdProducto != entidad.IdProducto)
                .ToList();

            if (variantesExistentes.Any(x => x.Talla == entidad.Talla))
                throw new Exception("Ya existe una variante de este modelo con esa talla");

            if (variantesExistentes.Any(x => x.Nombre != entidad.Nombre ||
                                             x.Precio != entidad.Precio ||
                                             x.Descripcion != entidad.Descripcion ||
                                             (!permitirCambioCategoria && x.Categoria != entidad.Categoria) ||
                                             (!permitirCambioMarca && x.Marca != entidad.Marca) ||
                                             x.Color != entidad.Color ||
                                             x.Proveedor != entidad.Proveedor))
                throw new Exception("El código de modelo ya está asociado a otra prenda o color");
        }

        private void QuitarNavegaciones(Productos entidad)
        {
            entidad._Categoria = null;
            entidad._Marca = null;
            entidad._Color = null;
            entidad._Proveedor = null;
            entidad._Talla = null;
            entidad.Inventario = null;
            entidad.DetalleCarrito = null;
            entidad.DetallePedido = null;
            entidad.Reseña = null;
            entidad.Favorito = null;
        }
    }
}
