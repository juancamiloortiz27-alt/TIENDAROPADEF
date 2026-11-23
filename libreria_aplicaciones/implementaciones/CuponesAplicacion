using libreria_aplicaciones.entidades;
using libreria_aplicaciones.interfaces;
using Microsoft.EntityFrameworkCore;

namespace libreria_aplicaciones.implementaciones
{
    public class CuponesAplicacion : ICuponesAplicacion
    {
        private IConexion IConexion;

        public CuponesAplicacion(IConexion IConexion)
        {
            this.IConexion = IConexion;
        }

        public List<Cupones> Consultar()
        {
            return this.IConexion.Cupones!.OrderBy(x => x.Codigo).ToList();
        }

        public List<Cupones> Buscar(Cupones entidad)
        {
            if (string.IsNullOrWhiteSpace(entidad.Codigo))
                return Consultar();
            var codigo = entidad.Codigo.Trim();
            return this.IConexion.Cupones!
                .Where(x => x.Codigo!.Contains(codigo))
                .Take(20)
                .ToList();
        }

        public Cupones Insertar(Cupones entidad)
        {
            if (entidad.IdCupon != 0)
                throw new Exception("El cupón ya fue guardado");
            Validar(entidad);
            entidad.Codigo = entidad.Codigo!.Trim().ToUpperInvariant();
            entidad.Pedido = null;
            if (ExisteCodigo(entidad.Codigo, 0))
                throw new Exception("Ya existe un cupón con ese código");
            this.IConexion.Cupones!.Add(entidad);
            this.IConexion.SaveChanges();
            return entidad;
        }

        public Cupones Actualizar(Cupones entidad)
        {
            if (entidad.IdCupon == 0)
                throw new Exception("Selecciona un cupón para actualizar");
            Validar(entidad);
            entidad.Codigo = entidad.Codigo!.Trim().ToUpperInvariant();
            entidad.Pedido = null;
            if (!this.IConexion.Cupones!.Any(x => x.IdCupon == entidad.IdCupon))
                throw new Exception("No se encontró el cupón");
            if (ExisteCodigo(entidad.Codigo, entidad.IdCupon))
                throw new Exception("Ya existe otro cupón con ese código");
            this.IConexion.Entry<Cupones>(entidad).State = EntityState.Modified;
            this.IConexion.SaveChanges();
            return entidad;
        }

        public Cupones Borrar(Cupones entidad)
        {
            if (entidad.IdCupon == 0)
                throw new Exception("Selecciona un cupón para borrar");
            var cupon = this.IConexion.Cupones!
                .FirstOrDefault(x => x.IdCupon == entidad.IdCupon);
            if (cupon == null)
                throw new Exception("No se encontró el cupón");
            if (this.IConexion.Pedidos!.Any(x => x.Cupon == entidad.IdCupon))
                throw new Exception("Este cupón ya fue usado en un pedido. Desactívalo para conservar el historial");
            this.IConexion.Cupones!.Remove(cupon);
            this.IConexion.SaveChanges();
            return cupon;
        }

        private bool ExisteCodigo(string codigo, int idExcluir)
        {
            return this.IConexion.Cupones!
                .Any(x => x.IdCupon != idExcluir && x.Codigo == codigo);
        }

        private void Validar(Cupones entidad)
        {
            if (string.IsNullOrWhiteSpace(entidad.Codigo) || entidad.Codigo.Trim().Length > 15)
                throw new Exception("El código es obligatorio y puede tener hasta 15 caracteres");
            if (entidad.PorcentajeDescuento <= 0 || entidad.PorcentajeDescuento > 100)
                throw new Exception("El descuento debe ser mayor que 0 y no puede superar el 100 %");
            if (entidad.FechaVence == default)
                throw new Exception("Indica la fecha de vencimiento del cupón");
        }
    }
}
