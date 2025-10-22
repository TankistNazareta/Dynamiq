using Dynamiq.Application.DTOs.AccountDTOs;
using MediatR;

namespace Dynamiq.Application.Queries.Subscriptions.Queries
{
    public record class GetAllSubscriptionsQuery : IRequest<List<SubscriptionDto>>;
}
