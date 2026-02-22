namespace ProductManagementAPI.Models
{
    public class Delivery
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string TrackingNumber { get; set; } = string.Empty;
        public string CarrierName { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending"; // Pending, In Transit, Out for Delivery, Delivered, Failed
        public DateTime? ShippedDate { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public string DeliveryAddress { get; set; } = string.Empty;
        public string? DeliveryNotes { get; set; }
        public string? SignedBy { get; set; }
        public Order? Order { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
