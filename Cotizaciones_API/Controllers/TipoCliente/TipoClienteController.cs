using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Cotizaciones_API.DTOs.TipoCliente;
using Cotizaciones_API.Interfaces.TipoCliente;
using Cotizaciones_API.Models;

namespace Cotizaciones_API.Controllers.TipoCliente
{
    [ApiController]
    [Route("api/[controller]")]
    public class TipoClientesController : ControllerBase
    {
        private readonly ITipoClienteService _service;
        private readonly IMapper _mapper;
        private readonly ILogger<TipoClientesController> _logger;

        public TipoClientesController(
            ITipoClienteService service,
            IMapper mapper,
            ILogger<TipoClientesController> logger)
        {
            _service = service;
            _mapper = mapper;
            _logger = logger;
        }

        // POST: api/tipoclientes
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TipoClienteCreateDto dto)
        {
            try
            {
                if (dto == null) return BadRequest("Payload inválido.");
                if (!ModelState.IsValid) return ValidationProblem(ModelState);

                var model = _mapper.Map<Models.TipoCliente>(dto);
                var id = await _service.CreateAsync(model);

                var created = await _service.GetByIdAsync(id);
                var readDto = _mapper.Map<TipoClienteReadDto>(created);

                return CreatedAtAction(nameof(GetById), new { id = id }, readDto);
            }
            catch (ArgumentException ae)
            {
                _logger.LogWarning(ae, "Validación fallida al crear TipoCliente");
                return BadRequest(new { Message = ae.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear TipoCliente");
                return Problem(detail: "Ocurrió un error al crear el tipo de cliente.", statusCode: 500);
            }
        }

        // GET: api/tipoclientes
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var list = await _service.GetAllAsync();
                var dtos = _mapper.Map<IEnumerable<TipoClienteReadDto>>(list);
                return Ok(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetAll TipoClientes");
                return Problem(detail: "Ocurrió un error al obtener los tipos de cliente.", statusCode: 500);
            }
        }

        // GET: api/tipoclientes/select
        [HttpGet("select")]
        public async Task<IActionResult> GetSelect()
        {
            try
            {
                var list = await _service.GetAllAsync();
                var simplified = list.Select(x => new { Id = x.IdTipoCliente, Nombre = x.NombreTipoCliente });
                return Ok(simplified);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetSelect TipoClientes");
                return Problem(detail: "Ocurrió un error al obtener la lista para select.", statusCode: 500);
            }
        }

        // GET: api/tipoclientes/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var item = await _service.GetByIdAsync(id);
                if (item == null) return NotFound(new { Message = "Tipo de cliente no encontrado." });

                var dto = _mapper.Map<TipoClienteReadDto>(item);
                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error GetById TipoClienteId={Id}", id);
                return Problem(detail: "Ocurrió un error al obtener el tipo de cliente.", statusCode: 500);
            }
        }

        // PUT: api/tipoclientes/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] TipoClienteUpdateDto dto)
        {
            try
            {
                if (dto == null) return BadRequest("Payload inválido.");
                if (dto.IdTipoCliente != id) return BadRequest("El id no coincide.");
                if (!ModelState.IsValid) return ValidationProblem(ModelState);

                var model = _mapper.Map<Models.TipoCliente>(dto);
                await _service.UpdateAsync(model);

                return NoContent();
            }
            catch (KeyNotFoundException knf)
            {
                _logger.LogWarning(knf, "TipoCliente no encontrado al actualizar Id={Id}", id);
                return NotFound(new { Message = knf.Message });
            }
            catch (ArgumentException ae)
            {
                _logger.LogWarning(ae, "Validación fallida al actualizar TipoCliente");
                return BadRequest(new { Message = ae.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Update TipoClienteId={Id}", id);
                return Problem(detail: "Ocurrió un error al actualizar el tipo de cliente.", statusCode: 500);
            }
        }

        // DELETE: api/tipoclientes/{id}?usuario=admin
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, [FromQuery] string usuario)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(usuario))
                    return BadRequest("Debe indicar usuario.");

                await _service.DeleteAsync(id, usuario);
                return NoContent();
            }
            catch (KeyNotFoundException knf)
            {
                _logger.LogWarning(knf, "TipoCliente no encontrado al eliminar Id={Id}", id);
                return NotFound(new { Message = knf.Message });
            }
            catch (ArgumentException ae)
            {
                _logger.LogWarning(ae, "Validación fallida al eliminar TipoCliente");
                return BadRequest(new { Message = ae.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Delete TipoClienteId={Id}", id);
                return Problem(detail: "Ocurrió un error al eliminar el tipo de cliente.", statusCode: 500);
            }
        }
    }
}
