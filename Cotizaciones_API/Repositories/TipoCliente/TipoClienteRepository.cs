using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Cotizaciones_API.Data;
using Cotizaciones_API.Interfaces;
using Cotizaciones_API.Interfaces.TipoCliente;
using Cotizaciones_API.Models;
using Dapper;
using Microsoft.Extensions.Logging;

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

        public async Task<int> CreateAsync(Models.TipoCliente item)
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
                _logger.LogError(ex, "Error CreateAsync TipoCliente {@TipoCliente}", item);
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

        public async Task UpdateAsync(Models.TipoCliente item)
        {
            const string sp = "dbo.sp_TipoCliente_Update";
            try
            {
                using var conn = _context.CreateConnection();
                var p = new DynamicParameters();
                p.Add("@IdTipoCliente", item.IdTipoCliente, DbType.Int32);
                p.Add("@NombreTipoCliente", item.NombreTipoCliente, DbType.String);
                p.Add("@Descripcion", item.Descripcion, DbType.String);
                p.Add("@UsuarioModificacion", item.UsuarioModificacion, DbType.String);

                var rows = await conn.ExecuteAsync(sp, p, commandType: CommandType.StoredProcedure);
                if (rows == 0)
                {
                    throw new KeyNotFoundException($"TipoCliente con Id {item.IdTipoCliente} no encontrado.");
                }
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error UpdateAsync TipoCliente {@TipoCliente}", item);
                throw;
            }
        }

        public async Task DeleteAsync(int id, string usuarioModificacion)
        {
            const string sp = "dbo.sp_TipoCliente_Delete";
            try
            {
                using var conn = _context.CreateConnection();
                var p = new DynamicParameters();
                p.Add("@IdTipoCliente", id, DbType.Int32);
                p.Add("@UsuarioModificacion", usuarioModificacion, DbType.String);

                var rows = await conn.ExecuteAsync(sp, p, commandType: CommandType.StoredProcedure);
                if (rows == 0)
                {
                    throw new KeyNotFoundException($"TipoCliente con Id {id} no encontrado.");
                }
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error DeleteAsync TipoClienteId={Id}", id);
                throw;
            }
        }
    }
}
