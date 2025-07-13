using AutoMapper;
using Dynamiq.API.DAL.Context;
using Dynamiq.API.DAL.Models;
using Dynamiq.API.Mapping.DTOs;
using Dynamiq.API.Interfaces;
using Microsoft.EntityFrameworkCore;
using Dynamiq.API.Repositories;

namespace Dynamiq.API.Repository
{
    public class ProductRepo : DefaultCrudRepo<ProductDto, Product>, IProductRepo
    {
        public ProductRepo(AppDbContext db, IMapper mapper) : base(db, mapper)
        {
        }

        protected override IQueryable<Product> Query()
        {
            return _db.Products
                .Include(p => p.PaymentHistories);
        }
    }
}
