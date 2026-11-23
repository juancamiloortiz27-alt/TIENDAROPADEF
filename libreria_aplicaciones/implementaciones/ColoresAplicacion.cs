using libreria_aplicaciones.entidades;
using libreria_aplicaciones.interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace libreria_aplicaciones.implementaciones
{
    public class ColoresAplicacion : IColoresAplicacion
    {
        private IConexion IConexion;

        public ColoresAplicacion(IConexion IConexion)
        {
            this.IConexion = IConexion;
        }

        public Colores Actualizar(Colores entidad)
        {
            if (entidad.IdColor == 0)
                throw new Exception("No se ha guardado");
            if (Validar(entidad))
                throw new Exception("Falta información");

            var entry = this.IConexion.Entry<Colores>(entidad);
            entry.State = EntityState.Modified;
            this.IConexion.SaveChanges();
            return entidad;
        }

        public Colores Borrar(Colores entidad)
        {
            if (entidad.IdColor == 0)
                throw new Exception("No se ha guardado");

            this.IConexion.Colores!.Remove(entidad);
            this.IConexion.SaveChanges();
            return entidad;
        }

        public List<Colores> Buscar(Colores entidad)
        {
            return this.IConexion.Colores!
                .Where(x => (!string.IsNullOrEmpty(entidad.Nombre) && x.Nombre!.Contains(entidad.Nombre)) ||
                            (!string.IsNullOrEmpty(entidad.Tipo) && x.Tipo!.Contains(entidad.Tipo)))
                .Take(5)
                .ToList();
        }

        public List<Colores> Consultar()
        {
            return this.IConexion.Colores!.ToList();
        }

        public Colores Insertar(Colores entidad)
        {
            if (entidad.IdColor != 0)
                throw new Exception("Ya se guardó");
            if (Validar(entidad))
                throw new Exception("Falta información");

            this.IConexion.Colores!.Add(entidad);
            this.IConexion.SaveChanges();
            return entidad;
        }

        private bool Validar(Colores entidad)
        {
            return string.IsNullOrEmpty(entidad.Nombre) ||
                string.IsNullOrEmpty(entidad.CodigoHexadecimal) ||
                !Regex.IsMatch(entidad.CodigoHexadecimal, "^#[0-9A-Fa-f]{6}$") ||
                string.IsNullOrEmpty(entidad.Tipo) ||
                string.IsNullOrEmpty(entidad.Descripcion);
        }
    }
}
