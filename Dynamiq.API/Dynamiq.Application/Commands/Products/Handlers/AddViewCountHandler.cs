using Dynamiq.Application.Commands.Products.Commands;
using Dynamiq.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace Dynamiq.Application.Commands.Products.Handlers
{
    public class AddViewCountHandler : IRequestHandler<AddViewCountCommand>
    {
        private readonly IProductRepo _repo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _cache;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AddViewCountHandler(
            IProductRepo repo,
            IUnitOfWork unitOfWork,
            IMemoryCache cache,
            IHttpContextAccessor httpContextAccessor)
        {
            _repo = repo;
            _unitOfWork = unitOfWork;
            _cache = cache;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task Handle(AddViewCountCommand request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
            if (userId == null)
                return;

            var cacheKey = $"product-view:{userId}:{request.Id}";
            if (_cache.TryGetValue(cacheKey, out _))
                return;

            var product = await _repo.GetByIdAsync(request.Id, cancellationToken);
            if (product == null)
                throw new KeyNotFoundException("Product with this id wasn't found");

            product.AddSearch();
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _cache.Set(cacheKey, true, TimeSpan.FromMinutes(30));
        }
    }
}
