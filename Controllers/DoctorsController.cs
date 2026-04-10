using Microsoft.AspNetCore.Mvc;
using APIClinica.Services;
using APIClinica.DTOs;

namespace APIClinica.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorService _service;

        public DoctorsController(IDoctorService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll([FromQuery] string? search = null)
        {
            var doctors = await _service.GetAllAsync(search);
            return Ok(doctors);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DoctorDTO>> GetById(int id)
        {
            var doctor = await _service.GetByIdAsync(id);
            if (doctor == null)
                return NotFound();
            return Ok(doctor);
        }

        [HttpPost]
        public async Task<ActionResult<DoctorDTO>> Create([FromBody] CreateDoctorDTO dto)
        {
            var doctor = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = doctor.Id }, doctor);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] CreateDoctorDTO dto)
        {
            await _service.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }

        [HttpGet("available")]
        public async Task<ActionResult> GetAvailable([FromQuery] int serviceId, [FromQuery] DateTime date)
        {
            var doctors = await _service.GetAvailableDoctorsAsync(serviceId, date);
            return Ok(doctors);
        }
    }
}
