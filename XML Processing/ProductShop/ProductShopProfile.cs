using AutoMapper;
using ProductShop.Dtos;
using ProductShop.Dtos.Import;
using ProductShop.Dtos.Export;
using ProductShop.Models;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            this.CreateMap<UserInputModel, User>();
            this.CreateMap<ProductImputModel, Product>();
            this.CreateMap<CategoriesInputModel, Category>();
            this.CreateMap<CategoriesProductsInputModels, CategoryProduct>();

            //this.CreateMap<Product, ProductsInangeExportDTO>()
            //    .ForMember(x => x.Buyer, y => y.MapFrom(s => s.Buyer.FirstName + " " + s.Buyer.LastName));
        }
    }
}
