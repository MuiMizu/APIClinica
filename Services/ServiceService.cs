using APIClinica.DTOs;
using APIClinica.Models;
using APIClinica.Repositories;
using Microsoft.EntityFrameworkCore;

namespace APIClinica.Services
{
    public class ServiceService : IServiceService
    {
        private readonly IRepository<Service> _serviceRepository;
        private readonly IRepository<APIClinica.Models.DoctorService> _doctorServiceRepository;

        public ServiceService(
            IRepository<Service> serviceRepository,
            IRepository<APIClinica.Models.DoctorService> doctorServiceRepository)
        {
            _serviceRepository = serviceRepository;
            _doctorServiceRepository = doctorServiceRepository;
        }

        public async Task<List<ServiceDTO>> GetAllAsync(bool onlyActive = true)
        {
            var servicesQuery = _serviceRepository.GetQueryable()
                .Include(s => s.DoctorServices).AsQueryable();

            if (onlyActive) servicesQuery = servicesQuery.Where(s => s.Active);

            var services = await servicesQuery.ToListAsync();

            return services.Select(s => new ServiceDTO
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                Active = s.Active,
                IsAssignedToDoctors = s.DoctorServices != null && s.DoctorServices.Any()
            }).OrderBy(s => s.Name).ToList();
        }

        public async Task<ServiceDTO?> GetByIdAsync(int id)
        {
            var s = await _serviceRepository.GetQueryable()
                .Include(s => s.DoctorServices)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (s == null) return null;

            return new ServiceDTO
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                Active = s.Active,
                IsAssignedToDoctors = s.DoctorServices != null && s.DoctorServices.Any()
            };
        }

        public async Task<ServiceDTO> CreateAsync(CreateServiceDTO dto)
        {
            if (await _serviceRepository.GetQueryable().AnyAsync(s => s.Name == dto.Name && s.Active))
            {
                throw new InvalidOperationException("Ya existe un servicio con este nombre");
            }

            if (!APIClinica.Help.Validation.IsSafeDescription(dto.Description))
                throw new ArgumentException("La descripción contiene caracteres no permitidos");

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

            if (!APIClinica.Help.Validation.IsSafeDescription(dto.Description))
                throw new ArgumentException("No agregar caracteres especiales en descripcion");

            service.Name = dto.Name;
            service.Description = dto.Description;

            _serviceRepository.Update(service);
            await _serviceRepository.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var service = await _serviceRepository.GetByIdAsync(id);
            if (service == null) return false;

            var isAssignedToDoctor = await _doctorServiceRepository.ExistsAsync(ds => ds.ServiceId == id);
            if (isAssignedToDoctor)
            {
                throw new InvalidOperationException("No se puede servicio con un doctor asignado");
            }

            service.Active = false;
            _serviceRepository.Update(service);
            await _serviceRepository.SaveChangesAsync();
            return true;
        }
    }
}
