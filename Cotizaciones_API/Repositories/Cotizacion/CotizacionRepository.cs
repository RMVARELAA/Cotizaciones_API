using Cotizaciones_API.Data;
using Cotizaciones_API.DTOs.Cotizacion;
using Cotizaciones_API.Interfaces.Cotizacion;
using Cotizaciones_API.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Cotizaciones_API.Repositories.Cotizacion
{
    public class CotizacionRepository : ICotizacionRepository
    {
        private readonly DapperContext _context;
        private readonly ILogger<CotizacionRepository> _logger;

        public CotizacionRepository(DapperContext context, ILogger<CotizacionRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<long> InsertAsync(Models.Cotizacion cotizacion)
        {
            const string sp = "dbo.sp_Cotizacion_Insert"; // se espera SP que inserte y devuelva Id (SCOPE_IDENTITY)

            try
            {
                using var conn = _context.CreateConnection();
                var p = new DynamicParameters();

                p.Add("@NumeroCotizacion", cotizacion.NumeroCotizacion, DbType.String, size: 50);
                p.Add("@FechaCotizacion", cotizacion.FechaCotizacion, DbType.DateTime2);
                p.Add("@IdTipoSeguro", cotizacion.IdTipoSeguro, DbType.Int32);
                p.Add("@IdMoneda", cotizacion.IdMoneda, DbType.Int32);
                p.Add("@IdCliente", cotizacion.IdCliente, DbType.Int32);
                p.Add("@DescripcionBien", cotizacion.DescripcionBien, DbType.String);
                p.Add("@SumaAsegurada", cotizacion.SumaAsegurada, DbType.Decimal);
                p.Add("@Tasa", cotizacion.Tasa, DbType.Decimal);
                p.Add("@PrimaNeta", cotizacion.PrimaNeta, DbType.Decimal);
                p.Add("@Observaciones", cotizacion.Observaciones, DbType.String);
                p.Add("@UsuarioCreacion", cotizacion.UsuarioCreacion, DbType.String);

                // SP must return the new Id as single row (SELECT CAST(SCOPE_IDENTITY() AS BIGINT))
                var id = await conn.QuerySingleAsync<long>(sp, p, commandType: CommandType.StoredProcedure);
                return id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error InsertAsync Cotizacion {@Cotizacion}", cotizacion);
                throw;
            }
        }

        public async Task<IEnumerable<CotizacionReadDto>> GetAllAsync()
        {
            const string sp = "dbo.sp_Cotizacion_GetAll";
            try
            {
                using var conn = _context.CreateConnection();
                var cotizaciones = await conn.QueryAsync<CotizacionReadDto>(sp, commandType: CommandType.StoredProcedure);
                return cotizaciones;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetAllAsync Cotizaciones");
                throw;
            }
        }
        public async Task<CotizacionReadDto?> GetByIdAsync(long id)
        {
            const string sp = "dbo.sp_Cotizacion_GetById";

            try
            {
                using var conn = _context.CreateConnection();
                var p = new DynamicParameters();
                p.Add("@IdCotizacion", id, DbType.Int64);

                var cot = await conn.QueryFirstOrDefaultAsync<CotizacionReadDto>(sp, p, commandType: CommandType.StoredProcedure);
                return cot;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetByIdAsync CotizacionId={Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<dynamic>> GetReportAsync(DateTime? desde, DateTime? hasta, int? idTipoSeguro)
        {
            const string sp = "dbo.sp_Cotizacion_GetReport"; // SP que acepta @desde, @hasta, @idTipoSeguro y devuelve filas con joins

            try
            {
                using var conn = _context.CreateConnection();
                var p = new DynamicParameters();
                p.Add("@Desde", desde, DbType.DateTime2);
                p.Add("@Hasta", hasta, DbType.DateTime2);
                p.Add("@IdTipoSeguro", idTipoSeguro, DbType.Int32);

                var rows = await conn.QueryAsync(sp, p, commandType: CommandType.StoredProcedure);
                return rows;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetReportAsync desde={desde} hasta={hasta} idTipoSeguro={idTipoSeguro}", desde, hasta, idTipoSeguro);
                throw;
            }
        }

        public async Task UpdateAsync(Models.Cotizacion cotizacion)
        {
            const string sp = "dbo.sp_Cotizacion_Update"; // Ajusta nombre/parametros si tu SP difiere

            try
            {
                using var conn = _context.CreateConnection();
                var p = new DynamicParameters();

                p.Add("@IdCotizacion", cotizacion.IdCotizacion, DbType.Int64);
                p.Add("@IdTipoSeguro", cotizacion.IdTipoSeguro, DbType.Int32);
                p.Add("@IdMoneda", cotizacion.IdMoneda, DbType.Int32);
                p.Add("@IdCliente", cotizacion.IdCliente, DbType.Int32);
                p.Add("@DescripcionBien", cotizacion.DescripcionBien, DbType.String);
                p.Add("@SumaAsegurada", cotizacion.SumaAsegurada, DbType.Decimal);
                p.Add("@Tasa", cotizacion.Tasa, DbType.Decimal);
                p.Add("@PrimaNeta", cotizacion.PrimaNeta, DbType.Decimal);
                p.Add("@Observaciones", cotizacion.Observaciones, DbType.String);
                p.Add("@UsuarioModificacion", cotizacion.UsuarioModificacion, DbType.String);

                var rows = await conn.ExecuteAsync(sp, p, commandType: CommandType.StoredProcedure);

                if (rows == 0)
                {
                    throw new KeyNotFoundException($"Cotización con Id {cotizacion.IdCotizacion} no encontrada.");
                }
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error UpdateAsync Cotizacion {@Cotizacion}", cotizacion);
                throw;
            }
        }

        public async Task DeleteAsync(long id, string usuarioModificacion)
        {
            const string sp = "dbo.sp_Cotizacion_Delete"; // Ajusta nombre/parametros si tu SP difiere

            try
            {
                using var conn = _context.CreateConnection();
                var p = new DynamicParameters();
                p.Add("@IdCotizacion", id, DbType.Int64);
                p.Add("@UsuarioModificacion", usuarioModificacion, DbType.String);

                var rows = await conn.ExecuteAsync(sp, p, commandType: CommandType.StoredProcedure);

                if (rows == 0)
                {
                    throw new KeyNotFoundException($"Cotización con Id {id} no encontrada.");
                }
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error DeleteAsync CotizacionId={Id}", id);
                throw;
            }
        }

        public async Task<PagedResult<CotizacionReadDto>> GetReportePaginadoAsync(
            DateTime? desde,
            DateTime? hasta,
            int? idTipoSeguro,
            string? filtro,
            int pageNumber,
            int pageSize)
        {
            const string sp = "dbo.sp_Cotizacion_GetReportPaged";

            try
            {
                using var conn = _context.CreateConnection();

                var p = new DynamicParameters();
                p.Add("@Desde", desde, DbType.Date);
                p.Add("@Hasta", hasta, DbType.Date);
                p.Add("@IdTipoSeguro", idTipoSeguro, DbType.Int32);
                p.Add("@Filtro", filtro, DbType.String);
                p.Add("@PageNumber", pageNumber, DbType.Int32);
                p.Add("@PageSize", pageSize, DbType.Int32);

                // CotizacionReportRowDto debe tener TODAS las columnas que devuelve el SP
                var rows = (await conn.QueryAsync<CotizacionReportRowDto>(
                    sp, p, commandType: CommandType.StoredProcedure)).ToList();

                var total = rows.FirstOrDefault()?.TotalRows ?? 0;

                var items = rows.Select(r => new CotizacionReadDto
                {
                    IdCotizacion = r.IdCotizacion,
                    NumeroCotizacion = r.NumeroCotizacion,
                    FechaCotizacion = r.FechaCotizacion,

                    IdTipoSeguro = r.IdTipoSeguro,
                    NombreTipoSeguro = r.NombreTipoSeguro,

                    IdMoneda = r.IdMoneda,
                    MonedaCodigoISO = r.MonedaCodigoISO,
                    MonedaNombre = r.MonedaNombre,

                    IdCliente = r.IdCliente,
                    ClienteNombre = r.ClienteNombre,

                    DescripcionBien = r.DescripcionBien,
                    SumaAsegurada = r.SumaAsegurada,
                    Tasa = r.Tasa,
                    PrimaNeta = r.PrimaNeta,
                    Observaciones = r.Observaciones
                }).ToList();

                return new PagedResult<CotizacionReadDto>
                {
                    Items = items,
                    TotalCount = total,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en GetReportePaginadoAsync");
                throw;
            }
        }



    }
}
