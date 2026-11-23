using libreria_aplicaciones.entidades;
using libreria_aplicaciones.interfaces;
using Microsoft.EntityFrameworkCore;

namespace libreria_aplicaciones.implementaciones
{
    public class CategoriasAplicacion : ICategoriasAplicacion
    {
        private IConexion IConexion;

        public CategoriasAplicacion(IConexion IConexion)
        {
            this.IConexion = IConexion;
        }

        public Categorias Actualizar(Categorias entidad)
        {
            if (entidad.IdCategoria == 0)
                throw new Exception("No se ha guardo");

            if (Validar(entidad))
                throw new Exception("Falta informacion");

            var entry = this.IConexion!.Entry<Categorias>(entidad);
            entry.State = EntityState.Modified;
            this.IConexion!.SaveChanges();
            return entidad;
        }

        public Categorias Borrar(Categorias entidad)
        {
            if (entidad.IdCategoria == 0)
                throw new Exception("No se ha guardo");

            this.IConexion.Categorias!.Remove(entidad!);
            this.IConexion.SaveChanges();
            return entidad;
        }

        public List<Categorias> Buscar(Categorias entidad)
        {
            return this.IConexion.Categorias!
                .Where(x => (!string.IsNullOrEmpty(entidad.Nombre) && x.Nombre!.Contains(entidad.Nombre)) ||
                            (!string.IsNullOrEmpty(entidad.Descripcion) && x.Descripcion!.Contains(entidad.Descripcion)) ||
                            (!string.IsNullOrEmpty(entidad.TipoRopa) && x.TipoRopa!.Contains(entidad.TipoRopa)))
                .Take(5)
                .ToList();
        }

        public List<Categorias> Consultar()
        {
            return this.IConexion.Categorias!.ToList();
        }

        public Categorias Insertar(Categorias entidad)
        {
            if (entidad.IdCategoria != 0)
                throw new Exception("Ya se guardo");

            if (Validar(entidad))
                throw new Exception("Falta informacion");

            this.IConexion.Categorias!.Add(entidad!);
            this.IConexion.SaveChanges();
            return entidad;
        }

        private bool Validar(Categorias entidad)
        {
            return string.IsNullOrEmpty(entidad.Nombre) ||
                string.IsNullOrEmpty(entidad.Descripcion) ||
                string.IsNullOrEmpty(entidad.TipoRopa);
        }
    }
}
