using APIClinica.DTOs;
using APIClinica.Models;
using APIClinica.Repositories;

namespace APIClinica.Services
{
    public class ServiceService : IServiceService
    {
        private readonly IRepository<Service> _serviceRepository;

        public ServiceService(IRepository<Service> serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        public async Task<List<ServiceDTO>> GetAllAsync(bool onlyActive = true)
        {
            var services = onlyActive 
                ? await _serviceRepository.FindAsync(s => s.Active)
                : await _serviceRepository.GetAllAsync();

            return services.Select(s => new ServiceDTO
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                Active = s.Active
            }).OrderBy(s => s.Name).ToList();
        }

        public async Task<ServiceDTO?> GetByIdAsync(int id)
        {
            var s = await _serviceRepository.GetByIdAsync(id);
            if (s == null) return null;

            return new ServiceDTO
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                Active = s.Active
            };
        }

        public async Task<ServiceDTO> CreateAsync(CreateServiceDTO dto)
        {
            var service = new Service
            {
                Name = dto.Name,
                Description = dto.Description,
                Active = true
            };

            await _serviceRepository.AddAsync(service);
            await _serviceRepository.SaveChangesAsync();

            return new ServiceDTO
            {
                Id = service.Id,
                Name = service.Name,
                Description = service.Description,
                Active = service.Active
            };
        }

        public async Task UpdateAsync(int id, CreateServiceDTO dto)
        {
            var service = await _serviceRepository.GetByIdAsync(id);
            if (service == null) throw new KeyNotFoundException("Service not found");

            service.Name = dto.Name;
            service.Description = dto.Description;

            _serviceRepository.Update(service);
            await _serviceRepository.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var service = await _serviceRepository.GetByIdAsync(id);
            if (service == null) return false;

            // Soft delete
            service.Active = false;
            _serviceRepository.Update(service);
            await _serviceRepository.SaveChangesAsync();
            return true;
        }
    }
}
