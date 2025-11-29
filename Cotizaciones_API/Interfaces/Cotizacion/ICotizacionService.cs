using Cotizaciones_API.DTOs.Cotizacion;
using Cotizaciones_API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cotizaciones_API.Interfaces.Cotizacion
{
    public interface ICotizacionService
    {
        Task<(long Id, string Numero)> CreateAsync(CotizacionCreateDto dto);
        Task<IEnumerable<dynamic>> GetReportAsync(DateTime? desde, DateTime? hasta, int? idTipoSeguro);
        Task<CotizacionReadDto?> GetByIdAsync(long id);
        Task<IEnumerable<CotizacionReadDto>> GetAllAsync();
        Task UpdateAsync(Models.Cotizacion cotizacion);
        Task DeleteAsync(long id, string usuarioModificacion);
    }
}
