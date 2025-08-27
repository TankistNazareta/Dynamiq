using AutoMapper;
using Dynamiq.Application.DTOs.AccountDTOs;
using Dynamiq.Application.DTOs.PaymentDTOs;
using Dynamiq.Application.DTOs.ProductDTOs;
using Dynamiq.Domain.Aggregates;
using Dynamiq.Domain.Entities;
using Dynamiq.Domain.ValueObject;

namespace Dynamiq.Application
{
    public class MapperConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            return new MapperConfiguration(config =>
            {
                config.CreateMap<Product, ProductDto>().ReverseMap();
                config.CreateMap<Cart, CartDto>().ReverseMap();
                config.CreateMap<CartItem, CartItemDto>().ReverseMap();
                config.CreateMap<ProductImgUrl, ProductImgUrlDto>().ReverseMap();
                config.CreateMap<ProductParagraph, ProductParagraphDto>().ReverseMap();
                config.CreateMap<ResponseProducts, ResponseProductsDto>().ReverseMap();
                config.CreateMap<User, UserDto>()
                    .ForMember(dest => dest.PaymentHistories,
                        opt => opt.MapFrom(src => src.PaymentHistories))
                    .ForMember(dest => dest.Subscription,
                        opt => opt.MapFrom(src =>
                            src.Subscriptions
                                .Where(s => s.IsActive())
                                .Select(s => new SubscriptionDto(true))
                                .FirstOrDefault()
                                ?? new SubscriptionDto(false)))
                    .ForMember(dest => dest.EmailVerification,
                        opt => opt.MapFrom(src =>
                            new EmailVerificationDto(
                                src.EmailVerification.ExpiresAt.AddHours(-2),
                                src.EmailVerification.IsConfirmed)));
                config.CreateMap<PaymentHistory, PaymentHistoryDto>();
                config.CreateMap<ProductPaymentHistory, ProductPaymentHistoryDto>();
                config.CreateMap<Coupon, CouponDto>();
            });
        }
    }
}
