using Cotizaciones_API.DTOs.Cotizacion;
using Cotizaciones_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Cotizaciones_API.Interfaces.Cotizacion
{
    public interface ICotizacionRepository
    {
        Task<long> InsertAsync(Models.Cotizacion cotizacion);
        Task<CotizacionReadDto?> GetByIdAsync(long id);
        Task<IEnumerable<dynamic>> GetReportAsync(DateTime? desde, DateTime? hasta, int? idTipoSeguro);
        Task<IEnumerable<CotizacionReadDto>> GetAllAsync();
        Task UpdateAsync(Models.Cotizacion cotizacion);
        Task DeleteAsync(long id, string usuarioModificacion);
        Task<PagedResult<CotizacionReadDto>> GetReportePaginadoAsync(
        DateTime? desde,
        DateTime? hasta,
        int? idTipoSeguro,
        string? filtro,
        int pageNumber,
        int pageSize);
    }
}
