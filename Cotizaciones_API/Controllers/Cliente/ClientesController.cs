using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AutoMapper;
using System.Collections.Generic;

using Cotizaciones_API.DTOs.Cliente;
using Cotizaciones_API.Interfaces.Cliente;
using Cotizaciones_API.Models;

namespace Cotizaciones_API.Controllers.Cliente
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly IClienteService _clienteService;
        private readonly IMapper _mapper;
        private readonly ILogger<ClientesController> _logger;

        public ClientesController(
            IClienteService clienteService,
            IMapper mapper,
            ILogger<ClientesController> logger)
        {
            _clienteService = clienteService;
            _mapper = mapper;
            _logger = logger;
        }

        // POST: api/clientes
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ClienteCreateDto dto)
        {
            try
            {
                if (dto == null) return BadRequest("Payload inválido.");

                // Validación automática por FluentValidation si está registrada
                if (!ModelState.IsValid) return ValidationProblem(ModelState);

                // Map DTO -> Modelo (tu IClienteService espera Models.Cliente)
                var clienteModel = _mapper.Map<Models.Cliente>(dto);

                var id = await _clienteService.CreateAsync(clienteModel);

                // Obtener el recurso creado para devolver (opcional: GetByIdAsync)
                var created = await _clienteService.GetByIdAsync(id);

                var readDto = _mapper.Map<ClienteReadDto>(created);

                return CreatedAtAction(nameof(GetById), new { id = id }, readDto);
            }
            catch (ArgumentException ae)
            {
                _logger.LogWarning(ae, "Validación fallida al crear cliente");
                return BadRequest(new { Message = ae.Message });
            }
            catch (InvalidOperationException ioe)
            {
                // por ejemplo: identidad duplicada
                _logger.LogWarning(ioe, "Operación inválida al crear cliente");
                return Conflict(new { Message = ioe.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear cliente");
                return Problem(detail: "Ocurrió un error al procesar la solicitud.", statusCode: 500);
            }
        }

        // GET: api/clientes
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var list = await _clienteService.GetAllAsync();
                var dtos = _mapper.Map<IEnumerable<ClienteReadDto>>(list);
                return Ok(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetAll clientes");
                return Problem("Error al obtener clientes", statusCode: 500);
            }
        }

        // GET: api/clientes/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                var cliente = await _clienteService.GetByIdAsync(id);
                if (cliente == null) return NotFound(new { Message = "Cliente no encontrado." });

                var dto = _mapper.Map<ClienteReadDto>(cliente);
                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetById clienteId={Id}", id);
                return Problem("Error al obtener cliente", statusCode: 500);
            }
        }

        // PUT: api/clientes/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] ClienteUpdateDto dto)
        {
            try
            {
                if (dto == null) return BadRequest("Payload inválido.");
                if (id != dto.IdCliente) return BadRequest("El id de ruta no coincide con el id del body.");

                if (!ModelState.IsValid) return ValidationProblem(ModelState);

                // Map update DTO -> Modelo
                var model = _mapper.Map<Models.Cliente>(dto);

                await _clienteService.UpdateAsync(model);

                return NoContent();
            }
            catch (KeyNotFoundException knf)
            {
                _logger.LogWarning(knf, "Cliente no encontrado al actualizar clienteId={Id}", id);
                return NotFound(new { Message = knf.Message });
            }
            catch (ArgumentException ae)
            {
                _logger.LogWarning(ae, "Validación fallida al actualizar cliente");
                return BadRequest(new { Message = ae.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Update clienteId={Id}", id);
                return Problem("Error al actualizar cliente", statusCode: 500);
            }
        }

        // DELETE: api/clientes/{id}?usuario=someone
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id, [FromQuery] string? usuario)
        {
            try
            {
                if (id <= 0) return BadRequest("Id inválido.");
                if (string.IsNullOrWhiteSpace(usuario)) return BadRequest("Usuario es requerido en query string (usuario).");

                await _clienteService.DeleteAsync(id, usuario);

                return NoContent();
            }
            catch (KeyNotFoundException knf)
            {
                _logger.LogWarning(knf, "Cliente no encontrado al eliminar clienteId={Id}", id);
                return NotFound(new { Message = knf.Message });
            }
            catch (ArgumentException ae)
            {
                _logger.LogWarning(ae, "Validación fallida al eliminar cliente");
                return BadRequest(new { Message = ae.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Delete clienteId={Id}", id);
                return Problem("Error al eliminar cliente", statusCode: 500);
            }
        }
    }
}
