using libreria_aplicaciones.entidades;
using libreria_aplicaciones.interfaces;
using Microsoft.EntityFrameworkCore;

namespace libreria_aplicaciones.implementaciones
{
    public class TallasAplicacion : ITallasAplicacion
    {
        private IConexion IConexion;

        public TallasAplicacion(IConexion IConexion)
        {
            this.IConexion = IConexion;
        }

        public Tallas Actualizar(Tallas entidad)
        {
            if (entidad.IdTalla == 0)
                throw new Exception("No se ha guardado");
            if (Validar(entidad))
                throw new Exception("Falta información");

            var entry = this.IConexion.Entry<Tallas>(entidad);
            entry.State = EntityState.Modified;
            this.IConexion.SaveChanges();
            return entidad;
        }

        public Tallas Borrar(Tallas entidad)
        {
            if (entidad.IdTalla == 0)
                throw new Exception("No se ha guardado");

            this.IConexion.Tallas!.Remove(entidad);
            this.IConexion.SaveChanges();
            return entidad;
        }

        public List<Tallas> Buscar(Tallas entidad)
        {
            return this.IConexion.Tallas!
                .Where(x => (!string.IsNullOrEmpty(entidad.Nombre) && x.Nombre!.Contains(entidad.Nombre)) ||
                            (!string.IsNullOrEmpty(entidad.Tipo) && x.Tipo!.Contains(entidad.Tipo)) ||
                            (!string.IsNullOrEmpty(entidad.SistemaMedida) && x.SistemaMedida!.Contains(entidad.SistemaMedida)))
                .Take(5)
                .ToList();
        }

        public List<Tallas> Consultar()
        {
            return this.IConexion.Tallas!.ToList();
        }

        public Tallas Insertar(Tallas entidad)
        {
            if (entidad.IdTalla != 0)
                throw new Exception("Ya se guardó");
            if (Validar(entidad))
                throw new Exception("Falta información");

            this.IConexion.Tallas!.Add(entidad);
            this.IConexion.SaveChanges();
            return entidad;
        }

        private bool Validar(Tallas entidad)
        {
            return string.IsNullOrEmpty(entidad.Nombre) ||
                string.IsNullOrEmpty(entidad.Tipo) ||
                string.IsNullOrEmpty(entidad.Descripcion) ||
                string.IsNullOrEmpty(entidad.SistemaMedida);
        }
    }
}
