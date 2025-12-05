using APIClinica.Models;
using APIClinica.Models.Enums;

namespace APIClinica.Repositories
{
    public interface IAppointmentRepository : IRepository<Appointment>
    {
        Task<IEnumerable<Appointment>> GetAppointmentsWithDetailsAsync();
        Task<Appointment?> GetAppointmentWithDetailsByIdAsync(int id);
        Task<IEnumerable<Appointment>> GetAppointmentsByDateAsync(DateTime date);
        Task<IEnumerable<Appointment>> GetAppointmentsByDoctorAsync(int doctorId);
        Task<IEnumerable<Appointment>> GetAppointmentsByPatientAsync(int patientId);
        Task<IEnumerable<Appointment>> GetAppointmentsByStatusAsync(AppointmentStatus status);
        Task<int> CountAppointmentsByDoctorAndTimeAsync(int doctorId, DateTime date, string time);
    }
}