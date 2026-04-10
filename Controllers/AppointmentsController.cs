using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using APIClinica.Services;
using APIClinica.DTOs;
using APIClinica.Models.Enums;

namespace APIClinica.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _service;

        public AppointmentsController(IAppointmentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll(
            [FromQuery] DateTime? date = null,
            [FromQuery] int? doctorId = null,
            [FromQuery] int? patientId = null,
            [FromQuery] AppointmentStatus? status = null)
        {
            var appointments = await _service.GetAllAsync(date, doctorId, patientId, status);
            return Ok(appointments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AppointmentDTO>> GetById(int id)
        {
            var appointment = await _service.GetByIdAsync(id);
            if (appointment == null)
                return NotFound();
            return Ok(appointment);
        }

        [HttpPost]
        public async Task<ActionResult<AppointmentDTO>> Create([FromBody] CreateAppointmentDTO dto)
        {
            var appointment = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, appointment);
        }

        [HttpPatch("{id}/status")]
        public async Task<ActionResult> UpdateStatus(int id, [FromBody] UpdateStatusDTO dto)
        {
            var result = await _service.UpdateStatusAsync(id, dto.Status);
            if (!result)
                return NotFound();
            return NoContent();
        }

        [HttpGet("availability")]
        public async Task<ActionResult> GetAvailability([FromQuery] int doctorId, [FromQuery] DateTime date)
        {
            var availability = await _service.GetAvailableTimesAsync(doctorId, date);
            return Ok(availability);
        }
    }

    public class UpdateStatusDTO
    {
        public AppointmentStatus Status { get; set; }
    }
}