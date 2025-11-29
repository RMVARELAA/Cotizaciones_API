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
        Task<Models.Cotizacion> GetByIdAsync(long id);

        // CRUD para cotizaciones
        Task UpdateAsync(Models.Cotizacion cotizacion);
        Task DeleteAsync(long id, string usuarioModificacion);
    }
}
