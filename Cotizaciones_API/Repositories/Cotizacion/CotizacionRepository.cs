using Dapper;
using System.Data;
using Cotizaciones_API.Data;
using Cotizaciones_API.Interfaces.Cotizacion;
using Cotizaciones_API.Models;

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

        public async Task<Models.Cotizacion> GetByIdAsync(long id)
        {
            const string sp = "dbo.sp_Cotizacion_GetById";

            try
            {
                using var conn = _context.CreateConnection();
                var p = new DynamicParameters();
                p.Add("@IdCotizacion", id, DbType.Int64);

                var cot = await conn.QueryFirstOrDefaultAsync<Models.Cotizacion>(sp, p, commandType: CommandType.StoredProcedure);
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
    }
}
