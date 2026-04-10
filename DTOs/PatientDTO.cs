namespace APIClinica.DTOs
{
    public class PatientDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Document { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public int InsuranceId { get; set; }
        public string? InsuranceName { get; set; }
        public bool HasAppointments { get; set; }
    }

    public class CreatePatientDTO
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Document { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public int InsuranceId { get; set; }
    }
}
