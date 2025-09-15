using Dynamiq.Domain.Common;
using MediatR;

namespace Dynamiq.Application.Queries.Products.Queries
{
    public record class GetSearchedProductQuery(ProductFilter ProductFilter, int Limit) : IRequest<List<string>>;
}
