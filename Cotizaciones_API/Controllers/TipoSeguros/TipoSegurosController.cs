using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;

using Cotizaciones_API.DTOs.TipoSeguro;
using Cotizaciones_API.Interfaces.TipoSeguro;
using Cotizaciones_API.Models;

namespace Cotizaciones_API.Controllers.TipoSeguro
{
    [ApiController]
    [Route("api/[controller]")]
    public class TipoSegurosController : ControllerBase
    {
        private readonly ITipoSeguroService _service;
        private readonly IMapper _mapper;
        private readonly ILogger<TipoSegurosController> _logger;

        public TipoSegurosController(ITipoSeguroService service, IMapper mapper, ILogger<TipoSegurosController> logger)
        {
            _service = service;
            _mapper = mapper;
            _logger = logger;
        }

        // POST: api/tiposeguros
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TipoSeguroCreateDto dto)
        {
            try
            {
                if (dto == null) return BadRequest("Payload inválido.");
                if (!ModelState.IsValid) return ValidationProblem(ModelState);

                var model = _mapper.Map<Models.TipoSeguro>(dto);
                var id = await _service.CreateAsync(model);

                var created = await _service.GetByIdAsync(id);
                var readDto = _mapper.Map<TipoSeguroReadDto>(created);

                return CreatedAtAction(nameof(GetById), new { id = id }, readDto);
            }
            catch (ArgumentException ae)
            {
                _logger.LogWarning(ae, "Validación fallida al crear tipo seguro");
                return BadRequest(new { Message = ae.Message });
            }
            catch (InvalidOperationException ioe)
            {
                _logger.LogWarning(ioe, "Operación inválida al crear tipo seguro");
                return Conflict(new { Message = ioe.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear tipo seguro");
                return Problem(detail: "Ocurrió un error al procesar la solicitud.", statusCode: 500);
            }
        }

        // GET: api/tiposeguros
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var list = await _service.GetAllAsync();
                var dtos = _mapper.Map<IEnumerable<TipoSeguroReadDto>>(list);
                return Ok(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetAll tipos seguros");
                return Problem("Error al obtener tipos de seguro", statusCode: 500);
            }
        }

        // GET: api/tiposeguros/select  -> devuelve lista simple para dropdowns { Id, Nombre }
        [HttpGet("select")]
        public async Task<IActionResult> GetSelect()
        {
            try
            {
                var list = await _service.GetAllAsync();
                var simplified = list.Select(x => new { Id = x.IdTipoSeguro, Nombre = x.NombreSeguro ?? $"Tipo {x.IdTipoSeguro}" });
                return Ok(simplified);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetSelect tipos seguros");
                return Problem("Error al obtener tipos de seguro (select)", statusCode: 500);
            }
        }

        // GET: api/tiposeguros/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                var item = await _service.GetByIdAsync(id);
                if (item == null) return NotFound(new { Message = "Tipo de seguro no encontrado." });

                var dto = _mapper.Map<TipoSeguroReadDto>(item);
                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetById tipoSeguroId={Id}", id);
                return Problem("Error al obtener tipo seguro", statusCode: 500);
            }
        }

        // PUT: api/tiposeguros/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] TipoSeguroUpdateDto dto)
        {
            try
            {
                if (dto == null) return BadRequest("Payload inválido.");
                if (id != dto.IdTipoSeguro) return BadRequest("El id de ruta no coincide con el id del body.");
                if (!ModelState.IsValid) return ValidationProblem(ModelState);

                var model = _mapper.Map<Models.TipoSeguro>(dto);
                await _service.UpdateAsync(model);
                return NoContent();
            }
            catch (KeyNotFoundException knf)
            {
                _logger.LogWarning(knf, "Tipo seguro no encontrado al actualizar Id={Id}", id);
                return NotFound(new { Message = knf.Message });
            }
            catch (ArgumentException ae)
            {
                _logger.LogWarning(ae, "Validación fallida al actualizar tipo seguro");
                return BadRequest(new { Message = ae.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Update tipoSeguroId={Id}", id);
                return Problem("Error al actualizar tipo seguro", statusCode: 500);
            }
        }

        // DELETE: api/tiposeguros/{id}?usuario=someone
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
                _logger.LogWarning(knf, "Tipo seguro no encontrado al eliminar Id={Id}", id);
                return NotFound(new { Message = knf.Message });
            }
            catch (ArgumentException ae)
            {
                _logger.LogWarning(ae, "Validación fallida al eliminar tipo seguro");
                return BadRequest(new { Message = ae.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Delete tipoSeguroId={Id}", id);
                return Problem("Error al eliminar tipo seguro", statusCode: 500);
            }
        }
    }
}
