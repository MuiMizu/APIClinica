namespace APIClinica.DTOs
{
    public class ServiceDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool Active { get; set; }
    }

    public class CreateServiceDTO
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
