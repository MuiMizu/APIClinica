using Microsoft.AspNetCore.Mvc;
using APIClinica.Services;

namespace APIClinica.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public DoctorsController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpGet("available")]
        public async Task<ActionResult> GetAvailable([FromQuery] int serviceId, [FromQuery] DateTime date)
        {
            var doctors = await _appointmentService.GetAvailableDoctorsAsync(serviceId, date);
            return Ok(doctors);
        }
    }
}