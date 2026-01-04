using Microsoft.AspNetCore.Mvc;
using APIClinica.Repositories;
using APIClinica.Models;

namespace APIClinica.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
    }
}