namespace Dynamiq.Application.DTOs.PaymentDTOs
{
    public record class CouponsResultDto(List<string>? CouponsCodeList, List<string>? StripeCouponIdList);
}
