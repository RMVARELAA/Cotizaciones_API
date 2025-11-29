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

                // Calcular PrimaNeta (2 decimales)
                var prima = Math.Round(dto.SumaAsegurada * dto.Tasa, 2);

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

        public Task<IEnumerable<dynamic>> GetReportAsync(DateTime? desde, DateTime? hasta, int? idTipoSeguro)
        {
            return _cotRepo.GetReportAsync(desde, hasta, idTipoSeguro);
        }

        public Task<Models.Cotizacion> GetByIdAsync(long id)
        {
            return _cotRepo.GetByIdAsync(id);
        }

        // Actualiza una cotización existente. Recalcula PrimaNeta y valida FKs si cambian.
        public async Task UpdateAsync(Models.Cotizacion cotizacion)
        {
            try
            {
                if (cotizacion == null) throw new ArgumentNullException(nameof(cotizacion));
                if (cotizacion.IdCotizacion <= 0) throw new ArgumentException("IdCotizacion inválido.");
                if (cotizacion.SumaAsegurada <= 0) throw new ArgumentException("SumaAsegurada debe ser mayor a 0.");
                if (cotizacion.Tasa < 0 || cotizacion.Tasa > 1) throw new ArgumentException("Tasa inválida (0 - 1).");

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

                // Recalcular prima neta
                cotizacion.PrimaNeta = Math.Round(cotizacion.SumaAsegurada * cotizacion.Tasa, 2);
                cotizacion.FechaCotizacion = existing.FechaCotizacion; // conservar fecha original a menos que quieras modificarla

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
    }
}
