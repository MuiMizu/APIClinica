using APIClinica.DTOs;
using APIClinica.Models;
using APIClinica.Models.Enums;
using APIClinica.Repositories;

namespace APIClinica.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IRepository<Service> _serviceRepository;

        public AppointmentService(
            IAppointmentRepository appointmentRepository,
            IDoctorRepository doctorRepository,
            IRepository<Service> serviceRepository)
        {
            _appointmentRepository = appointmentRepository;
            _doctorRepository = doctorRepository;
            _serviceRepository = serviceRepository;
        }

        public async Task<List<AppointmentDTO>> GetAllAsync(DateTime? date = null, int? doctorId = null, int? patientId = null, AppointmentStatus? status = null)
        {
            IEnumerable<Appointment> appointments;

            if (date.HasValue)
                appointments = await _appointmentRepository.GetAppointmentsByDateAsync(date.Value);
            else if (doctorId.HasValue)
                appointments = await _appointmentRepository.GetAppointmentsByDoctorAsync(doctorId.Value);
            else if (patientId.HasValue)
                appointments = await _appointmentRepository.GetAppointmentsByPatientAsync(patientId.Value);
            else if (status.HasValue)
                appointments = await _appointmentRepository.GetAppointmentsByStatusAsync(status.Value);
            else
                appointments = await _appointmentRepository.GetAppointmentsWithDetailsAsync();

            if (date.HasValue && appointments.Any())
                appointments = appointments.Where(a => a.Date.Date == date.Value.Date);

            if (doctorId.HasValue && appointments.Any())
                appointments = appointments.Where(a => a.DoctorId == doctorId.Value);

            if (patientId.HasValue && appointments.Any())
                appointments = appointments.Where(a => a.PatientId == patientId.Value);

            if (status.HasValue && appointments.Any())
                appointments = appointments.Where(a => a.Status == status.Value);

            return appointments
                .OrderBy(a => a.Date)
                .ThenBy(a => a.Time)
                .Select(a => new AppointmentDTO
                {
                    Id = a.Id,
                    PatientId = a.PatientId,
                    PatientName = $"{a.Patient.FirstName} {a.Patient.LastName}",
                    DoctorId = a.DoctorId,
                    DoctorName = $"{a.Doctor.FirstName} {a.Doctor.LastName}",
                    Date = a.Date,
                    Time = a.Time,
                    Status = a.Status,
                    ServiceName = a.Doctor.DoctorServices
                    .Select(ds => ds.Service.Name)
                    .FirstOrDefault() ?? string.Empty
                })
                .ToList();
        }

        public async Task<AppointmentDTO?> GetByIdAsync(int id)
        {
            var appointment = await _appointmentRepository.GetAppointmentWithDetailsByIdAsync(id);
            if (appointment == null)
                return null;

            return new AppointmentDTO
            {
                Id = appointment.Id,
                PatientId = appointment.PatientId,
                PatientName = $"{appointment.Patient.FirstName} {appointment.Patient.LastName}",
                DoctorId = appointment.DoctorId,
                DoctorName = $"{appointment.Doctor.FirstName} {appointment.Doctor.LastName}",
                Date = appointment.Date,
                Time = appointment.Time,
                Status = appointment.Status,
                ServiceName = appointment.Doctor.DoctorServices
                    .Select(ds => ds.Service.Name)
                    .FirstOrDefault() ?? string.Empty
            };
        }

        public async Task<AppointmentDTO> CreateAsync(CreateAppointmentDTO dto)
        {
            var doctor = await _doctorRepository.GetDoctorWithServicesByIdAsync(dto.DoctorId);
            if (doctor == null || !doctor.Active)
                throw new InvalidOperationException("Doctor no valido o no disponible");

            var validTimes = new[] { "08:00", "09:00", "10:00", "11:00", "12:00", "13:00", "14:00", "15:00", "16:00", "17:00" };
            if (!validTimes.Contains(dto.Time))
                throw new ArgumentException("Tiene que ser entre 08:00 y 17:00");


            if (dto.Date.Date < DateTime.Today)
                throw new ArgumentException("Ingrese una fecha valida");


            var maxAttempts = 2;
            var attempt = 0;

            while (attempt < maxAttempts)
            {
                try
                {
                    var existingAppointments = await _appointmentRepository
                        .CountAppointmentsByDoctorAndTimeAsync(dto.DoctorId, dto.Date, dto.Time);

                    if (existingAppointments >= 2)
                    {
                        throw new InvalidOperationException("No hay cupos disponibles");
                    }

                    var appointment = new Appointment
                    {
                        PatientId = dto.PatientId,
                        DoctorId = dto.DoctorId,
                        Date = dto.Date.Date,
                        Time = dto.Time,
                        Status = AppointmentStatus.Scheduled
                    };

                    await _appointmentRepository.AddAsync(appointment);
                    await _appointmentRepository.SaveChangesAsync();

                    return await GetByIdAsync(appointment.Id) ?? throw new Exception("No se pudo crear la cita");
                }
                catch (Exception ex) when (ex.Message.Contains("UX_Appointments_SlotPerHour"))
                {
                    attempt++;
                    if (attempt >= maxAttempts)
                    {
                        throw new InvalidOperationException("No hay cupos a disponibles a esta hora.");
                    }
                    await Task.Delay(100);
                }
            }

            throw new InvalidOperationException("No se pudo crear la cita");
        }

        public async Task<bool> UpdateStatusAsync(int id, AppointmentStatus status)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);
            if (appointment == null)
                return false;

            appointment.Status = status;
            _appointmentRepository.Update(appointment);
            await _appointmentRepository.SaveChangesAsync();
            return true;
        }

        public async Task<List<AvailableDoctorDTO>> GetAvailableDoctorsAsync(int serviceId, DateTime date)
        {

            var doctors = await _doctorRepository.GetDoctorsByServiceAsync(serviceId);

            var availableDoctors = new List<AvailableDoctorDTO>();

            foreach (var doctor in doctors)
            {
                var appointmentsOfDay = await _appointmentRepository
                    .FindAsync(a => a.DoctorId == doctor.Id &&
                    a.Date.Date == date.Date &&
                    a.Status == AppointmentStatus.Scheduled);

                var appointmentsByTime = appointmentsOfDay
                    .GroupBy(a => a.Time)
                    .Select(g => new { Time = g.Key, Count = g.Count() })
                    .ToList();


                var occupiedHours = appointmentsByTime.Where(a => a.Count >= 2).Count();
                var totalHours = 10;

                if (occupiedHours < totalHours)
                {
                    availableDoctors.Add(new AvailableDoctorDTO
                    {
                    Id = doctor.Id,
                    FirstName = doctor.FirstName,
                    LastName = doctor.LastName,
                    Specialty = doctor.Specialty
                    });
                }
            }

            return availableDoctors;
        }

        public async Task<List<AvailableTimeDTO>> GetAvailableTimesAsync(int doctorId, DateTime date)
        {
            var times = new[] { "08:00", "09:00", "10:00", "11:00", "12:00", "13:00", "14:00", "15:00", "16:00", "17:00" };
            var availability = new List<AvailableTimeDTO>();

            var appointmentsOfDay = await _appointmentRepository
                .FindAsync(a => a.DoctorId == doctorId &&
                a.Date.Date == date.Date &&
                a.Status == AppointmentStatus.Scheduled);

            var appointmentsByTime = appointmentsOfDay
                .GroupBy(a => a.Time)
                .ToDictionary(g => g.Key, g => g.Count());

            foreach (var time in times)
            {
                var occupiedSlots = appointmentsByTime.GetValueOrDefault(time, 0);
                var availableSlots = Math.Max(0, 2 - occupiedSlots);

                availability.Add(new AvailableTimeDTO
                {
                    Time = time,
                    AvailableSlots = availableSlots
                });
            }

            return availability;
        }
    }
}