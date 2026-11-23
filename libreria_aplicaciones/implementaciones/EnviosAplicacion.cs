using libreria_aplicaciones.entidades;
using libreria_aplicaciones.interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace libreria_aplicaciones.implementaciones
{
    public class EnviosAplicacion : IEnviosAplicacion
    {
        private static readonly string[] EstadosPermitidos =
        {
            "Preparando", "Enviado", "En tránsito", "Entregado"
        };

        private IConexion IConexion;

        public EnviosAplicacion(IConexion IConexion)
        {
            this.IConexion = IConexion;
        }

        public List<Envios> Consultar()
        {
            var envios = this.IConexion.Envios!
                .OrderByDescending(x => x.IdEnvio)
                .ToList();
            foreach (var envio in envios)
            {
                envio._Transportadora = this.IConexion.Transportadoras!
                    .Where(x => x.IdTransportadoras == envio.Transportadora)
                    .Select(x => new Transportadoras
                    {
                        IdTransportadoras = x.IdTransportadoras,
                        Nombre = x.Nombre,
                        Telefono = x.Telefono,
                        Correo = x.Correo,
                        Ciudad = x.Ciudad
                    })
                    .FirstOrDefault();
                envio._Pedido = this.IConexion.Pedidos!
                    .Where(x => x.IdPedido == envio.Pedido)
                    .Select(x => new Pedidos
                    {
                        IdPedido = x.IdPedido,
                        Fecha = x.Fecha,
                        Estado = x.Estado,
                        Total = x.Total,
                        DireccionEnvio = x.DireccionEnvio,
                        CiudadEnvio = x.CiudadEnvio,
                        CodigoPostalEnvio = x.CodigoPostalEnvio
                    })
                    .FirstOrDefault();
            }
            return envios;
        }

        public List<Pedidos> ConsultarPedidosSinEnvio()
        {
            return this.IConexion.Pedidos!
                .Where(pedido => pedido.Estado == "Pagado" &&
                    !this.IConexion.Envios!.Any(envio => envio.Pedido == pedido.IdPedido))
                .OrderBy(pedido => pedido.Fecha)
                .Select(pedido => new Pedidos
                {
                    IdPedido = pedido.IdPedido,
                    Fecha = pedido.Fecha,
                    Estado = pedido.Estado,
                    Total = pedido.Total,
                    DireccionEnvio = pedido.DireccionEnvio,
                    CiudadEnvio = pedido.CiudadEnvio,
                    CodigoPostalEnvio = pedido.CodigoPostalEnvio,
                    _Cliente = new Clientes
                    {
                        Nombre = pedido._Cliente!.Nombre,
                        Apellido = pedido._Cliente.Apellido
                    }
                })
                .ToList();
        }

        public Envios Insertar(Envios entidad)
        {
            if (entidad.IdEnvio != 0)
                throw new Exception("El envío ya fue guardado");
            if (entidad.Pedido == 0 || entidad.Transportadora == 0)
                throw new Exception("Selecciona un pedido y una transportadora");
            var pedido = this.IConexion.Pedidos!
                .FirstOrDefault(x => x.IdPedido == entidad.Pedido);
            if (pedido == null || pedido.Estado != "Pagado")
                throw new Exception("Solo se pueden preparar envíos para pedidos pagados");
            if (this.IConexion.Envios!.Any(x => x.Pedido == entidad.Pedido))
                throw new Exception("Este pedido ya tiene un envío asignado");
            if (!this.IConexion.Transportadoras!.Any(x => x.IdTransportadoras == entidad.Transportadora))
                throw new Exception("No se encontró la transportadora seleccionada");

            entidad.NumeroGuia = GenerarNumeroGuia();
            entidad.Estado = "Preparando";
            entidad._Pedido = null;
            entidad._Transportadora = null;
            this.IConexion.Envios!.Add(entidad);
            this.IConexion.SaveChanges();
            return entidad;
        }

        public Envios ActualizarEstado(Envios entidad)
        {
            if (entidad.IdEnvio == 0 || !EstadosPermitidos.Contains(entidad.Estado ?? ""))
                throw new Exception("Selecciona un envío y un estado válido");
            var envio = this.IConexion.Envios!
                .FirstOrDefault(x => x.IdEnvio == entidad.IdEnvio);
            if (envio == null)
                throw new Exception("No se encontró el envío");
            envio.Estado = entidad.Estado;
            this.IConexion.Entry<Envios>(envio).State = EntityState.Modified;
            this.IConexion.SaveChanges();
            return envio;
        }

        private string GenerarNumeroGuia()
        {
            for (var intento = 0; intento < 50; intento++)
            {
                var numero = RandomNumberGenerator.GetInt32(1_000_000_000, 2_000_000_000)
                    .ToString(System.Globalization.CultureInfo.InvariantCulture);
                if (!this.IConexion.Envios!.Any(x => x.NumeroGuia == numero))
                    return numero;
            }
            throw new Exception("No se pudo generar un número de guía único. Intenta de nuevo");
        }
    }
}
