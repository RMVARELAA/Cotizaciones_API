using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AutoMapper;
using System.Collections.Generic;

using Cotizaciones_API.DTOs.Cotizacion;
using Cotizaciones_API.Interfaces.Cotizacion;
using Cotizaciones_API.Models;

namespace Cotizaciones_API.Controllers.Cotizacion
{
    [ApiController]
    [Route("api/[controller]")]
    public class CotizacionesController : ControllerBase
    {
        private readonly ICotizacionService _cotService;
        private readonly IMapper _mapper;
        private readonly ILogger<CotizacionesController> _logger;

        public CotizacionesController(
            ICotizacionService cotService,
            IMapper mapper,
            ILogger<CotizacionesController> logger)
        {
            _cotService = cotService;
            _mapper = mapper;
            _logger = logger;
        }

        // POST: api/cotizaciones
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CotizacionCreateDto dto)
        {
            try
            {
                if (dto == null) return BadRequest("Payload inválido.");
                if (!ModelState.IsValid) return ValidationProblem(ModelState);

                var (id, numero) = await _cotService.CreateAsync(dto);

                // Obtener la cotización creada
                var created = await _cotService.GetByIdAsync(id);
                var readDto = _mapper.Map<CotizacionReadDto>(created);

                // Añadir el número generado si el mapping no lo incluye
                if (readDto != null && string.IsNullOrWhiteSpace(readDto.NumeroCotizacion))
                    readDto.NumeroCotizacion = numero;

                return CreatedAtAction(nameof(GetById), new { id = id }, new { Id = id, Numero = numero, Cotizacion = readDto });
            }
            catch (KeyNotFoundException knf)
            {
                // FK not found (cliente, moneda, tipo seguro)
                _logger.LogWarning(knf, "FK not found when creating cotizacion");
                return NotFound(new { Message = knf.Message });
            }
            catch (ArgumentException ae)
            {
                _logger.LogWarning(ae, "Validation error when creating cotizacion");
                return BadRequest(new { Message = ae.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating cotizacion");
                return Problem("Error al crear cotización", statusCode: 500);
            }
        }

        // GET: api/cotizaciones/{id}
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById([FromRoute] long id)
        {
            try
            {
                var dto = await _cotService.GetByIdAsync(id); // debe devolver CotizacionReadDto?
                if (dto == null) return NotFound(new { Message = "Cotización no encontrada." });

                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetById CotizacionId={Id}", id);
                return Problem("Error al obtener cotización", statusCode: 500);
            }
        }

        // GET: api/cotizaciones/report?desde=2025-01-01&hasta=2025-12-31&idTipoSeguro=1
        [HttpGet("report")]
        public async Task<IActionResult> Report([FromQuery] DateTime? desde, [FromQuery] DateTime? hasta, [FromQuery] int? idTipoSeguro)
        {
            try
            {
                var rows = await _cotService.GetReportAsync(desde, hasta, idTipoSeguro);
                return Ok(rows);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating cotizaciones report");
                return Problem("Error al generar reporte", statusCode: 500);
            }
        }

        // PUT: api/cotizaciones/{id}
        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update([FromRoute] long id, [FromBody] CotizacionUpdateDto dto)
        {
            try
            {
                if (dto == null) return BadRequest("Payload inválido.");
                if (id != dto.IdCotizacion) return BadRequest("El id de ruta no coincide con el id del body.");

                if (!ModelState.IsValid) return ValidationProblem(ModelState);

                var model = _mapper.Map<Models.Cotizacion>(dto);

                await _cotService.UpdateAsync(model);

                return NoContent();
            }
            catch (KeyNotFoundException knf)
            {
                _logger.LogWarning(knf, "Cotización no encontrada al actualizar Id={Id}", id);
                return NotFound(new { Message = knf.Message });
            }
            catch (ArgumentException ae)
            {
                _logger.LogWarning(ae, "Validación fallida al actualizar cotización");
                return BadRequest(new { Message = ae.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Update CotizacionId={Id}", id);
                return Problem("Error al actualizar cotización", statusCode: 500);
            }
        }

        // DELETE: api/cotizaciones/{id}?usuario=someone
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete([FromRoute] long id, [FromQuery] string? usuario)
        {
            try
            {
                if (id <= 0) return BadRequest("Id inválido.");
                if (string.IsNullOrWhiteSpace(usuario)) return BadRequest("Usuario es requerido en query string (usuario).");

                await _cotService.DeleteAsync(id, usuario);

                return NoContent();
            }
            catch (KeyNotFoundException knf)
            {
                _logger.LogWarning(knf, "Cotización no encontrada al eliminar Id={Id}", id);
                return NotFound(new { Message = knf.Message });
            }
            catch (ArgumentException ae)
            {
                _logger.LogWarning(ae, "Validación fallida al eliminar cotización");
                return BadRequest(new { Message = ae.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Delete CotizacionId={Id}", id);
                return Problem("Error al eliminar cotización", statusCode: 500);
            }
        }
    }
}
