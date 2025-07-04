using AutoMapper;
using Dynamiq.API.DAL.Context;
using Dynamiq.API.DAL.Models;
using Dynamiq.API.Mapping.DTOs;
using Dynamiq.API.Stripe.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dynamiq.API.Stripe.Repositories
{
    public class PaymentHistoryRepo : IPaymentHistoryRepo
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public PaymentHistoryRepo(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task Delete(Guid id)
        {
            var model = await _db.PaymentHistories.FirstOrDefaultAsync(x => x.Id == id);

            if (model == null)
                throw new ArgumentException($"{nameof(id)} does not exist");

            _db.PaymentHistories.Remove(model);

            await _db.SaveChangesAsync();
        }

        public async Task<List<PaymentHistoryDto>> GetAll()
        {
            var models = await _db.PaymentHistories.ToListAsync();

            return _mapper.Map<List<PaymentHistoryDto>>(models);
        }

        public async Task<PaymentHistoryDto> GetById(Guid id)
        {
            var model = await _db.PaymentHistories.FirstOrDefaultAsync(p => p.Id == id);

            return _mapper.Map<PaymentHistoryDto>(model);
        }

        public async Task<PaymentHistoryDto> Insert(PaymentHistoryDto paymentHistory)
        {
            var model = _mapper.Map<PaymentHistory>(paymentHistory);

            _db.PaymentHistories.Add(model);

            await _db.SaveChangesAsync();

            return _mapper.Map<PaymentHistoryDto>(model);
        }
    }
}
