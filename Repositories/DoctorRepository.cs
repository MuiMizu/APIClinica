using Microsoft.EntityFrameworkCore;
using APIClinica.Data;
using APIClinica.Models;

namespace APIClinica.Repositories
{
    public class DoctorRepository : Repository<Doctor>, IDoctorRepository
    {
        public DoctorRepository(ClinicaDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Doctor>> GetActiveDoctorsAsync()
        {
            return await _dbSet
                .ToListAsync();
        }

        public async Task<IEnumerable<Doctor>> GetAllWithServicesAsync()
        {
            return await _dbSet
                .Include(d => d.DoctorServices)
                .Include(d => d.Appointments)
                .ToListAsync();
        }

        public async Task<Doctor?> GetDoctorWithServicesByIdAsync(int id)
        {
            return await _dbSet
                .Include(d => d.DoctorServices)
                    .ThenInclude(ds => ds.Service)
                .Include(d => d.Appointments)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<IEnumerable<Doctor>> GetDoctorsByServiceAsync(int serviceId)
        {
            return await _dbSet
                .Include(d => d.DoctorServices)
                .Where(d => d.DoctorServices.Any(ds => ds.ServiceId == serviceId))
                .ToListAsync();
        }
    }
}