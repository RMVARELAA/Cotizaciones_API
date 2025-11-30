using Cotizaciones_API.Data;
using Cotizaciones_API.DTOs.Cotizacion;
using Cotizaciones_API.Interfaces.Cliente;
using Cotizaciones_API.Interfaces.Cotizacion;
using Cotizaciones_API.Interfaces.Moneda;
using Cotizaciones_API.Interfaces.TipoSeguro;
using Cotizaciones_API.Models;
using Dapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Cotizaciones_API.Services.Cotizacion
{
    public class CotizacionService : ICotizacionService
    {
        private readonly ICotizacionRepository _cotRepo;
        private readonly IClienteRepository _clienteRepo;
        private readonly ITipoSeguroRepository _tipoSeguroRepo;
        private readonly IMonedaRepository _monedaRepo;
        private readonly DapperContext _dapperContext;
        private readonly ILogger<CotizacionService> _logger;

        public CotizacionService(
            ICotizacionRepository cotRepo,
            IClienteRepository clienteRepo,
            ITipoSeguroRepository tipoSeguroRepo,
            IMonedaRepository monedaRepo,
            DapperContext dapperContext,
            ILogger<CotizacionService> logger)
        {
            _cotRepo = cotRepo;
            _clienteRepo = clienteRepo;
            _tipoSeguroRepo = tipoSeguroRepo;
            _monedaRepo = monedaRepo;
            _dapperContext = dapperContext;
            _logger = logger;
        }

        public async Task<(long Id, string Numero)> CreateAsync(CotizacionCreateDto dto)
        {
            try
            {
                // Validaciones de negocios
                if (dto.SumaAsegurada <= 0) throw new ArgumentException("SumaAsegurada debe ser mayor a 0.");
                if (dto.Tasa < 0 || dto.Tasa > 1) throw new ArgumentException("Tasa inválida (0 - 1).");

                // Verificar FK existencia
                var cliente = await _clienteRepo.GetByIdAsync(dto.IdCliente);
                if (cliente == null) throw new KeyNotFoundException("Cliente no encontrado.");

                var tipoSeguro = await _tipoSeguroRepo.GetByIdAsync(dto.IdTipoSeguro);
                if (tipoSeguro == null) throw new KeyNotFoundException("Tipo de seguro no encontrado.");

                var moneda = await _monedaRepo.GetByIdAsync(dto.IdMoneda);
                if (moneda == null) throw new KeyNotFoundException("Moneda no encontrada.");

                decimal tasa;
                using (var conn = _dapperContext.CreateConnection())
                {
                    var p = new DynamicParameters();
                    p.Add("@IdTipoSeguro", dto.IdTipoSeguro, DbType.Int32);
                    p.Add("@SumaAsegurada", dto.SumaAsegurada, DbType.Decimal);
                    // llamamos al SP que devuelve una fila con columna Tasa
                    var tasaDesdeRegla = await conn.QueryFirstOrDefaultAsync<decimal?>(
                        "dbo.sp_Tasa_GetPorTipoYSuma", p, commandType: CommandType.StoredProcedure);
                    if (tasaDesdeRegla.HasValue)
                        tasa = tasaDesdeRegla.Value;
                    else
                        tasa = dto.Tasa; // fallback al valor que envía el cliente
                }
                dto.Tasa = tasa;

                if (tasa < 0 || tasa > 1) throw new ArgumentException("Tasa inválida.");

                // Calcular PrimaNeta (2 decimales)
                var prima = Math.Round(dto.SumaAsegurada * tasa, 2);

                // Generar NumeroCotizacion ejecutando el SP sp_Cotizacion_GenerarNumero
                string numero;
                using (var conn = _dapperContext.CreateConnection())
                {
                    // Dapper mapeará la primera columna devuelta al tipo string
                    numero = await conn.QuerySingleAsync<string>("EXEC dbo.sp_Cotizacion_GenerarNumero");
                }

                // Construir entidad
                var entity = new Models.Cotizacion
                {
                    NumeroCotizacion = numero,
                    FechaCotizacion = DateTime.UtcNow,
                    IdTipoSeguro = dto.IdTipoSeguro,
                    IdMoneda = dto.IdMoneda,
                    IdCliente = dto.IdCliente,
                    DescripcionBien = dto.DescripcionBien,
                    SumaAsegurada = dto.SumaAsegurada,
                    Tasa = dto.Tasa,
                    PrimaNeta = prima,
                    Observaciones = dto.Observaciones,
                    UsuarioCreacion = dto.UsuarioCreacion,
                    Estado = true
                };

                // Persistir
                var id = await _cotRepo.InsertAsync(entity);

                // Opcional: enviar correo, eventos, logging adicional
                _logger.LogInformation("Cotización creada Id={Id} Numero={Numero}", id, numero);

                return (id, numero);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en CotizacionService.CreateAsync {@Dto}", dto);
                throw;
            }
        }

        public async Task<IEnumerable<CotizacionReadDto>> GetAllAsync()
        {
            try
            {
                return await _cotRepo.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetAllAsync Cotizaciones");
                throw;
            }
        }

        public async Task<IEnumerable<dynamic>> GetReportAsync(DateTime? desde, DateTime? hasta, int? idTipoSeguro)
        {
            try
            {
                return await _cotRepo.GetReportAsync(desde, hasta, idTipoSeguro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetReportAsync desde={Desde} hasta={Hasta} idTipoSeguro={Tipo}", desde, hasta, idTipoSeguro);
                throw;
            }
        }

        public async Task<CotizacionReadDto?> GetByIdAsync(long id)
        {
            try
            {
                return await _cotRepo.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetByIdAsync CotizacionId={Id}", id);
                throw;
            }
        }

        // Actualiza una cotización existente. Recalcula PrimaNeta y valida FKs si cambian.
        public async Task UpdateAsync(Models.Cotizacion cotizacion)
        {
            try
            {
                if (cotizacion == null) throw new ArgumentNullException(nameof(cotizacion));
                if (cotizacion.IdCotizacion <= 0) throw new ArgumentException("IdCotizacion inválido.");
                if (cotizacion.SumaAsegurada <= 0) throw new ArgumentException("SumaAsegurada debe ser mayor a 0.");

                // Existe la cotización?
                var existing = await _cotRepo.GetByIdAsync(cotizacion.IdCotizacion);
                if (existing == null) throw new KeyNotFoundException("Cotización no encontrada.");

                // Si se cambió cliente/tipo/moneda, verificar existencia
                if (cotizacion.IdCliente != existing.IdCliente)
                {
                    var cliente = await _clienteRepo.GetByIdAsync(cotizacion.IdCliente);
                    if (cliente == null) throw new KeyNotFoundException("Cliente no encontrado.");
                }

                if (cotizacion.IdTipoSeguro != existing.IdTipoSeguro)
                {
                    var tipoSeguro = await _tipoSeguroRepo.GetByIdAsync(cotizacion.IdTipoSeguro);
                    if (tipoSeguro == null) throw new KeyNotFoundException("Tipo de seguro no encontrado.");
                }

                if (cotizacion.IdMoneda != existing.IdMoneda)
                {
                    var moneda = await _monedaRepo.GetByIdAsync(cotizacion.IdMoneda);
                    if (moneda == null) throw new KeyNotFoundException("Moneda no encontrada.");
                }

                // Obtener (o recalcular) la tasa usando la misma regla que en Create
                decimal tasa;
                using (var conn = _dapperContext.CreateConnection())
                {
                    var p = new DynamicParameters();
                    p.Add("@IdTipoSeguro", cotizacion.IdTipoSeguro, DbType.Int32);
                    p.Add("@SumaAsegurada", cotizacion.SumaAsegurada, DbType.Decimal);

                    var tasaDesdeRegla = await conn.QueryFirstOrDefaultAsync<decimal?>(
                        "dbo.sp_Tasa_GetPorTipoYSuma", p, commandType: CommandType.StoredProcedure);

                    if (tasaDesdeRegla.HasValue)
                        tasa = tasaDesdeRegla.Value;
                    else
                        tasa = cotizacion.Tasa; // mantener la tasa enviada si SP no devuelve nada
                }

                // Validar tasa resultante
                if (tasa < 0 || tasa > 1) throw new ArgumentException("Tasa inválida (0 - 1).");

                // Asignar tasa y recalcular prima neta
                cotizacion.Tasa = tasa;
                cotizacion.PrimaNeta = Math.Round(cotizacion.SumaAsegurada * cotizacion.Tasa, 2);

                // Conservar FechaCotizacion original (si se desea)
                cotizacion.FechaCotizacion = existing.FechaCotizacion;

                await _cotRepo.UpdateAsync(cotizacion);

                _logger.LogInformation("Cotización actualizada Id={Id}", cotizacion.IdCotizacion);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en CotizacionService.UpdateAsync {@Cotizacion}", cotizacion);
                throw;
            }
        }

        // Eliminación lógica de cotización
        public async Task DeleteAsync(long id, string usuarioModificacion)
        {
            try
            {
                if (id <= 0) throw new ArgumentException("Id inválido.");
                if (string.IsNullOrWhiteSpace(usuarioModificacion)) throw new ArgumentException("UsuarioModificacion es requerido.");

                // Verificar existencia
                var existing = await _cotRepo.GetByIdAsync(id);
                if (existing == null) throw new KeyNotFoundException("Cotización no encontrada.");

                await _cotRepo.DeleteAsync(id, usuarioModificacion);

                _logger.LogInformation("Cotización eliminada (lógicamente) Id={Id} Usuario={Usuario}", id, usuarioModificacion);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en CotizacionService.DeleteAsync CotizacionId={Id}", id);
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
            // El repositorio devuelve PagedResult<Models.Cotizacion>
            var paged = await _cotRepo.GetReportePaginadoAsync(desde, hasta, idTipoSeguro, filtro, pageNumber, pageSize);

            // Mapear entidad -> ReadDto
            var items = paged.Items.Select(c => new CotizacionReadDto
            {
                IdCotizacion = c.IdCotizacion,
                NumeroCotizacion = c.NumeroCotizacion,
                FechaCotizacion = c.FechaCotizacion,
                IdTipoSeguro = c.IdTipoSeguro,
                IdMoneda = c.IdMoneda,
                IdCliente = c.IdCliente,
                DescripcionBien = c.DescripcionBien,
                SumaAsegurada = c.SumaAsegurada,
                Tasa = c.Tasa,
                PrimaNeta = c.PrimaNeta,
                Observaciones = c.Observaciones,
                // Si en tu modelo Cotizacion tienes más campos que quieras exponer, los agregas aquí
            }).ToList();

            return new PagedResult<CotizacionReadDto>
            {
                Items = items,
                TotalCount = paged.TotalCount,
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize
            };
        }
    }
}
