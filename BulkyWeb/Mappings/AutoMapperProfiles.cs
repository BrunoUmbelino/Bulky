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
        }
    }
}
