using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using presentaciones_tienda.interfaces;

namespace presentaciones_tienda.implementaciones
{
    public class Comunicaciones : IComunicaciones
    {
        private static readonly HttpClient ClienteCompartido = new() { Timeout = TimeSpan.FromMinutes(4) };
        private readonly IHttpContextAccessor IHttpContextAccessor;
        private readonly HttpClient ClienteHttp;
        private readonly string UrlBase;

        public Comunicaciones(IHttpContextAccessor IHttpContextAccessor)
            : this(IHttpContextAccessor, ClienteCompartido, "https://localhost:7135/")
        {
        }

        public Comunicaciones(IHttpContextAccessor IHttpContextAccessor, HttpClient clienteHttp, string urlBase)
        {
            this.IHttpContextAccessor = IHttpContextAccessor;
            this.ClienteHttp = clienteHttp ?? throw new ArgumentNullException(nameof(clienteHttp));
            this.UrlBase = string.IsNullOrWhiteSpace(urlBase)
                ? throw new ArgumentException("La URL base es obligatoria.", nameof(urlBase))
                : urlBase.EndsWith('/') ? urlBase : urlBase + "/";
        }

        public async Task<T> Ejecutar<T>(string url, object? entidad)
        {
            var token = this.IHttpContextAccessor.HttpContext?.Session.GetString("Token");
            using var request = new HttpRequestMessage(HttpMethod.Post, new Uri(new Uri(this.UrlBase), url.TrimStart('/')))
            {
                Content = JsonContent.Create(entidad)
            };
            if (!string.IsNullOrEmpty(token))
                request.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            using var message = await this.ClienteHttp.SendAsync(request);

            if (!message.IsSuccessStatusCode)
            {
                var detalle = await message.Content.ReadAsStringAsync();
                throw new HttpRequestException(
                    $"Error de comunicación HTTP {(int)message.StatusCode}: {detalle}", null, message.StatusCode);
            }

            var respuesta = await message.Content.ReadFromJsonAsync<T>();
            return respuesta!;
        }
    }
}
