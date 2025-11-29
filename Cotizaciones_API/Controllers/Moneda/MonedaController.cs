using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;

using Cotizaciones_API.DTOs.Moneda;
using Cotizaciones_API.Interfaces.Moneda;
using Cotizaciones_API.Models;

namespace Cotizaciones_API.Controllers.Moneda
{
    [ApiController]
    [Route("api/[controller]")]
    public class MonedaController : ControllerBase
    {
        private readonly IMonedaService _service;
        private readonly IMapper _mapper;
        private readonly ILogger<MonedaController> _logger;

        public MonedaController(IMonedaService service, IMapper mapper, ILogger<MonedaController> logger)
        {
            _service = service;
            _mapper = mapper;
            _logger = logger;
        }

        // POST: api/monedas
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MonedaCreateDto dto)
        {
            try
            {
                if (dto == null) return BadRequest("Payload inválido.");
                if (!ModelState.IsValid) return ValidationProblem(ModelState);

                var model = _mapper.Map<Models.Moneda>(dto);
                var id = await _service.CreateAsync(model);

                var created = await _service.GetByIdAsync(id);
                var readDto = _mapper.Map<MonedaReadDto>(created);

                return CreatedAtAction(nameof(GetById), new { id = id }, readDto);
            }
            catch (ArgumentException ae)
            {
                _logger.LogWarning(ae, "Validación fallida al crear moneda");
                return BadRequest(new { Message = ae.Message });
            }
            catch (InvalidOperationException ioe)
            {
                _logger.LogWarning(ioe, "Operación inválida al crear moneda");
                return Conflict(new { Message = ioe.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear moneda");
                return Problem(detail: "Ocurrió un error al procesar la solicitud.", statusCode: 500);
            }
        }

        // GET: api/monedas
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var list = await _service.GetAllAsync();
                var dtos = _mapper.Map<IEnumerable<MonedaReadDto>>(list);
                return Ok(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetAll monedas");
                return Problem("Error al obtener monedas", statusCode: 500);
            }
        }

        // GET: api/monedas/select  -> devuelve lista simple para dropdowns { Id, Nombre }
        [HttpGet("select")]
        public async Task<IActionResult> GetSelect()
        {
            try
            {
                var list = await _service.GetAllAsync();
                var simplified = list.Select(x => new { Id = x.IdMoneda, Nombre = x.Nombre ?? x.CodigoISO ?? $"Moneda {x.IdMoneda}" });
                return Ok(simplified);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetSelect monedas");
                return Problem("Error al obtener monedas (select)", statusCode: 500);
            }
        }

        // GET: api/monedas/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                var item = await _service.GetByIdAsync(id);
                if (item == null) return NotFound(new { Message = "Moneda no encontrada." });

                var dto = _mapper.Map<MonedaReadDto>(item);
                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetById monedaId={Id}", id);
                return Problem("Error al obtener moneda", statusCode: 500);
            }
        }

        // PUT: api/monedas/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] MonedaUpdateDto dto)
        {
            try
            {
                if (dto == null) return BadRequest("Payload inválido.");
                if (id != dto.IdMoneda) return BadRequest("El id de ruta no coincide con el id del body.");
                if (!ModelState.IsValid) return ValidationProblem(ModelState);

                var model = _mapper.Map<Models.Moneda>(dto);
                await _service.UpdateAsync(model);
                return NoContent();
            }
            catch (KeyNotFoundException knf)
            {
                _logger.LogWarning(knf, "Moneda no encontrada al actualizar Id={Id}", id);
                return NotFound(new { Message = knf.Message });
            }
            catch (ArgumentException ae)
            {
                _logger.LogWarning(ae, "Validación fallida al actualizar moneda");
                return BadRequest(new { Message = ae.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Update monedaId={Id}", id);
                return Problem("Error al actualizar moneda", statusCode: 500);
            }
        }

        // DELETE: api/monedas/{id}?usuario=someone
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id, [FromQuery] string? usuario)
        {
            try
            {
                if (id <= 0) return BadRequest("Id inválido.");
                if (string.IsNullOrWhiteSpace(usuario)) return BadRequest("Usuario es requerido en query string (usuario).");

                await _service.DeleteAsync(id, usuario);
                return NoContent();
            }
            catch (KeyNotFoundException knf)
            {
                _logger.LogWarning(knf, "Moneda no encontrada al eliminar Id={Id}", id);
                return NotFound(new { Message = knf.Message });
            }
            catch (ArgumentException ae)
            {
                _logger.LogWarning(ae, "Validación fallida al eliminar moneda");
                return BadRequest(new { Message = ae.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Delete monedaId={Id}", id);
                return Problem("Error al eliminar moneda", statusCode: 500);
            }
        }
    }
}
