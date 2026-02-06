using Microsoft.EntityFrameworkCore;
using APIClinica.Data;
using APIClinica.Models;
using APIClinica.Models.Enums;

namespace APIClinica.Repositories
{
    public class AppointmentRepository : Repository<Appointment>, IAppointmentRepository
    {
        public AppointmentRepository(ClinicaDbContext context) : base(context)
        {
        }

        private IQueryable<Appointment> WithDetails() => _dbSet
            .Include(a => a.Patient)
            .Include(a => a.Doctor);

        public async Task<IEnumerable<Appointment>> GetAppointmentsWithDetailsAsync()
        {
            return await WithDetails().ToListAsync();
        }

        public async Task<Appointment?> GetAppointmentWithDetailsByIdAsync(int id)
        {
            return await WithDetails().FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByDateAsync(DateTime date)
        {
            return await WithDetails().Where(a => a.Date.Date == date.Date).ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByDoctorAsync(int doctorId)
        {
            return await WithDetails().Where(a => a.DoctorId == doctorId).ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByPatientAsync(int patientId)
        {
            return await WithDetails().Where(a => a.PatientId == patientId).ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByStatusAsync(AppointmentStatus status)
        {
            return await WithDetails().Where(a => a.Status == status).ToListAsync();
        }

        public async Task<int> CountAppointmentsByDoctorAndTimeAsync(int doctorId, DateTime date, string time)
        {
            return await _dbSet
                .CountAsync(a => a.DoctorId == doctorId &&
                a.Date.Date == date.Date &&
                a.Time == time &&
                a.Status == AppointmentStatus.Scheduled);
        }
    }
}