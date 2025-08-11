using AutoMapper;
using ShopSpire.Utilities.DTO;
using ShopSpire.Utilities.DTO.Cart;
using ShopSpire.Utilities.DTO.Order;
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
                .ForMember(p=>p.SellerName,o=>o.MapFrom(s=>s.Seller.FirstName+" "+s.Seller.LastName))
                .ReverseMap();
            CreateMap<ProductCreateDTO, Product>().ReverseMap();
            CreateMap<UpdateProductDTO, Product>().ReverseMap();
            //Cart
            CreateMap<Cart, CartDTO>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(src => src.Product.Price))
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Product.Price * src.Quantity))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Product.Category.Name));
            //Product Order
            CreateMap<ProductOrder, ProductOrderDTO>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.OrderNumber, opt => opt.MapFrom(src => src.Order.Id))
            .ForMember(dest => dest.LineTotal, opt => opt.MapFrom(src => src.LineTotal));

            CreateMap<CreateProductOrderDTO, ProductOrder>();
            CreateMap<UpdateProductOrderDTO, ProductOrder>();
        }
    }
}
