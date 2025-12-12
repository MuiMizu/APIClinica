using Microsoft.EntityFrameworkCore;
using APIClinica.Data;
using APIClinica.Models;

namespace APIClinica.Repositories
{
    public class PatientRepository : Repository<Patient>, IPatientRepository
    {
        public PatientRepository(ClinicaDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Patient>> GetPatientsWithInsuranceAsync()
        {
            return await _dbSet
                .Include(p => p.Insurance)
                .ToListAsync();
        }

        public async Task<Patient?> GetPatientWithInsuranceByIdAsync(int id)
        {
            return await _dbSet
                .Include(p => p.Insurance)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Patient>> SearchPatientsAsync(string searchTerm)
        {
            return await _dbSet
                .Include(p => p.Insurance)
                .Where(p => p.FirstName.Contains(searchTerm) ||
                p.LastName.Contains(searchTerm) ||
                p.Document.Contains(searchTerm))
                .ToListAsync();
        }
    }
}