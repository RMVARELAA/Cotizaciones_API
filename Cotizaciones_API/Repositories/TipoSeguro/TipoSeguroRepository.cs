using Cotizaciones_API.Data;
using Cotizaciones_API.Interfaces;
using Cotizaciones_API.Interfaces.TipoSeguro;
using Cotizaciones_API.Models;
using Dapper;
using System.Data;

namespace Cotizaciones_API.Repositories.TipoSeguro
{
    public class TipoSeguroRepository : ITipoSeguroRepository
    {
        private readonly DapperContext _context;
        private readonly ILogger<TipoSeguroRepository> _logger;

        public TipoSeguroRepository(DapperContext context, ILogger<TipoSeguroRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> InsertAsync(Models.TipoSeguro item)
        {
            const string sp = "dbo.sp_TipoSeguro_Insert";
            try
            {
                using var conn = _context.CreateConnection();
                var p = new DynamicParameters();
                p.Add("@NombreSeguro", item.NombreSeguro, DbType.String);
                p.Add("@Codigo", item.Codigo, DbType.String);
                p.Add("@Descripcion", item.Descripcion, DbType.String);

                var id = await conn.QuerySingleAsync<int>(sp, p, commandType: CommandType.StoredProcedure);
                return id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error InsertAsync TipoSeguro {@TipoSeguro}", item);
                throw;
            }
        }

        public async Task<Models.TipoSeguro> GetByIdAsync(int id)
        {
            const string sp = "dbo.sp_TipoSeguro_GetById";
            try
            {
                using var conn = _context.CreateConnection();
                var p = new DynamicParameters();
                p.Add("@IdTipoSeguro", id, DbType.Int32);

                var item = await conn.QueryFirstOrDefaultAsync<Models.TipoSeguro>(sp, p, commandType: CommandType.StoredProcedure);
                return item;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetByIdAsync TipoSeguroId={Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<Models.TipoSeguro>> GetAllAsync()
        {
            const string sp = "dbo.sp_TipoSeguro_GetAll";
            try
            {
                using var conn = _context.CreateConnection();
                return await conn.QueryAsync<Models.TipoSeguro>(sp, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetAllAsync TipoSeguros");
                throw;
            }
        }
    }
}
