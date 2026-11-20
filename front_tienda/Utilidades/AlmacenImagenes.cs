using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace front_tienda.Utilidades
{
    public static class AlmacenImagenes
    {
        private const long TamañoMaximo = 5 * 1024 * 1024;

        public static string Guardar(IFormFile imagen, IWebHostEnvironment entorno, string carpeta)
        {
            if (carpeta != "productos" && carpeta != "categorias" && carpeta != "marcas")
                throw new Exception("La carpeta de imágenes no es válida");

            if (imagen.Length == 0 || imagen.Length > TamañoMaximo)
                throw new Exception("La imagen debe tener contenido y pesar máximo 5 MB");

            var firma = new byte[12];
            var leidos = 0;
            using (var lectura = imagen.OpenReadStream())
            {
                while (leidos < firma.Length)
                {
                    var cantidad = lectura.Read(firma, leidos, firma.Length - leidos);
                    if (cantidad == 0) break;
                    leidos += cantidad;
                }
            }

            var extension = ObtenerExtension(firma, leidos);
            if (extension == null)
                throw new Exception("El archivo no parece una imagen compatible. Usa JPG, PNG, WEBP, GIF, BMP, TIFF o AVIF.");

            var carpetaWeb = entorno.WebRootPath;
            if (string.IsNullOrWhiteSpace(carpetaWeb))
                carpetaWeb = Path.Combine(entorno.ContentRootPath, "wwwroot");

            var carpetaDestino = Path.Combine(carpetaWeb, "images", carpeta);
            Directory.CreateDirectory(carpetaDestino);

            var nombreArchivo = Guid.NewGuid().ToString("N") + extension;
            var rutaArchivo = Path.Combine(carpetaDestino, nombreArchivo);
            using (var archivo = File.Create(rutaArchivo))
                imagen.CopyTo(archivo);

            return "/images/" + carpeta + "/" + nombreArchivo;
        }

        public static void Eliminar(string? rutaImagen, IWebHostEnvironment entorno, string carpeta)
        {
            var prefijo = "/images/" + carpeta + "/";
            if (string.IsNullOrWhiteSpace(rutaImagen) ||
                !rutaImagen.StartsWith(prefijo, StringComparison.OrdinalIgnoreCase))
                return;

            var carpetaWeb = entorno.WebRootPath;
            if (string.IsNullOrWhiteSpace(carpetaWeb))
                carpetaWeb = Path.Combine(entorno.ContentRootPath, "wwwroot");

            var nombreArchivo = Path.GetFileName(rutaImagen);
            var rutaArchivo = Path.Combine(carpetaWeb, "images", carpeta, nombreArchivo);
            if (File.Exists(rutaArchivo)) File.Delete(rutaArchivo);
        }

        private static string? ObtenerExtension(byte[] firma, int leidos)
        {
            if (leidos >= 3 && firma[0] == 0xFF && firma[1] == 0xD8 && firma[2] == 0xFF)
                return ".jpg";

            if (leidos >= 8 && firma[0] == 0x89 && firma[1] == 0x50 && firma[2] == 0x4E && firma[3] == 0x47 &&
                firma[4] == 0x0D && firma[5] == 0x0A && firma[6] == 0x1A && firma[7] == 0x0A)
                return ".png";

            if (leidos >= 12 && firma[0] == 0x52 && firma[1] == 0x49 && firma[2] == 0x46 && firma[3] == 0x46 &&
                firma[8] == 0x57 && firma[9] == 0x45 && firma[10] == 0x42 && firma[11] == 0x50)
                return ".webp";

            if (leidos >= 6 && firma[0] == 0x47 && firma[1] == 0x49 && firma[2] == 0x46 && firma[3] == 0x38 &&
                (firma[4] == 0x37 || firma[4] == 0x39) && firma[5] == 0x61)
                return ".gif";

            if (leidos >= 2 && firma[0] == 0x42 && firma[1] == 0x4D)
                return ".bmp";

            if (leidos >= 4 && ((firma[0] == 0x49 && firma[1] == 0x49 && firma[2] == 0x2A && firma[3] == 0x00) ||
                (firma[0] == 0x4D && firma[1] == 0x4D && firma[2] == 0x00 && firma[3] == 0x2A)))
                return ".tif";

            if (leidos >= 12 && firma[4] == 0x66 && firma[5] == 0x74 && firma[6] == 0x79 && firma[7] == 0x70 &&
                ((firma[8] == 0x61 && firma[9] == 0x76 && firma[10] == 0x69 && (firma[11] == 0x66 || firma[11] == 0x73))))
                return ".avif";

            return null;
        }
    }
}
