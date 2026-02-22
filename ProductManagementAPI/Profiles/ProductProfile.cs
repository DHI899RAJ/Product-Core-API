using AutoMapper;
using ProductManagementAPI.Models;
using ProductManagementAPI.Models.DTOs;

namespace ProductManagementAPI.Profiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<ProductCreateDto, Product>();
            CreateMap<ProductUpdateDto, Product>();
            CreateMap<Product, ProductResponseDto>();
        }
    }
}
