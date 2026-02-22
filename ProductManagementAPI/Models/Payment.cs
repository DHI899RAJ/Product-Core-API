namespace ProductManagementAPI.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty; // CreditCard, DebitCard, PayPal, Bank Transfer, etc.
        public string Status { get; set; } = "Pending"; // Pending, Completed, Failed, Refunded
        public string TransactionId { get; set; } = string.Empty;
        public string? Reference { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime? RefundDate { get; set; }
        public string? RefundReason { get; set; }
        public Order? Order { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
