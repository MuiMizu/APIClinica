using APIClinica.DTOs;

namespace APIClinica.Services
{
    public interface IPatientService
    {
        Task<(List<PatientDTO> Items, int TotalItems, int CurrentPage, int TotalPages)>
        GetAllAsync(int page = 1, int pageSize = 10, string? search = null);
        Task<PatientDTO?> GetByIdAsync(int id);
        Task<PatientDTO> CreateAsync(CreatePatientDTO dto);
        Task<PatientDTO?> UpdateAsync(int id, CreatePatientDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
    