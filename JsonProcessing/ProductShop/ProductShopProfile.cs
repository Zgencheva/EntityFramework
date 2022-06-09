using AutoMapper;
using ProductShop.DataTransferObjects;
using ProductShop.Models;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            this.CreateMap<UserInputModel, User>();
            this.CreateMap<ProductsInputModel, Product>();
            this.CreateMap<CategoryInputModel, Category>();
            this.CreateMap<CategoryProductsInputModel, CategoryProduct>();

            this.CreateMap<Product, ProductExportDto>()
                .ForMember(x => x.seller, y => y.MapFrom(c => c.Seller.FirstName + " " + c.Seller.LastName));

            

        }
    }
}
