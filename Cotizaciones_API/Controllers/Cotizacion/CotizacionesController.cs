using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;

using Cotizaciones_API.DTOs.Cotizacion;
using Cotizaciones_API.Interfaces.Cotizacion;
using Cotizaciones_API.Interfaces.Utils;
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
        private readonly IExcelExporter _excelExporter;

        public CotizacionesController(
            ICotizacionService cotService,
            IMapper mapper,
            ILogger<CotizacionesController> logger,
            IExcelExporter excelExporter)
        {
            _cotService = cotService;
            _mapper = mapper;
            _logger = logger;
            _excelExporter = excelExporter;
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

                // Obtener la cotización creada (el servicio devuelve CotizacionReadDto)
                var readDto = await _cotService.GetByIdAsync(id);

                // Añadir el número generado si el registro no lo contiene
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

        // GET: api/cotizaciones
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var list = await _cotService.GetAllAsync(); // devuelve IEnumerable<CotizacionReadDto> por ejemplo
                if (list == null || !list.Any()) return NoContent();
                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo todas las cotizaciones");
                return Problem("Error al obtener cotizaciones", statusCode: 500);
            }
        }


        // GET: api/cotizaciones/{id}
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById([FromRoute] long id)
        {
            try
            {
                var dto = await _cotService.GetByIdAsync(id);
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
        public async Task<IActionResult> Report([FromQuery] DateTime? desde, [FromQuery] DateTime? hasta, [FromQuery] int? idTipoSeguro, [FromQuery] string? format = null)
        {
            try
            {
                // Validaciones básicas de parámetros
                if (desde.HasValue && hasta.HasValue && desde > hasta)
                    return BadRequest(new { Message = "El parámetro 'desde' no puede ser mayor que 'hasta'." });

                var rows = await _cotService.GetReportAsync(desde, hasta, idTipoSeguro);

                // materializar para comprobar contenido y evitar múltiples enumeraciones
                var list = rows as IList<dynamic> ?? rows?.ToList();

                if (list == null || list.Count == 0)
                {
                    return NoContent();
                }

                // Si piden Excel, generar y devolver el fichero
                if (!string.IsNullOrWhiteSpace(format) && format.Equals("excel", StringComparison.OrdinalIgnoreCase))
                {
                    // Nombre de fichero legible con rango y filtro
                    var desdeStr = desde?.ToString("yyyyMMdd") ?? "start";
                    var hastaStr = hasta?.ToString("yyyyMMdd") ?? "end";
                    var tipoStr = idTipoSeguro.HasValue ? $"_Tipo{ idTipoSeguro.Value }" : string.Empty;
                    var fileName = $"Reporte_Cotizaciones_{desdeStr}_{hastaStr}{tipoStr}_{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx";

                    // Title para la hoja
                    var title = $"Reporte de Cotizaciones";
                    if (desde.HasValue || hasta.HasValue)
                        title += $" ({desde?.ToString("yyyy-MM-dd") ?? ""} → {hasta?.ToString("yyyy-MM-dd") ?? ""})";

                    var bytes = _excelExporter.ExportToExcel(list, "Cotizaciones", title);

                    const string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    return File(bytes, contentType, fileName);
                }

                // Por defecto devolver JSON
                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating report");
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
