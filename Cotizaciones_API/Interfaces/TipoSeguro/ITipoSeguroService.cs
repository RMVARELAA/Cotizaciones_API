using Cotizaciones_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cotizaciones_API.Interfaces.TipoSeguro
{
    public interface ITipoSeguroService
    {
        Task<int> CreateAsync(Models.TipoSeguro model);
        Task<IEnumerable<Models.TipoSeguro>> GetAllAsync();
        Task<Models.TipoSeguro?> GetByIdAsync(int id);
        Task UpdateAsync(Models.TipoSeguro model);
        Task DeleteAsync(int id, string usuarioModificacion);
    }
}
