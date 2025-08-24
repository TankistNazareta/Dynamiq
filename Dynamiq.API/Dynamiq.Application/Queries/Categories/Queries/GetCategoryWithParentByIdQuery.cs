using MediatR;

namespace Dynamiq.Application.Queries.Categories.Queries
{
    /// <summary>
    /// The larger the index in the resulting list, the higher this category is positioned in the tree.
    /// </summary>
    public record class GetCategoryWithParentByIdQuery(Guid ChildId) : IRequest<List<string>>;
}
