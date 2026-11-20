using System.Globalization;

namespace front_tienda.Utilidades;

public static class FormatoMoneda
{
    private static readonly CultureInfo CulturaColombiana = CultureInfo.GetCultureInfo("es-CO");

    public static string Cop(decimal valor) => $"COP {valor.ToString("#,##0.##", CulturaColombiana)}";
}