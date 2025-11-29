using Cotizaciones_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cotizaciones_API.Interfaces.TipoCliente
{
    public interface ITipoClienteService
    {
        Task<int> CreateAsync(Models.TipoCliente model);
        Task<IEnumerable<Models.TipoCliente>> GetAllAsync();
        Task<Models.TipoCliente?> GetByIdAsync(int id);
        Task UpdateAsync(Models.TipoCliente model);
        Task DeleteAsync(int id, string usuarioModificacion);
    }
}
