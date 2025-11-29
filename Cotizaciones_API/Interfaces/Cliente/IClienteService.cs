using Cotizaciones_API.Models;

namespace Cotizaciones_API.Interfaces.Cliente
{
    public interface IClienteService
    {
        Task<int> CreateAsync(Models.Cliente cliente);
        Task<Models.Cliente?> GetByIdAsync(int id);
        Task<IEnumerable<Models.Cliente>> GetAllAsync();

        Task UpdateAsync(Models.Cliente cliente);
        Task DeleteAsync(int id, string usuarioModificacion);
    }
}
