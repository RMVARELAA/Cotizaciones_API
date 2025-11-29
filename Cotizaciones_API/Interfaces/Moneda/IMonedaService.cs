using Cotizaciones_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cotizaciones_API.Interfaces.Moneda
{
    public interface IMonedaService
    {
        Task<int> CreateAsync(Models.Moneda model);
        Task<IEnumerable<Models.Moneda>> GetAllAsync();
        Task<Models.Moneda?> GetByIdAsync(int id);
        Task UpdateAsync(Models.Moneda model);
        Task DeleteAsync(int id, string usuarioModificacion);
    }
}
