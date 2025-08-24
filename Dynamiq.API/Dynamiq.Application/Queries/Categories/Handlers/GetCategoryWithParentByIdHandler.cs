using Dynamiq.Application.Interfaces.Repositories;
using Dynamiq.Application.Queries.Categories.Queries;
using MediatR;

namespace Dynamiq.Application.Queries.Categories.Handlers
{
    public class GetCategoryWithParentByIdHandler : IRequestHandler<GetCategoryWithParentByIdQuery, List<string>>
    {
        private readonly ICategoryRepo _repo;

        public GetCategoryWithParentByIdHandler(ICategoryRepo categoryRepo)
        {
            _repo = categoryRepo;
        }

        public async Task<List<string>> Handle(GetCategoryWithParentByIdQuery request, CancellationToken cancellationToken)
        {
            var arrayOfSlugs = new List<string>();

            var category = await _repo.GetByIdAsync(request.ChildId, cancellationToken);

            if (category == null)
                throw new KeyNotFoundException($"category with id: {request.ChildId} is not exist");

            arrayOfSlugs.Add(category.Slug);

            while (category.ParentCategoryId != null)
            {
                category = await _repo.GetByIdAsync(category.ParentCategoryId.Value, cancellationToken);
                arrayOfSlugs.Add(category.Slug);
            }

            return arrayOfSlugs;
        }
    }
}
