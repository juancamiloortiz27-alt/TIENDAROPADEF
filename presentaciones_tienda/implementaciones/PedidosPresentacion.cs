using libreria_aplicaciones.entidades;
using presentaciones_tienda.interfaces;

namespace presentaciones_tienda.implementaciones
{
    public class PedidosPresentacion : IPedidosPresentacion
    {
        private IComunicaciones IComunicaciones;

        public PedidosPresentacion(IComunicaciones IComunicaciones)
        {
            this.IComunicaciones = IComunicaciones;
        }

        public List<Pedidos> ConsultarMios()
        {
            var task = this.IComunicaciones.Ejecutar<List<Pedidos>>("Pedidos/ConsultarMios", null);
            task.Wait();
            return task.Result;
        }

        public Pedidos Confirmar(ConfirmarPedido entidad)
        {
            var task = this.IComunicaciones.Ejecutar<Pedidos>("Pedidos/Confirmar", entidad);
            task.Wait();
            return task.Result;
        }

        public CuponValidado ValidarCupon(string codigoCupon)
        {
            var task = this.IComunicaciones.Ejecutar<CuponValidado>("Pedidos/ValidarCupon", new ConfirmarPedido
            {
                CodigoCupon = codigoCupon
            });
            task.Wait();
            return task.Result;
        }
    }
}
