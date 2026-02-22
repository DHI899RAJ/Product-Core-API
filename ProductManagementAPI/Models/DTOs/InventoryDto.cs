namespace ProductManagementAPI.Models.DTOs
{
    public class InventoryCreateDto
    {
        public int ProductId { get; set; }
        public int QuantityOnHand { get; set; }
        public int ReorderLevel { get; set; }
        public int ReorderQuantity { get; set; }
        public string WarehouseLocation { get; set; } = string.Empty;
    }

    public class InventoryUpdateDto
    {
        public int QuantityOnHand { get; set; }
        public int QuantityReserved { get; set; }
        public int ReorderLevel { get; set; }
        public string WarehouseLocation { get; set; } = string.Empty;
    }

    public class InventoryDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int QuantityOnHand { get; set; }
        public int QuantityReserved { get; set; }
        public int QuantityAvailable { get; set; }
        public int ReorderLevel { get; set; }
        public string WarehouseLocation { get; set; } = string.Empty;
    }

    public class InventoryResponseDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int QuantityAvailable { get; set; }
        public int ReorderLevel { get; set; }
    }
}
