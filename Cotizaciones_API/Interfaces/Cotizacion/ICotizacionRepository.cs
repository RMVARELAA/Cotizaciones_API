using Cotizaciones_API.Models;

namespace Cotizaciones_API.Interfaces.Cotizacion
{
    public interface ICotizacionRepository
    {
        Task<long> InsertAsync(Models.Cotizacion cotizacion);
        Task<Models.Cotizacion> GetByIdAsync(long id);
        Task<IEnumerable<dynamic>> GetReportAsync(DateTime? desde, DateTime? hasta, int? idTipoSeguro);
    }
}
