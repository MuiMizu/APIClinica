using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using APIClinica.Repositories;
using APIClinica.Models;

using APIClinica.DTOs;

namespace APIClinica.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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

        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(int id)
        {
            var service = await _serviceRepository.GetByIdAsync(id);
            if (service == null) return NotFound();
            return Ok(new { service.Id, service.Name, service.Description });
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateServiceDTO dto)
        {
            var service = new Service
            {
                Name = dto.Name,
                Description = dto.Description,
                Active = true
            };
            await _serviceRepository.AddAsync(service);
            await _serviceRepository.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = service.Id }, new { service.Id, service.Name, service.Description });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, CreateServiceDTO dto)
        {
            var service = await _serviceRepository.GetByIdAsync(id);
            if (service == null) return NotFound();

            service.Name = dto.Name;
            service.Description = dto.Description;

            _serviceRepository.Update(service);
            await _serviceRepository.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var service = await _serviceRepository.GetByIdAsync(id);
            if (service == null) return NotFound();

            service.Active = false;
            _serviceRepository.Update(service);
            await _serviceRepository.SaveChangesAsync();
            return NoContent();
        }
    }
}
