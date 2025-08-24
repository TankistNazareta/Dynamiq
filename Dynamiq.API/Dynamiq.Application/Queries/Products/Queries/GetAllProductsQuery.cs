using Dynamiq.Application.DTOs.ProductDTOs;
using MediatR;

namespace Dynamiq.Application.Queries.Products.Queries
{
    public record class GetAllProductsQuery(int Limit, int Offset) : IRequest<ResponseProductsDto>;
}
