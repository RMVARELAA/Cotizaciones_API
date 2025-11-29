using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cotizaciones_API.Interfaces.Moneda;
using Cotizaciones_API.Models;
using Microsoft.Extensions.Logging;

namespace Cotizaciones_API.Services.Moneda
{
    public class MonedaService : IMonedaService
    {
        private readonly IMonedaRepository _repo;
        private readonly ILogger<MonedaService> _logger;

        public MonedaService(IMonedaRepository repo, ILogger<MonedaService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<int> CreateAsync(Models.Moneda model)
        {
            try
            {
                return await _repo.CreateAsync(model);
            }
            catch (ArgumentException ae)
            {
                _logger.LogWarning(ae, "Validación fallida en MonedaService.CreateAsync {@Moneda}", model);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en MonedaService.CreateAsync {@Moneda}", model);
                throw;
            }
        }

        public async Task<IEnumerable<Models.Moneda>> GetAllAsync()
        {
            try
            {
                return await _repo.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en MonedaService.GetAllAsync");
                throw;
            }
        }

        public async Task<Models.Moneda?> GetByIdAsync(int id)
        {
            try
            {
                return await _repo.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en MonedaService.GetByIdAsync Id={Id}", id);
                throw;
            }
        }

        public async Task UpdateAsync(Models.Moneda model)
        {
            try
            {
                await _repo.UpdateAsync(model);
            }
            catch (KeyNotFoundException knf)
            {
                _logger.LogWarning(knf, "Moneda no encontrada en MonedaService.UpdateAsync Id={Id}", model?.IdMoneda);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en MonedaService.UpdateAsync {@Moneda}", model);
                throw;
            }
        }

        public async Task DeleteAsync(int id, string usuarioModificacion)
        {
            try
            {
                await _repo.DeleteAsync(id, usuarioModificacion);
            }
            catch (KeyNotFoundException knf)
            {
                _logger.LogWarning(knf, "Moneda no encontrada en MonedaService.DeleteAsync Id={Id}", id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en MonedaService.DeleteAsync Id={Id} Usuario={Usuario}", id, usuarioModificacion);
                throw;
            }
        }
    }
}
