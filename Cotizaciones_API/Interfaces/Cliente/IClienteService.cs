using Cotizaciones_API.Models;

namespace Cotizaciones_API.Interfaces.Cliente
{
    public interface IClienteService
    {
        Task<int> InsertAsync(Cliente cliente);
        Task<Cliente?> GetByIdAsync(int id);
        Task<IEnumerable<Cliente>> GetAllAsync();
    }
}
