namespace ProductManagementAPI.Models
{
    public class Supplier
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ContactEmail { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Property
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
