namespace ProductManagementAPI.Models.DTOs
{
    public class DeliveryCreateDto
    {
        public int OrderId { get; set; }
        public string CarrierName { get; set; } = string.Empty;
        public string DeliveryAddress { get; set; } = string.Empty;
        public string? DeliveryNotes { get; set; }
    }

    public class DeliveryUpdateDto
    {
        public string Status { get; set; } = string.Empty;
        public string? DeliveryNotes { get; set; }
        public string? SignedBy { get; set; }
        public DateTime? DeliveredDate { get; set; }
    }

    public class DeliveryDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string TrackingNumber { get; set; } = string.Empty;
        public string CarrierName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? ShippedDate { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public string DeliveryAddress { get; set; } = string.Empty;
    }

    public class DeliveryResponseDto
    {
        public int Id { get; set; }
        public string TrackingNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string CarrierName { get; set; } = string.Empty;
    }
}
