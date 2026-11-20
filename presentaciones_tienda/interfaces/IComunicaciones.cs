namespace presentaciones_tienda.interfaces
{
    public interface IComunicaciones
    {
        Task<T> Ejecutar<T>(string url, object? entidad);
    }
}
