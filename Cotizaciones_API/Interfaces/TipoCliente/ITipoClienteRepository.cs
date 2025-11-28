namespace Cotizaciones_API.Interfaces.TipoCliente
{
    public interface ITipoClienteRepository
    {
        Task<int> InsertAsync(Models.TipoCliente item);
        Task<Models.TipoCliente> GetByIdAsync(int id);
        Task<IEnumerable<Models.TipoCliente>> GetAllAsync();
    }
}
