using AutoMapper;
using Bulky.Models;
using Bulky.Models.ViewModels;

namespace BulkyWeb.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Product, ProductVM>();
            CreateMap<ProductVM, Product>();

            CreateMap<Category, CategoryVM>();
            CreateMap<CategoryVM, Category>();

            CreateMap<Company, CompanyVM>();
            CreateMap<CompanyVM, Company>();

            CreateMap<ShopCart, ShopCartVM>();
            CreateMap<ShopCartVM, ShopCart>();

            CreateMap<ShopCartItem, ShopCartItemVM>();
            CreateMap<ShopCartItemVM, ShopCartItem>();

            CreateMap<PurchaseItem, ShopCartItem>();
            CreateMap<ShopCartItem, PurchaseItem>();
        }
    }
}
