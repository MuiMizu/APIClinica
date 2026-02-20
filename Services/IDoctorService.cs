using APIClinica.DTOs;

namespace APIClinica.Services
{
    public interface IDoctorService
    {
        Task<List<DoctorDTO>> GetAllAsync(string? search = null);
        Task<DoctorDTO?> GetByIdAsync(int id);
        Task<DoctorDTO> CreateAsync(CreateDoctorDTO dto);
        Task UpdateAsync(int id, CreateDoctorDTO dto);
        Task<bool> DeleteAsync(int id);
        Task<List<AvailableDoctorDTO>> GetAvailableDoctorsAsync(int serviceId, DateTime date);
    }
}
