namespace APIClinica.DTOs
{
    public class AvailableTimeDTO
    {
        public string Time { get; set; } = string.Empty;
        public int AvailableSlots { get; set; }
    }

    public class DoctorDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Specialty { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public bool Active { get; set; }
    }

    public class CreateDoctorDTO
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Specialty { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
    }
}
