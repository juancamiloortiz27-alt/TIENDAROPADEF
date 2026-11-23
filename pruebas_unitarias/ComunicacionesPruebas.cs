using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using presentaciones_tienda.implementaciones;

namespace pruebas_unitarias
{
    [TestClass]
    public class ComunicacionesPruebas
    {
        [TestMethod]
        public async Task Ejecutar_EnviaPostYDeserializaRespuesta()
        {
            var handler = new RespuestaHttpHandler((request, _) =>
            {
                Assert.AreEqual(HttpMethod.Post, request.Method);
                Assert.AreEqual("https://api.pruebas.local/Productos/Consultar", request.RequestUri!.ToString());
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(new RespuestaProducto { Nombre = "Chaqueta" })
                });
            });
            using var cliente = new HttpClient(handler);
            var comunicaciones = new Comunicaciones(new HttpContextAccessor(), cliente, "https://api.pruebas.local");

            var respuesta = await comunicaciones.Ejecutar<RespuestaProducto>("/Productos/Consultar", new { Id = 7 });

            Assert.AreEqual("Chaqueta", respuesta.Nombre);
            Assert.IsNotNull(handler.UltimoJson);
            Assert.AreEqual(7, handler.UltimoJson.RootElement.GetProperty("id").GetInt32());
        }

        [TestMethod]
        public async Task Ejecutar_AgregaTokenDeSesionComoBearer()
        {
            var handler = new RespuestaHttpHandler((request, _) =>
            {
                Assert.AreEqual("Bearer token-de-prueba", request.Headers.Authorization?.ToString());
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(true)
                });
            });
            using var cliente = new HttpClient(handler);
            var contexto = new DefaultHttpContext { Session = new SesionPrueba() };
            contexto.Session.SetString("Token", "token-de-prueba");
            var comunicaciones = new Comunicaciones(new HttpContextAccessor { HttpContext = contexto }, cliente, "https://api.pruebas.local/");

            var respuesta = await comunicaciones.Ejecutar<bool>("Auditorias/Consultar", null);

            Assert.IsTrue(respuesta);
        }

        [TestMethod]
        public async Task Ejecutar_RespuestaHttpFallida_LanzaConEstadoYDetalle()
        {
            var handler = new RespuestaHttpHandler((_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.Forbidden)
            {
                Content = new StringContent("Acceso denegado")
            }));
            using var cliente = new HttpClient(handler);
            var comunicaciones = new Comunicaciones(new HttpContextAccessor(), cliente, "https://api.pruebas.local/");

            var error = await Assert.ThrowsExceptionAsync<HttpRequestException>(
                () => comunicaciones.Ejecutar<bool>("Usuarios/Consultar", null));

            Assert.AreEqual(HttpStatusCode.Forbidden, error.StatusCode);
            StringAssert.Contains(error.Message, "Acceso denegado");
        }

        private sealed class RespuestaProducto
        {
            public string? Nombre { get; set; }
        }

        private sealed class RespuestaHttpHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> responder;
            public JsonDocument? UltimoJson { get; private set; }

            public RespuestaHttpHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> responder)
            {
                this.responder = responder;
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (request.Content != null)
                    this.UltimoJson = await JsonDocument.ParseAsync(await request.Content.ReadAsStreamAsync(cancellationToken), cancellationToken: cancellationToken);
                return await this.responder(request, cancellationToken);
            }
        }

        private sealed class SesionPrueba : ISession
        {
            private readonly Dictionary<string, byte[]> valores = new(StringComparer.Ordinal);
            public bool IsAvailable => true;
            public string Id => "sesion-prueba";
            public IEnumerable<string> Keys => valores.Keys;
            public void Clear() => valores.Clear();
            public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
            public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
            public void Remove(string key) => valores.Remove(key);
            public void Set(string key, byte[] value) => valores[key] = value;
            public bool TryGetValue(string key, out byte[] value) => valores.TryGetValue(key, out value!);
        }
    }
}
