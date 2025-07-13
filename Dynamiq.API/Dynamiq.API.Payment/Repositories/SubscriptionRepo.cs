using AutoMapper;
using Dynamiq.API.DAL.Context;
using Dynamiq.API.DAL.Models;
using Dynamiq.API.Mapping.DTOs;
using Dynamiq.API.Stripe.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dynamiq.API.Stripe.Repositories
{
    public class SubscriptionRepo : ISubscriptionRepo
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _db;

        public SubscriptionRepo(IMapper mapper, AppDbContext db)
        {
            _mapper = mapper;
            _db = db;
        }

        public async Task Delete(Guid id)
        {
            var model = await _db.Subscriptions.FirstOrDefaultAsync(x => x.Id == id);

            if (model == null)
                throw new KeyNotFoundException($"Subscription with the id: {id} wasn't found");

            _db.Subscriptions.Remove(model);

            await _db.SaveChangesAsync();
        }

        public async Task<List<SubscriptionDto>> GetAll()
        {
            var models = await _db.Subscriptions.ToListAsync();

            return _mapper.Map<List<SubscriptionDto>>(models);
        }

        public async Task<SubscriptionDto> GetById(Guid id)
        {
            var model = await _db.Subscriptions.FirstOrDefaultAsync(x => x.Id == id);

            if (model == null)
                throw new KeyNotFoundException($"Subscription with the id: {id} wasn't found");

            return _mapper.Map<SubscriptionDto>(model);
        }

        public async Task<bool> HasActiveSubscription(Guid userId)
        {
            var models = await _db.Subscriptions
                .Where(s => s.UserId == userId)
                .Where(s => s.IsActive && s.EndDate > DateTime.UtcNow)
                .ToListAsync();

            if (models.Count == 0)
                return false;

            return true;
        }

        public async Task<SubscriptionDto> Insert(SubscriptionDto entity)
        {
            var model = _mapper.Map<Subscription>(entity);

            _db.Subscriptions.Add(model);

            await _db.SaveChangesAsync();

            return _mapper.Map<SubscriptionDto>(model);
        }

        public async Task<SubscriptionDto> Update(SubscriptionDto entity)
        {
            var newModel = _mapper.Map<Subscription>(entity);

            _db.Subscriptions.Update(newModel);

            await _db.SaveChangesAsync();

            return _mapper.Map<SubscriptionDto>(newModel);
        }
    }
}
