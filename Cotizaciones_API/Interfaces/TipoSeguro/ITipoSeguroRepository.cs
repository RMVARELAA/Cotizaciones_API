using Cotizaciones_API.Models;

namespace Cotizaciones_API.Interfaces.TipoSeguro

{
    public interface ITipoSeguroRepository
    {
        Task<int> InsertAsync(Models.TipoSeguro item);
        Task<Models.TipoSeguro> GetByIdAsync(int id);
        Task<IEnumerable<Models.TipoSeguro>> GetAllAsync();
    }
}
