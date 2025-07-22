using Dynamiq.Application.Commands.Payment.Commands;
using Dynamiq.Application.DTOs;
using Dynamiq.Application.Interfaces.Stripe;
using Dynamiq.Domain.Interfaces.Repositories;
using MediatR;

namespace Dynamiq.Application.Commands.Payment.Handlers
{
    public class CreateCheckoutSessionHandler : IRequestHandler<CreateCheckoutSessionCommand, string>
    {
        private readonly IStripePaymentService _paymentService;
        private readonly IProductRepo _productRepo;

        public CreateCheckoutSessionHandler(IStripePaymentService paymentService, IProductRepo productRepo)
        {
            _productRepo = productRepo;
            _paymentService = paymentService;
        }

        public async Task<string> Handle(CreateCheckoutSessionCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepo.GetByIdAsync(request.ProductId, cancellationToken);

            if (product == null)
                throw new KeyNotFoundException($"Product with id {request.ProductId} doesn't exist");

            var checkoutSessionRequest = new CheckoutSessionDto()
            {
                ProductId = product.Id,
                UserId = request.UserId,
                Quantity = request.Quantity,
                SuccessUrl = request.SuccessUrl,
                CancelUrl = request.CancelUrl,
                Amount = product.Price,
                Interval = product.Interval,
                StripePriceId = product.StripePriceId
            };

            var url = await _paymentService.CreateCheckoutSessionAsync(checkoutSessionRequest);

            return url;
        }
    }
}
