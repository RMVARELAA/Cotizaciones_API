using Cotizaciones_API.Data;
using Cotizaciones_API.Interfaces.Cliente;
using Cotizaciones_API.Models;
using Dapper;
using System.Data;
using Microsoft.Extensions.Logging;

namespace Cotizaciones_API.Repositories.Cliente
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly DapperContext _context;
        private readonly ILogger<ClienteRepository> _logger;

        public ClienteRepository(DapperContext context, ILogger<ClienteRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Inserta un cliente llamando al SP sp_Cliente_Insert
        /// Devuelve el Id generado (INT).
        /// </summary>
        public async Task<int> InsertAsync(Models.Cliente cliente)
        {
            const string sp = "dbo.sp_Cliente_Insert";

            try
            {
                using var conn = _context.CreateConnection();

                var parameters = new DynamicParameters();
                parameters.Add("@Nombre", cliente.Nombre, DbType.String);
                parameters.Add("@Identidad", cliente.Identidad, DbType.String);
                parameters.Add("@FechaNacimiento", cliente.FechaNacimiento, DbType.Date);
                parameters.Add("@IdTipoCliente", cliente.IdTipoCliente, DbType.Int32);
                parameters.Add("@Telefono", cliente.Telefono, DbType.String);
                parameters.Add("@Email", cliente.Email, DbType.String);
                parameters.Add("@Direccion", cliente.Direccion, DbType.String);
                parameters.Add("@UsuarioCreacion", cliente.UsuarioCreacion, DbType.String);

                // El SP retorna el Id con SELECT SCOPE_IDENTITY()
                var id = await conn.QuerySingleAsync<int>(
                    sp,
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error InsertAsync Cliente {@Cliente}", cliente);
                throw;
            }
        }

        /// <summary>
        /// Obtiene un cliente por Id llamando al SP sp_Cliente_GetById
        /// </summary>
        public async Task<Models.Cliente?> GetByIdAsync(int id)
        {
            const string sp = "dbo.sp_Cliente_GetById";

            try
            {
                using var conn = _context.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@IdCliente", id, DbType.Int32);

                var cliente = await conn.QueryFirstOrDefaultAsync<Models.Cliente>(
                    sp,
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return cliente;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetByIdAsync ClienteId={Id}", id);
                throw;
            }
        }

        /// <summary>
        /// Obtiene todos los clientes activos llamando al SP sp_Cliente_GetAll
        /// </summary>
        public async Task<IEnumerable<Models.Cliente>> GetAllAsync()
        {
            const string sp = "dbo.sp_Cliente_GetAll";

            try
            {
                using var conn = _context.CreateConnection();
                return await conn.QueryAsync<Models.Cliente>(
                    sp,
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetAllAsync Clientes");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un cliente llamando al SP sp_Cliente_Update
        /// Lanza KeyNotFoundException si no se afectó ninguna fila.
        /// </summary>
        public async Task UpdateAsync(Models.Cliente cliente)
        {
            const string sp = "dbo.sp_Cliente_Update";

            try
            {
                using var conn = _context.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@IdCliente", cliente.IdCliente, DbType.Int32);
                parameters.Add("@Nombre", cliente.Nombre, DbType.String);
                parameters.Add("@Identidad", cliente.Identidad, DbType.String);
                parameters.Add("@FechaNacimiento", cliente.FechaNacimiento, DbType.Date);
                parameters.Add("@IdTipoCliente", cliente.IdTipoCliente, DbType.Int32);
                parameters.Add("@Telefono", cliente.Telefono, DbType.String);
                parameters.Add("@Email", cliente.Email, DbType.String);
                parameters.Add("@Direccion", cliente.Direccion, DbType.String);
                parameters.Add("@UsuarioModificacion", cliente.UsuarioModificacion, DbType.String);
                parameters.Add("@Estado", cliente.Estado, DbType.Boolean);

                var rows = await conn.ExecuteAsync(
                    sp,
                    parameters,
                    commandType: CommandType.StoredProcedure);

                if (rows == 0)
                {
                    throw new KeyNotFoundException($"Cliente con Id {cliente.IdCliente} no encontrado.");
                }
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error UpdateAsync Cliente {@Cliente}", cliente);
                throw;
            }
        }

        /// <summary>
        /// Elimina (lógicamente) un cliente llamando al SP sp_Cliente_Delete
        /// Lanza KeyNotFoundException si no se afectó ninguna fila.
        /// </summary>
        public async Task DeleteAsync(int id, string usuarioModificacion)
        {
            const string sp = "dbo.sp_Cliente_Delete";

            try
            {
                using var conn = _context.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@IdCliente", id, DbType.Int32);
                parameters.Add("@UsuarioModificacion", usuarioModificacion, DbType.String);

                var rows = await conn.ExecuteAsync(
                    sp,
                    parameters,
                    commandType: CommandType.StoredProcedure);

                if (rows == 0)
                {
                    throw new KeyNotFoundException($"Cliente con Id {id} no encontrado.");
                }
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error DeleteAsync ClienteId={Id}", id);
                throw;
            }
        }
    }
}
