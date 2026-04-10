using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using APIClinica.Repositories;
using APIClinica.Models;
using APIClinica.Services;


using APIClinica.DTOs;

namespace APIClinica.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ServicesController : ControllerBase
    {
        private readonly IServiceService _service;

        public ServicesController(IServiceService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var services = await _service.GetAllAsync();
            return Ok(services);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(int id)
        {
            var service = await _service.GetByIdAsync(id);
            if (service == null) return NotFound();
            return Ok(service);
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateServiceDTO dto)
        {
            var service = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = service.Id }, service);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, CreateServiceDTO dto)
        {
            await _service.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
