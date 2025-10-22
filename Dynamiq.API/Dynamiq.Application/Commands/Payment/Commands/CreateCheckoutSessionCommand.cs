using MediatR;

namespace Dynamiq.Application.Commands.Payment.Commands
{
    public class CreateCheckoutSessionCommand : IRequest<string>
    {
        public Guid UserId { get; set; }
        public string SuccessUrl { get; set; }
        public string CancelUrl { get; set; }
        public int? Quantity { get; set; }
        public Guid? ProductId { get; set; }
        public List<string>? CouponCodes { get; set; }
        public Guid? SubscriptionId { get; set; }
    }
}
