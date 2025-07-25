using Dynamiq.Application.DTOs;
using MediatR;

namespace Dynamiq.Application.Queries.Categories.Queries
{
    public record class GetAllCategoriesQuery : IRequest<List<CategoryDto>>;
}
