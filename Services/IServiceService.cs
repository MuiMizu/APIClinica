using APIClinica.DTOs;

namespace APIClinica.Services
{
    public interface IServiceService
    {
        Task<List<ServiceDTO>> GetAllAsync(bool onlyActive = true);
        Task<ServiceDTO?> GetByIdAsync(int id);
        Task<ServiceDTO> CreateAsync(CreateServiceDTO dto);
        Task UpdateAsync(int id, CreateServiceDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
