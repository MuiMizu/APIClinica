using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using APIClinica.Repositories;
using APIClinica.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace APIClinica.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InsurancesController : ControllerBase
    {
        private readonly IRepository<Insurance> _insuranceRepository;
        private readonly IRepository<Patient> _patientRepository;

        public InsurancesController(IRepository<Insurance> insuranceRepository, IRepository<Patient> patientRepository)
        {
            _insuranceRepository = insuranceRepository;
            _patientRepository = patientRepository;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var insurances = await _insuranceRepository.GetQueryable()
                .Include(i => i.Patients)
                .Where(i => i.Active)
                .ToListAsync();

            var result = insurances.Select(i => new { 
                i.Id, 
                i.Name, 
                i.Description,
                IsAssignedToPatients = i.Patients != null && i.Patients.Any()
            });
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
            if (await _insuranceRepository.GetQueryable().AnyAsync(i => i.Name == dto.Name && i.Active))
            {
                return Conflict(new { message = "Ya existe un seguro con este nombre" });
            }

            if (!APIClinica.Help.Validation.IsSafeDescription(dto.Description))
                return BadRequest(new { message = "La descripción contiene caracteres no permitidos" });

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

            if (!APIClinica.Help.Validation.IsSafeDescription(dto.Description))
                return BadRequest(new { message = "La descripción contiene caracteres no permitidos" });
            
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

            var isAssignedToPatient = await _patientRepository.ExistsAsync(p => p.InsuranceId == id);
            if (isAssignedToPatient)
            {
                return BadRequest(new { message = "No se puede eliminar un seguro que está asignado a uno o más pacientes" });
            }

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