using System.ComponentModel.DataAnnotations;

namespace APIClinica.Models
{
    public class Doctor
    {
        public int Id { get; set; }

        [Required]
        [StringLength(32)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(32)]
        public string LastName { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Specialty { get; set; }

        [StringLength(16)]
        public string? Phone { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        public bool Active { get; set; } = true;
        public ICollection<DoctorService> DoctorServices { get; set; } = new List<DoctorService>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}