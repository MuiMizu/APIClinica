using APIClinica.Models;
using APIClinica.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace APIClinica.Data
{
    public class ClinicaDbContext : DbContext
    {
        public ClinicaDbContext(DbContextOptions<ClinicaDbContext> options) : base(options) { }

        public DbSet<Insurance> Insurances { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<DoctorService> DoctorServices { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Patient>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Document).IsUnique();
                entity.HasOne(e => e.Insurance)
                .WithMany(s => s.Patients)
                .HasForeignKey(e => e.InsuranceId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<DoctorService>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.DoctorId, e.ServiceId }).IsUnique();
            });

            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.DoctorId, e.Date, e.Time, e.Status })
                .HasDatabaseName("UX_Appointments_SlotPerHour")
                .IsUnique()
                .HasFilter("[Status] = 0");
            });

            modelBuilder.Entity<Insurance>().HasData(
                new Insurance { Id = 1, Name = "Seguro A", Description = "Seguro Basico", Active = true },
                new Insurance { Id = 2, Name = "Seguro B", Description = "Seguro Premium", Active = true },
                new Insurance { Id = 3, Name = "Seguro C", Description = "Seguro Familiar", Active = true }
            );

            modelBuilder.Entity<Service>().HasData(
                new Service { Id = 1, Name = "Consultas Generales", Description = "Consultas Medicas Generales", Active = true },
                new Service { Id = 2, Name = "Cardiologia", Description = "Especialidad en Cardiologia", Active = true },
                new Service { Id = 3, Name = "Pediatra", Description = "Especialidad en Pediatria", Active = true },
                new Service { Id = 4, Name = "Dermatologia", Description = "Especialidad en Dermatologia", Active = true },
                new Service { Id = 5, Name = "Neurologia", Description = "Especialidad en Neurologia", Active = true }
            );

            modelBuilder.Entity<Doctor>().HasData(
                new Doctor { Id = 1, FirstName = "Juan", LastName = "Pérez", Specialty = "Medico General", Active = true },
                new Doctor { Id = 2, FirstName = "María", LastName = "González", Specialty = "Cardiologia", Active = true },
                new Doctor { Id = 3, FirstName = "Carlos", LastName = "Rodríguez", Specialty = "Pediatra", Active = true },
                new Doctor { Id = 4, FirstName = "Ana", LastName = "Martínez", Specialty = "Dermatologia", Active = true }
            );

            modelBuilder.Entity<DoctorService>().HasData(
                new DoctorService { Id = 1, DoctorId = 1, ServiceId = 1 },
                new DoctorService { Id = 2, DoctorId = 2, ServiceId = 2 },
                new DoctorService { Id = 3, DoctorId = 2, ServiceId = 1 },
                new DoctorService { Id = 4, DoctorId = 3, ServiceId = 3 },
                new DoctorService { Id = 5, DoctorId = 3, ServiceId = 1 },
                new DoctorService { Id = 6, DoctorId = 4, ServiceId = 4 },
                new DoctorService { Id = 7, DoctorId = 4, ServiceId = 1 }
            );
        }
    }
}