using APIClinica.Models;

namespace APIClinica.Repositories
{
    public interface IDoctorRepository : IRepository<Doctor>
    {
        Task<IEnumerable<Doctor>> GetActiveDoctorsAsync();
        Task<IEnumerable<Doctor>> GetAllWithServicesAsync();
        Task<Doctor?> GetDoctorWithServicesByIdAsync(int id);
        Task<IEnumerable<Doctor>> GetDoctorsByServiceAsync(int serviceId);
    }
}