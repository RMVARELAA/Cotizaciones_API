using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Cotizaciones_API.Data;
using Cotizaciones_API.Interfaces.Moneda;
using Cotizaciones_API.Models;
using Dapper;
using Microsoft.Extensions.Logging;

namespace Cotizaciones_API.Repositories.Moneda
{
    public class MonedaRepository : IMonedaRepository
    {
        private readonly DapperContext _context;
        private readonly ILogger<MonedaRepository> _logger;

        public MonedaRepository(DapperContext context, ILogger<MonedaRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> CreateAsync(Models.Moneda item)
        {
            const string sp = "dbo.sp_Moneda_Insert";
            try
            {
                using var conn = _context.CreateConnection();
                var p = new DynamicParameters();

                p.Add("@CodigoISO", item.CodigoISO, DbType.String);
                p.Add("@Nombre", item.Nombre, DbType.String);
                p.Add("@Simbolo", item.Simbolo, DbType.String);

                // Si tu SP retorna el Id con SELECT SCOPE_IDENTITY()
                var id = await conn.QuerySingleAsync<int>(sp, p, commandType: CommandType.StoredProcedure);
                return id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error CreateAsync Moneda {@Moneda}", item);
                throw;
            }
        }

        public async Task<Models.Moneda> GetByIdAsync(int id)
        {
            const string sp = "dbo.sp_Moneda_GetById";
            try
            {
                using var conn = _context.CreateConnection();
                var p = new DynamicParameters();
                p.Add("@IdMoneda", id, DbType.Int32);

                var item = await conn.QueryFirstOrDefaultAsync<Models.Moneda>(sp, p, commandType: CommandType.StoredProcedure);
                return item;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetByIdAsync MonedaId={Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<Models.Moneda>> GetAllAsync()
        {
            const string sp = "dbo.sp_Moneda_GetAll";
            try
            {
                using var conn = _context.CreateConnection();
                return await conn.QueryAsync<Models.Moneda>(sp, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetAllAsync Monedas");
                throw;
            }
        }

        public async Task UpdateAsync(Models.Moneda item)
        {
            const string sp = "dbo.sp_Moneda_Update";
            try
            {
                using var conn = _context.CreateConnection();
                var p = new DynamicParameters();

                p.Add("@IdMoneda", item.IdMoneda, DbType.Int32);
                p.Add("@CodigoISO", item.CodigoISO, DbType.String);
                p.Add("@Nombre", item.Nombre, DbType.String);
                p.Add("@Simbolo", item.Simbolo, DbType.String);

                var rows = await conn.ExecuteAsync(sp, p, commandType: CommandType.StoredProcedure);
                if (rows == 0)
                {
                    throw new KeyNotFoundException($"Moneda con Id {item.IdMoneda} no encontrada.");
                }
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error UpdateAsync Moneda {@Moneda}", item);
                throw;
            }
        }

        public async Task DeleteAsync(int id, string usuarioModificacion)
        {
            const string sp = "dbo.sp_Moneda_Delete";
            try
            {
                using var conn = _context.CreateConnection();
                var p = new DynamicParameters();
                p.Add("@IdMoneda", id, DbType.Int32);
                p.Add("@UsuarioModificacion", usuarioModificacion, DbType.String);

                var rows = await conn.ExecuteAsync(sp, p, commandType: CommandType.StoredProcedure);
                if (rows == 0)
                {
                    throw new KeyNotFoundException($"Moneda con Id {id} no encontrada.");
                }
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error DeleteAsync MonedaId={Id}", id);
                throw;
            }
        }
    }
}
