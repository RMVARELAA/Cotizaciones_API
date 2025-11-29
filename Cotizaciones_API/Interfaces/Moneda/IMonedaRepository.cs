namespace Cotizaciones_API.Interfaces.Moneda
{
    public interface IMonedaRepository
    {
        Task<int> CreateAsync(Models.Moneda item);
        Task<Models.Moneda> GetByIdAsync(int id);
        Task<IEnumerable<Models.Moneda>> GetAllAsync();
        Task UpdateAsync(Models.Moneda item);
        Task DeleteAsync(int id, string usuarioModificacion);
    }
}
