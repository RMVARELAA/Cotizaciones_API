namespace Cotizaciones_API.Interfaces.Cliente
{
    public interface IClienteRepository
    {
        Task<int> InsertAsync(Models.Cliente cliente);
        Task<Models.Cliente?> GetByIdAsync(int id);
        Task<IEnumerable<Models.Cliente>> GetAllAsync();

    }
}
