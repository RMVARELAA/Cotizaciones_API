using Cotizaciones_API.Data;
using Cotizaciones_API.Interfaces;
using Cotizaciones_API.Interfaces.TipoCliente;
using Cotizaciones_API.Models;
using Dapper;
using System.Data;

namespace Cotizaciones_API.Repositories.TipoCliente
{
    public class TipoClienteRepository : ITipoClienteRepository
    {
        private readonly DapperContext _context;
        private readonly ILogger<TipoClienteRepository> _logger;

        public TipoClienteRepository(DapperContext context, ILogger<TipoClienteRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> InsertAsync(Models.TipoCliente item)
        {
            const string sp = "dbo.sp_TipoCliente_Insert";
            try
            {
                using var conn = _context.CreateConnection();
                var p = new DynamicParameters();
                p.Add("@NombreTipoCliente", item.NombreTipoCliente, DbType.String);
                p.Add("@Descripcion", item.Descripcion, DbType.String);

                var id = await conn.QuerySingleAsync<int>(sp, p, commandType: CommandType.StoredProcedure);
                return id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error InsertAsync TipoCliente {@TipoCliente}", item);
                throw;
            }
        }

        public async Task<Models.TipoCliente> GetByIdAsync(int id)
        {
            const string sp = "dbo.sp_TipoCliente_GetById";
            try
            {
                using var conn = _context.CreateConnection();
                var p = new DynamicParameters();
                p.Add("@IdTipoCliente", id, DbType.Int32);

                var item = await conn.QueryFirstOrDefaultAsync<Models.TipoCliente>(sp, p, commandType: CommandType.StoredProcedure);
                return item;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetByIdAsync TipoClienteId={Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<Models.TipoCliente>> GetAllAsync()
        {
            const string sp = "dbo.sp_TipoCliente_GetAll";
            try
            {
                using var conn = _context.CreateConnection();
                return await conn.QueryAsync<Models.TipoCliente>(sp, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetAllAsync TipoClientes");
                throw;
            }
        }
    }
}
