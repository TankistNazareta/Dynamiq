using AutoMapper;
using Dynamiq.Application.DTOs.ProductDTOs;
using Dynamiq.Application.Queries.Categories.Queries;
using Dynamiq.Domain.Interfaces.Repositories;
using MediatR;

namespace Dynamiq.Application.Queries.Categories.Handlers
{
    public class GetAllCategoriesHandler : IRequestHandler<GetAllCategoriesQuery, List<CategoryDto>>
    {
        private readonly ICategoryRepo _categoryRepo;
        private readonly IMapper _mapper;

        public GetAllCategoriesHandler(ICategoryRepo categoryRepo, IMapper mapper)
        {
            _categoryRepo = categoryRepo;
            _mapper = mapper;
        }

        public async Task<List<CategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            var allCategories = await _categoryRepo.GetAllAsync(cancellationToken);

            var categoryDict = allCategories.ToDictionary(c => c.Id, 
                c => new CategoryDto(c.Id, c.Name, c.Slug, new()));

            List<CategoryDto> rootCategories = new();

            foreach (var category in allCategories)
            {
                if (category.ParentCategoryId is null)
                {
                    rootCategories.Add(categoryDict[category.Id]);
                }
                else if (categoryDict.TryGetValue(category.ParentCategoryId.Value, out var parentDto))
                {
                    parentDto.SubCategories.Add(categoryDict[category.Id]);
                }
            }

            return rootCategories;
        }
    }
}
