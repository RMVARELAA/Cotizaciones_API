using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cotizaciones_API.Interfaces.TipoSeguro;
using Cotizaciones_API.Models;
using Microsoft.Extensions.Logging;

namespace Cotizaciones_API.Services.TipoSeguro
{
    public class TipoSeguroService : ITipoSeguroService
    {
        private readonly ITipoSeguroRepository _repo;
        private readonly ILogger<TipoSeguroService> _logger;

        public TipoSeguroService(ITipoSeguroRepository repo, ILogger<TipoSeguroService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<int> CreateAsync(Models.TipoSeguro model)
        {
            try
            {
                return await _repo.InsertAsync(model);
            }
            catch (ArgumentException ae)
            {
                _logger.LogWarning(ae, "Validación fallida en TipoSeguroService.CreateAsync {@TipoSeguro}", model);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en TipoSeguroService.CreateAsync {@TipoSeguro}", model);
                throw;
            }
        }

        public async Task<IEnumerable<Models.TipoSeguro>> GetAllAsync()
        {
            try
            {
                return await _repo.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en TipoSeguroService.GetAllAsync");
                throw;
            }
        }

        public async Task<Models.TipoSeguro?> GetByIdAsync(int id)
        {
            try
            {
                return await _repo.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en TipoSeguroService.GetByIdAsync Id={Id}", id);
                throw;
            }
        }

        public async Task UpdateAsync(Models.TipoSeguro model)
        {
            try
            {
                await _repo.UpdateAsync(model);
            }
            catch (KeyNotFoundException knf)
            {
                _logger.LogWarning(knf, "TipoSeguro no encontrado en TipoSeguroService.UpdateAsync Id={Id}", model?.IdTipoSeguro);
                throw;
            }
            catch (ArgumentException ae)
            {
                _logger.LogWarning(ae, "Validación fallida en TipoSeguroService.UpdateAsync {@TipoSeguro}", model);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en TipoSeguroService.UpdateAsync {@TipoSeguro}", model);
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
                _logger.LogWarning(knf, "TipoSeguro no encontrado en TipoSeguroService.DeleteAsync Id={Id}", id);
                throw;
            }
            catch (ArgumentException ae)
            {
                _logger.LogWarning(ae, "Validación fallida en TipoSeguroService.DeleteAsync Id={Id}", id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en TipoSeguroService.DeleteAsync Id={Id} Usuario={Usuario}", id, usuarioModificacion);
                throw;
            }
        }
    }
}
