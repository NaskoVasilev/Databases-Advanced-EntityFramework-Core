using AutoMapper;
using ProductShop.Dtos;
using ProductShop.Models;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            CreateMap<Product, SoldProductDto>();
        }
    }
}
