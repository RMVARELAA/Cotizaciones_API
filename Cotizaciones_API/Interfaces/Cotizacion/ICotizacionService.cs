using Cotizaciones_API.DTOs.Cotizacion;

namespace Cotizaciones_API.Interfaces.Cotizacion
{
    public interface ICotizacionService
    {
        Task<(long Id, string Numero)> CreateAsync(CotizacionCreateDto dto);
        Task<IEnumerable<dynamic>> GetReportAsync(DateTime? desde, DateTime? hasta, int? idTipoSeguro);
        Task<Models.Cotizacion> GetByIdAsync(long id);
        
    }
}
