using AutoMapper;
using Dynamiq.API.DAL.Context;
using Dynamiq.API.DAL.Models;
using Dynamiq.API.Mapping.DTOs;
using Dynamiq.API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dynamiq.API.Repository
{
    public class ProductRepo : IProductRepo
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public ProductRepo(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task Delete(Guid id)
        {
            var model = await _db.Products.FirstOrDefaultAsync(x => x.Id == id);

            if (model == null)
                throw new KeyNotFoundException($"Product with the id: {id} does not exist");

            _db.Products.Remove(model);

            await _db.SaveChangesAsync();
        }

        public async Task<List<ProductDto>> GetAll()
        {
            var models = await _db.Products
                .Include(p => p.PaymentHistories)
                .ToListAsync();

            return _mapper.Map<List<ProductDto>>(models);
        }

        public async Task<ProductDto> GetById(Guid id)
        {
            var model = await _db.Products.FirstOrDefaultAsync(x => x.Id == id);

            if (model == null)
                throw new KeyNotFoundException($"product with the id: {id} does not exist");

            return _mapper.Map<ProductDto>(model);
        }

        public async Task<ProductDto> Insert(ProductDto entity)
        {
            var model = _mapper.Map<Product>(entity);

            _db.Products.Add(model);

            await _db.SaveChangesAsync();

            return _mapper.Map<ProductDto>(model);
        }

        public async Task<ProductDto> Update(ProductDto entity)
        {
            var newDataProduct = _mapper.Map<Product>(entity);

            _db.Products.Update(newDataProduct);

            await _db.SaveChangesAsync();

            return _mapper.Map<ProductDto>(newDataProduct);
        }
    }
}
