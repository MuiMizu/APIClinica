using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using APIClinica.Repositories;
using APIClinica.Models;
using System.Threading.Tasks;
using System.Linq;

namespace APIClinica.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InsurancesController : ControllerBase
    {
        private readonly IRepository<Insurance> _insuranceRepository;

        public InsurancesController(IRepository<Insurance> insuranceRepository)
        {
            _insuranceRepository = insuranceRepository;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var insurances = await _insuranceRepository.FindAsync(i => i.Active);
            var result = insurances.Select(i => new { i.Id, i.Name, i.Description });
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(int id)
        {
            var insurance = await _insuranceRepository.GetByIdAsync(id);
            if (insurance == null) return NotFound();
            return Ok(new { insurance.Id, insurance.Name, insurance.Description });
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateInsuranceDTO dto)
        {
            var insurance = new Insurance
            {
                Name = dto.Name,
                Description = dto.Description,
                Active = true
            };
            await _insuranceRepository.AddAsync(insurance);
            await _insuranceRepository.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = insurance.Id }, new { insurance.Id, insurance.Name, insurance.Description });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, CreateInsuranceDTO dto)
        {
            var insurance = await _insuranceRepository.GetByIdAsync(id);
            if (insurance == null) return NotFound();

            insurance.Name = dto.Name;
            insurance.Description = dto.Description;

            _insuranceRepository.Update(insurance);
            await _insuranceRepository.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var insurance = await _insuranceRepository.GetByIdAsync(id);
            if (insurance == null) return NotFound();

            insurance.Active = false;
            _insuranceRepository.Update(insurance);
            await _insuranceRepository.SaveChangesAsync();
            return NoContent();
        }
    }

    public class CreateInsuranceDTO
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}