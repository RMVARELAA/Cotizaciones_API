namespace Cotizaciones_API.Interfaces.Moneda
{
    public interface IMonedaRepository
    {
        Task<int> InsertAsync(Models.Moneda item);
        Task<Models.Moneda> GetByIdAsync(int id);
        Task<IEnumerable<Models.Moneda>> GetAllAsync();
    }
}
