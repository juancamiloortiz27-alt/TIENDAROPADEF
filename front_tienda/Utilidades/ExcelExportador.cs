using System.Globalization;
using System.IO.Compression;
using System.Text;
using System.Xml;

namespace front_tienda.Utilidades
{
    /// <summary>Genera una hoja .xlsx sencilla sin dependencias externas.</summary>
    public static class ExcelExportador
    {
        public const string TipoContenido = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        public static byte[] Crear(string nombreHoja, IReadOnlyList<string> encabezados, IEnumerable<IReadOnlyList<object?>> filas)
        {
            using var salida = new MemoryStream();
            using (var archivo = new ZipArchive(salida, ZipArchiveMode.Create, leaveOpen: true))
            {
                EscribirTexto(archivo, "[Content_Types].xml", writer =>
                {
                    writer.WriteStartElement("Types", "http://schemas.openxmlformats.org/package/2006/content-types");
                    writer.WriteStartElement("Default"); writer.WriteAttributeString("Extension", "rels"); writer.WriteAttributeString("ContentType", "application/vnd.openxmlformats-package.relationships+xml"); writer.WriteEndElement();
                    writer.WriteStartElement("Default"); writer.WriteAttributeString("Extension", "xml"); writer.WriteAttributeString("ContentType", "application/xml"); writer.WriteEndElement();
                    writer.WriteStartElement("Override"); writer.WriteAttributeString("PartName", "/xl/workbook.xml"); writer.WriteAttributeString("ContentType", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml"); writer.WriteEndElement();
                    writer.WriteStartElement("Override"); writer.WriteAttributeString("PartName", "/xl/worksheets/sheet1.xml"); writer.WriteAttributeString("ContentType", "application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml"); writer.WriteEndElement();
                    writer.WriteEndElement();
                });
                EscribirTexto(archivo, "_rels/.rels", writer =>
                {
                    writer.WriteStartElement("Relationships", "http://schemas.openxmlformats.org/package/2006/relationships");
                    writer.WriteStartElement("Relationship"); writer.WriteAttributeString("Id", "rId1"); writer.WriteAttributeString("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument"); writer.WriteAttributeString("Target", "xl/workbook.xml"); writer.WriteEndElement();
                    writer.WriteEndElement();
                });
                EscribirTexto(archivo, "xl/workbook.xml", writer =>
                {
                    writer.WriteStartElement("workbook", "http://schemas.openxmlformats.org/spreadsheetml/2006/main");
                    writer.WriteAttributeString("xmlns", "r", null, "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
                    writer.WriteStartElement("sheets"); writer.WriteStartElement("sheet");
                    writer.WriteAttributeString("name", nombreHoja.Length > 31 ? nombreHoja[..31] : nombreHoja);
                    writer.WriteAttributeString("sheetId", "1"); writer.WriteAttributeString("r", "id", "http://schemas.openxmlformats.org/officeDocument/2006/relationships", "rId1");
                    writer.WriteEndElement(); writer.WriteEndElement(); writer.WriteEndElement();
                });
                EscribirTexto(archivo, "xl/_rels/workbook.xml.rels", writer =>
                {
                    writer.WriteStartElement("Relationships", "http://schemas.openxmlformats.org/package/2006/relationships");
                    writer.WriteStartElement("Relationship"); writer.WriteAttributeString("Id", "rId1"); writer.WriteAttributeString("Type", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet"); writer.WriteAttributeString("Target", "worksheets/sheet1.xml"); writer.WriteEndElement();
                    writer.WriteEndElement();
                });
                EscribirTexto(archivo, "xl/worksheets/sheet1.xml", writer =>
                {
                    writer.WriteStartElement("worksheet", "http://schemas.openxmlformats.org/spreadsheetml/2006/main");
                    writer.WriteStartElement("sheetViews"); writer.WriteStartElement("sheetView"); writer.WriteAttributeString("workbookViewId", "0");
                    writer.WriteStartElement("pane"); writer.WriteAttributeString("ySplit", "1"); writer.WriteAttributeString("topLeftCell", "A2"); writer.WriteAttributeString("activePane", "bottomLeft"); writer.WriteAttributeString("state", "frozen"); writer.WriteEndElement();
                    writer.WriteEndElement(); writer.WriteEndElement();
                    writer.WriteStartElement("sheetData");
                    EscribirFila(writer, 1, encabezados.Cast<object?>().ToArray());
                    uint numeroFila = 2;
                    foreach (var fila in filas)
                        EscribirFila(writer, numeroFila++, fila);
                    writer.WriteEndElement();
                    if (encabezados.Count > 0)
                    {
                        writer.WriteStartElement("autoFilter");
                        writer.WriteAttributeString("ref", $"A1:{Columna(encabezados.Count)}{Math.Max(1, numeroFila - 1)}");
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                });
            }
            return salida.ToArray();
        }

        private static void EscribirFila(XmlWriter writer, uint numeroFila, IReadOnlyList<object?> valores)
        {
            writer.WriteStartElement("row"); writer.WriteAttributeString("r", numeroFila.ToString(CultureInfo.InvariantCulture));
            for (var i = 0; i < valores.Count; i++)
            {
                var valor = valores[i];
                if (valor is null) continue;
                var referencia = Columna(i + 1) + numeroFila.ToString(CultureInfo.InvariantCulture);
                if (EsNumero(valor))
                {
                    writer.WriteStartElement("c"); writer.WriteAttributeString("r", referencia);
                    writer.WriteElementString("v", Convert.ToString(valor, CultureInfo.InvariantCulture));
                    writer.WriteEndElement();
                }
                else
                {
                    writer.WriteStartElement("c"); writer.WriteAttributeString("r", referencia); writer.WriteAttributeString("t", "inlineStr");
                    writer.WriteStartElement("is"); writer.WriteElementString("t", Convert.ToString(valor, CultureInfo.InvariantCulture) ?? string.Empty); writer.WriteEndElement();
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();
        }

        private static bool EsNumero(object valor) => valor is byte or sbyte or short or ushort or int or uint or long or ulong or float or double or decimal;

        private static string Columna(int numero)
        {
            var nombre = new StringBuilder();
            while (numero > 0)
            {
                numero--;
                nombre.Insert(0, (char)('A' + numero % 26));
                numero /= 26;
            }
            return nombre.ToString();
        }

        private static void EscribirTexto(ZipArchive archivo, string ruta, Action<XmlWriter> escribir)
        {
            var entrada = archivo.CreateEntry(ruta, CompressionLevel.Optimal);
            using var stream = entrada.Open();
            using var writer = XmlWriter.Create(stream, new XmlWriterSettings { Encoding = new UTF8Encoding(false), Indent = false, CloseOutput = false });
            escribir(writer);
        }
    }
}
