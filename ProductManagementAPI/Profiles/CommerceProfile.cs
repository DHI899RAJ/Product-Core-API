using AutoMapper;
using ProductManagementAPI.Models;
using ProductManagementAPI.Models.DTOs;

namespace ProductManagementAPI.Profiles
{
    public class CommerceProfile : Profile
    {
        public CommerceProfile()
        {
            // Category Mappings
            CreateMap<CategoryCreateDto, Category>();
            CreateMap<CategoryUpdateDto, Category>();
            CreateMap<Category, CategoryDto>();
            CreateMap<Category, CategoryResponseDto>();

            // Order Mappings
            CreateMap<OrderCreateDto, Order>();
            CreateMap<OrderUpdateDto, Order>();
            CreateMap<Order, OrderDto>();
            CreateMap<Order, OrderResponseDto>();

            // OrderItem Mappings
            CreateMap<OrderItemDto, OrderItem>();
            CreateMap<OrderItem, OrderItemDto>();

            // Delivery Mappings
            CreateMap<DeliveryCreateDto, Delivery>();
            CreateMap<DeliveryUpdateDto, Delivery>();
            CreateMap<Delivery, DeliveryDto>();
            CreateMap<Delivery, DeliveryResponseDto>();

            // Inventory Mappings
            CreateMap<InventoryCreateDto, Inventory>();
            CreateMap<InventoryUpdateDto, Inventory>();
            CreateMap<Inventory, InventoryDto>();
            CreateMap<Inventory, InventoryResponseDto>();

            // Payment Mappings
            CreateMap<PaymentCreateDto, Payment>();
            CreateMap<PaymentUpdateDto, Payment>();
            CreateMap<Payment, PaymentDto>();
            CreateMap<Payment, PaymentResponseDto>();
        }
    }
}
