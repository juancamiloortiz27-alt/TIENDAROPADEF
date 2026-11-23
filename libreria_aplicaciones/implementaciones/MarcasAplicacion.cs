using libreria_aplicaciones.entidades;
using libreria_aplicaciones.interfaces;
using Microsoft.EntityFrameworkCore;

namespace libreria_aplicaciones.implementaciones
{
    public class MarcasAplicacion : IMarcasAplicacion
    {
        private IConexion IConexion;

        public MarcasAplicacion(IConexion IConexion)
        {
            this.IConexion = IConexion;
        }

        public Marcas Actualizar(Marcas entidad)
        {
            if (entidad.IdMarca == 0)
                throw new Exception("No se ha guardado");
            if (Validar(entidad))
                throw new Exception("Falta información");

            var entry = this.IConexion.Entry<Marcas>(entidad);
            entry.State = EntityState.Modified;
            this.IConexion.SaveChanges();
            return entidad;
        }

        public Marcas Borrar(Marcas entidad)
        {
            if (entidad.IdMarca == 0)
                throw new Exception("No se ha guardado");

            this.IConexion.Marcas!.Remove(entidad);
            this.IConexion.SaveChanges();
            return entidad;
        }

        public List<Marcas> Buscar(Marcas entidad)
        {
            return this.IConexion.Marcas!
                .Where(x => (!string.IsNullOrEmpty(entidad.Nombre) && x.Nombre!.Contains(entidad.Nombre)) ||
                            (!string.IsNullOrEmpty(entidad.PaisOrigen) && x.PaisOrigen!.Contains(entidad.PaisOrigen)))
                .Take(5)
                .ToList();
        }

        public List<Marcas> BuscarActivas(Marcas entidad)
        {
            return this.IConexion.Marcas!
                .Where(x => x.Estado &&
                    ((!string.IsNullOrEmpty(entidad.Nombre) && x.Nombre!.Contains(entidad.Nombre)) ||
                     (!string.IsNullOrEmpty(entidad.PaisOrigen) && x.PaisOrigen!.Contains(entidad.PaisOrigen))))
                .Take(5)
                .ToList();
        }

        public List<Marcas> Consultar()
        {
            return this.IConexion.Marcas!.ToList();
        }

        public List<Marcas> ConsultarActivas()
        {
            return this.IConexion.Marcas!.Where(x => x.Estado).ToList();
        }

        public Marcas Insertar(Marcas entidad)
        {
            if (entidad.IdMarca != 0)
                throw new Exception("Ya se guardó");
            if (Validar(entidad))
                throw new Exception("Falta información");

            this.IConexion.Marcas!.Add(entidad);
            this.IConexion.SaveChanges();
            return entidad;
        }

        private bool Validar(Marcas entidad)
        {
            return string.IsNullOrEmpty(entidad.Nombre) ||
                string.IsNullOrEmpty(entidad.PaisOrigen) ||
                string.IsNullOrEmpty(entidad.SitioWeb);
        }
    }
}
