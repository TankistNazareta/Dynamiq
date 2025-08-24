using Dynamiq.Application.DTOs.ProductDTOs;
using Dynamiq.Domain.Common;
using MediatR;

namespace Dynamiq.Application.Queries.Products.Queries
{
    public record class GetFilteredProductsQuery(ProductFilter Filter, int Limit, int Offset) : IRequest<ResponseProductsDto>;
}
