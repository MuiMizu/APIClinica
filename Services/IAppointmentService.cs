using APIClinica.DTOs;
using APIClinica.Models.Enums;

namespace APIClinica.Services
{
    public interface IAppointmentService
    {
        Task<List<AppointmentDTO>> GetAllAsync(DateTime? date = null, int? doctorId = null, int? patientId = null, AppointmentStatus? status = null);
        Task<AppointmentDTO?> GetByIdAsync(int id);
        Task<AppointmentDTO> CreateAsync(CreateAppointmentDTO dto);
        Task<bool> UpdateStatusAsync(int id, AppointmentStatus status);
        Task<List<AvailableDoctorDTO>> GetAvailableDoctorsAsync(int serviceId, DateTime date);
        Task<List<AvailableTimeDTO>> GetAvailableTimesAsync(int doctorId, DateTime date);
    }
}