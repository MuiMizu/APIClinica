using APIClinica.DTOs;
using APIClinica.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIClinica.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _service;

        public PatientsController(IPatientService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
        {
            var result = await _service.GetAllAsync(page, pageSize, search);
            return Ok(new
            {
                items = result.Items,
                currentPage = result.CurrentPage,
                totalPages = result.TotalPages,
                totalItems = result.TotalItems
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PatientDTO>> GetById(int id)
        {
            var patient = await _service.GetByIdAsync(id);
            if (patient == null)
                return NotFound();
            return Ok(patient);
        }

        [HttpPost]  
        public async Task<ActionResult<PatientDTO>> Create([FromBody] CreatePatientDTO dto)
        {
            try
            {
                var patient = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = patient.Id }, patient);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PatientDTO>> Update(int id, [FromBody] CreatePatientDTO dto)
        {
            try
            {
                var patient = await _service.UpdateAsync(id, dto);
                return Ok(patient);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }
    }
}