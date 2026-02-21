namespace ProductManagementAPI.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Foreign Keys
        public int CategoryId { get; set; }
        public int SupplierId { get; set; }

        // Navigation Properties
        public Category? Category { get; set; }
        public Supplier? Supplier { get; set; }
    }
}
