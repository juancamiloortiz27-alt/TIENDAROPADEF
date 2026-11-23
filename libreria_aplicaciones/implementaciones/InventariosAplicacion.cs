

using libreria_aplicaciones.entidades;
using libreria_aplicaciones.interfaces;
using Microsoft.EntityFrameworkCore;

namespace libreria_aplicaciones.implementaciones
{
    public class InventariosAplicacion : IInventariosAplicacion
    {
        private const int StockMinimo = 5;
        private IConexion IConexion;

        public InventariosAplicacion(IConexion IConexion)
        {
            this.IConexion = IConexion;
        }

        public Inventarios Actualizar(Inventarios entidad)
        {
            if (entidad.IdInventario == 0)
                throw new Exception("No se ha guardado");
            if (Validar(entidad))
                throw new Exception("Falta información");

            entidad.StockMinimo = StockMinimo;
            entidad._Producto = null;
            var entry = this.IConexion.Entry<Inventarios>(entidad);
            entry.State = EntityState.Modified;
            this.IConexion.SaveChanges();
            entidad._Producto = null;
            return entidad;
        }

        public Inventarios Borrar(Inventarios entidad)
        {
            if (entidad.IdInventario == 0)
                throw new Exception("No se ha guardado");

            entidad._Producto = null;
            this.IConexion.Inventarios!.Remove(entidad);
            this.IConexion.SaveChanges();
            return entidad;
        }

        public List<Inventarios> Buscar(Inventarios entidad)
        {
            if (entidad.Producto == 0 && string.IsNullOrEmpty(entidad.Ubicacion))
                return Consultar();

            return ConsultarInventarios()
                .Where(x => (entidad.Producto != 0 && x.Producto == entidad.Producto) ||
                            (!string.IsNullOrEmpty(entidad.Ubicacion) && x.Ubicacion!.Contains(entidad.Ubicacion)))
                .Take(5)
                .ToList();
        }

        public List<Inventarios> Consultar()
        {
            return ConsultarInventarios().ToList();
        }

        public Inventarios Insertar(Inventarios entidad)
        {
            if (entidad.IdInventario != 0)
                throw new Exception("Ya se guardó");
            if (Validar(entidad))
                throw new Exception("Falta información");

            entidad.StockMinimo = StockMinimo;
            entidad._Producto = null;
            this.IConexion.Inventarios!.Add(entidad);
            this.IConexion.SaveChanges();
            entidad._Producto = null;
            return entidad;
        }

        private IQueryable<Inventarios> ConsultarInventarios()
        {
            return this.IConexion.Inventarios!
                .Select(x => new Inventarios
                {
                    IdInventario = x.IdInventario,
                    Cantidad = x.Cantidad,
                    StockMinimo = StockMinimo,
                    Ubicacion = x.Ubicacion,
                    Producto = x.Producto,
                    _Producto = new Productos
                    {
                        IdProducto = x._Producto!.IdProducto,
                        Nombre = x._Producto.Nombre
                    }
                });
        }

        private bool Validar(Inventarios entidad)
        {
            return entidad.Cantidad < 0 ||
                entidad.Producto == 0 ||
                string.IsNullOrEmpty(entidad.Ubicacion);
        }
    }
}
