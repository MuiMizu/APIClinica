using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIClinica.Models
{
    public class Patient
    {
        public int Id { get; set; }

        [Required]
        [StringLength(32)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(32)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [StringLength(16)]
        public string Document { get; set; } = string.Empty;

        [StringLength(10)]
        public string? Phone { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public int InsuranceId { get; set; }

        [ForeignKey(nameof(InsuranceId))]
        public Insurance Insurance { get; set; } = null!;

        public bool Active { get; set; } = true;
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}