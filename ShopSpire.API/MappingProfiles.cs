using AutoMapper;
using ShopSpire.Utilities.DTO;
using ShopSpireCore.Entities;

namespace ShopSpire.API
{
    public class MappingProfiles:Profile
    {
        public MappingProfiles()
        {
            //Product
            CreateMap<Product, ProductDTO>()
                .ForMember(p => p.CategoryName, o => o.MapFrom(s => s.Category.Name))
                .ForMember(p=>p.SellerName,o=>o.MapFrom(s=>s.Seller.FirstName+""+s.Seller.LastName))
                .ReverseMap();
            CreateMap<ProductCreateDTO, Product>().ReverseMap();
            CreateMap<UpdateProductDTO, Product>().ReverseMap();
        }
    }
}
