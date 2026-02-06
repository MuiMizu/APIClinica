using APIClinica.DTOs;
using APIClinica.Models;
using APIClinica.Repositories;
using APIClinica.Help;
using System;

namespace APIClinica.Services
{ public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IRepository<Insurance> _insuranceRepository;
        public PatientService(
            IPatientRepository patientRepository,
            IRepository<Insurance> insuranceRepository)
        {
            _patientRepository = patientRepository;
            _insuranceRepository = insuranceRepository;
        }

        public async Task<(List<PatientDTO> Items, int TotalItems, int CurrentPage, int TotalPages)>
            GetAllAsync(int page = 1, int pageSize = 10, string? search = null)
        {
            IEnumerable<Patient> patients;

            if (!string.IsNullOrWhiteSpace(search))
            {
                patients = await _patientRepository.SearchPatientsAsync(search);
            }
            else
            {
                patients = await _patientRepository.GetPatientsWithInsuranceAsync();
            }

            var totalItems = patients.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var items = patients
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PatientDTO
                {
                    Id = p.Id,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    Document = p.Document,
                    Phone = p.Phone,
                    Email = p.Email,
                    InsuranceId = p.InsuranceId,
                    InsuranceName = p.Insurance?.Name ?? string.Empty
                })
                .ToList();

            return (items, totalItems, page, totalPages);
        }
        public async Task<PatientDTO?> GetByIdAsync(int id)
        {
            var patient = await _patientRepository.GetPatientWithInsuranceByIdAsync(id);
            if (patient == null)
                return null;

            return new PatientDTO
            {
                Id = patient.Id,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                Document = patient.Document,
                Phone = patient.Phone,
                Email = patient.Email,
                InsuranceId = patient.InsuranceId,
                InsuranceName = patient.Insurance?.Name ?? string.Empty
            };
        }
        public async Task<PatientDTO> CreateAsync(CreatePatientDTO dto)
        {
            if (!Validation.IsOnlyLetters(dto.FirstName))
                throw new ArgumentException("Nombre solo debe tener letras");
            if (!Validation.IsOnlyLetters(dto.LastName))
                throw new ArgumentException("Apellido solo debe tener letras");
            if (!string.IsNullOrWhiteSpace(dto.Phone) && !Validation.IsOnlyDigits(dto.Phone))
                throw new ArgumentException("Telefono solo debe tener numeros");

            var insurance = await _insuranceRepository.GetByIdAsync(dto.InsuranceId);
            if (insurance == null || !insurance.Active)
                throw new ArgumentException("Seguro no encontrado");

            var patient = new Patient
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Document = dto.Document,
                Phone = dto.Phone,
                Email = dto.Email,
                InsuranceId = dto.InsuranceId
            };

            await _patientRepository.AddAsync(patient);
            await _patientRepository.SaveChangesAsync();

            return await GetByIdAsync(patient.Id) ?? throw new Exception("Error al crear paciente");
        }
        public async Task<PatientDTO> UpdateAsync(int id, CreatePatientDTO dto)
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null)
                throw new KeyNotFoundException("Paciente no encontrado");
            if (!Validation.IsOnlyLetters(dto.FirstName))
                throw new ArgumentException("Nombre solo debe tener letras");
            if (!Validation.IsOnlyLetters(dto.LastName))
                throw new ArgumentException("Apellido solo debe tener letras");
            if (!string.IsNullOrWhiteSpace(dto.Phone) && !Validation.IsOnlyDigits(dto.Phone))
                throw new ArgumentException("Telefono solo debe tener numeros");

            var insurance = await _insuranceRepository.GetByIdAsync(dto.InsuranceId);
            if (insurance == null || !insurance.Active)
                throw new ArgumentException("Seguro no encontrado");

            patient.FirstName = dto.FirstName;
            patient.LastName = dto.LastName;
            patient.Document = dto.Document;
            patient.Phone = dto.Phone;
            patient.Email = dto.Email;
            patient.InsuranceId = dto.InsuranceId;

            _patientRepository.Update(patient);
            await _patientRepository.SaveChangesAsync();

            return await GetByIdAsync(id) ?? throw new Exception("Error al actualizar paciente");
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null)
                return false;

            _patientRepository.Remove(patient);
            await _patientRepository.SaveChangesAsync();
            return true;
        }
    }
}