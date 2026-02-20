using APIClinica.DTOs;
using APIClinica.Models;
using APIClinica.Repositories;
using APIClinica.Models.Enums;

namespace APIClinica.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IAppointmentRepository _appointmentRepository;

        public DoctorService(IDoctorRepository doctorRepository, IAppointmentRepository appointmentRepository)
        {
            _doctorRepository = doctorRepository;
            _appointmentRepository = appointmentRepository;
        }

        public async Task<List<DoctorDTO>> GetAllAsync(string? search = null)
        {
            var doctors = await _doctorRepository.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(search))
            {
                doctors = doctors.Where(d =>
                    d.FirstName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    d.LastName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    (d.Specialty != null && d.Specialty.Contains(search, StringComparison.OrdinalIgnoreCase))
                );
            }

            return doctors.Select(d => new DoctorDTO
            {
                Id = d.Id,
                FirstName = d.FirstName,
                LastName = d.LastName,
                Specialty = d.Specialty,
                Phone = d.Phone,
                Email = d.Email,
                Active = d.Active
            }).ToList();
        }

        public async Task<DoctorDTO?> GetByIdAsync(int id)
        {
            var d = await _doctorRepository.GetByIdAsync(id);
            if (d == null) return null;

            return new DoctorDTO
            {
                Id = d.Id,
                FirstName = d.FirstName,
                LastName = d.LastName,
                Specialty = d.Specialty,
                Phone = d.Phone,
                Email = d.Email,
                Active = d.Active
            };
        }

        public async Task<DoctorDTO> CreateAsync(CreateDoctorDTO dto)
        {
            var doctor = new Doctor
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Specialty = dto.Specialty,
                Phone = dto.Phone,
                Email = dto.Email,
                Active = true
            };

            await _doctorRepository.AddAsync(doctor);
            await _doctorRepository.SaveChangesAsync();

            return await GetByIdAsync(doctor.Id) ?? throw new Exception("Error creando doctor");
        }

        public async Task UpdateAsync(int id, CreateDoctorDTO dto)
        {
            var doctor = await _doctorRepository.GetByIdAsync(id);
            if (doctor == null) throw new KeyNotFoundException("Doctor no encontrado");

            doctor.FirstName = dto.FirstName;
            doctor.LastName = dto.LastName;
            doctor.Specialty = dto.Specialty;
            doctor.Phone = dto.Phone;
            doctor.Email = dto.Email;

            _doctorRepository.Update(doctor);
            await _doctorRepository.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var doctor = await _doctorRepository.GetByIdAsync(id);
            if (doctor == null) return false;

            _doctorRepository.Remove(doctor);
            await _doctorRepository.SaveChangesAsync();
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
    }
}
