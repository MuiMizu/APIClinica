using Microsoft.AspNetCore.Mvc;
using APIClinica.Repositories;
using APIClinica.Models;

namespace APIClinica.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServicesController : ControllerBase
    {
        private readonly IRepository<Service> _serviceRepository;

        public ServicesController(IRepository<Service> serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var services = await _serviceRepository.FindAsync(s => s.Active);
            var result = services.Select(s => new { s.Id, s.Name, s.Description });
            return Ok(result);
        }
    }
}