using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cotizaciones_API.Interfaces.Cliente;
using Cotizaciones_API.Interfaces;
using Cotizaciones_API.DTOs.Cliente;
using Cotizaciones_API.Models;
using Microsoft.Extensions.Logging;

namespace Cotizaciones_API.Services.Cliente
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepo;
        private readonly ILogger<ClienteService> _logger;

        public ClienteService(IClienteRepository clienteRepo, ILogger<ClienteService> logger)
        {
            _clienteRepo = clienteRepo;
            _logger = logger;
        }

        public async Task<int> CreateAsync(Models.Cliente dto)
        {
            try
            {
                // reglas de negocio ligeras (validaciones de DTO ya las hace FluentValidation)
                if (string.IsNullOrWhiteSpace(dto.Nombre))
                    throw new ArgumentException("Nombre es requerido.");

                var entity = new Models.Cliente
                {
                    Nombre = dto.Nombre?.Trim(),
                    Identidad = dto.Identidad?.Trim(),
                    FechaNacimiento = dto.FechaNacimiento,
                    IdTipoCliente = dto.IdTipoCliente,
                    Telefono = dto.Telefono,
                    Email = dto.Email,
                    Direccion = dto.Direccion,
                    UsuarioCreacion = dto.UsuarioCreacion,
                    Estado = true
                };

                var id = await _clienteRepo.InsertAsync(entity);
                return id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en ClienteService.CreateAsync {@Dto}", dto);
                throw;
            }
        }

        public Task<Models.Cliente> GetByIdAsync(int id)
        {
            return _clienteRepo.GetByIdAsync(id);
        }

        public Task<IEnumerable<Models.Cliente>> GetAllAsync()
        {
            return _clienteRepo.GetAllAsync();
        }
    }
}
