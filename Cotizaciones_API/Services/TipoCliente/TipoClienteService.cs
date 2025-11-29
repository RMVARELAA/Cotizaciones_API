using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cotizaciones_API.Interfaces.TipoCliente;
using Cotizaciones_API.Models;
using Microsoft.Extensions.Logging;

namespace Cotizaciones_API.Services.TipoCliente
{
    public class TipoClienteService : ITipoClienteService
    {
        private readonly ITipoClienteRepository _repo;
        private readonly ILogger<TipoClienteService> _logger;

        public TipoClienteService(ITipoClienteRepository repo, ILogger<TipoClienteService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<int> CreateAsync(Models.TipoCliente model)
        {
            try
            {
                return await _repo.CreateAsync(model);
            }
            catch (ArgumentException ae)
            {
                _logger.LogWarning(ae, "Validación fallida en TipoClienteService.CreateAsync {@TipoCliente}", model);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en TipoClienteService.CreateAsync {@TipoCliente}", model);
                throw;
            }
        }

        public async Task<IEnumerable<Models.TipoCliente>> GetAllAsync()
        {
            try
            {
                return await _repo.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en TipoClienteService.GetAllAsync");
                throw;
            }
        }

        public async Task<Models.TipoCliente?> GetByIdAsync(int id)
        {
            try
            {
                return await _repo.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en TipoClienteService.GetByIdAsync Id={Id}", id);
                throw;
            }
        }

        public async Task UpdateAsync(Models.TipoCliente model)
        {
            try
            {
                await _repo.UpdateAsync(model);
            }
            catch (KeyNotFoundException knf)
            {
                _logger.LogWarning(knf, "TipoCliente no encontrado en TipoClienteService.UpdateAsync Id={Id}", model?.IdTipoCliente);
                throw;
            }
            catch (ArgumentException ae)
            {
                _logger.LogWarning(ae, "Validación fallida en TipoClienteService.UpdateAsync {@TipoCliente}", model);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en TipoClienteService.UpdateAsync {@TipoCliente}", model);
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
                _logger.LogWarning(knf, "TipoCliente no encontrado en TipoClienteService.DeleteAsync Id={Id}", id);
                throw;
            }
            catch (ArgumentException ae)
            {
                _logger.LogWarning(ae, "Validación fallida en TipoClienteService.DeleteAsync Id={Id}", id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en TipoClienteService.DeleteAsync Id={Id} Usuario={Usuario}", id, usuarioModificacion);
                throw;
            }
        }
    }
}
