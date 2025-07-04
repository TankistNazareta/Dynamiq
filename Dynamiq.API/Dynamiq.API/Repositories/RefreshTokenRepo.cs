using AutoMapper;
using Dynamiq.API.DAL.Context;
using Dynamiq.API.DAL.Models;
using Dynamiq.API.Interfaces;
using Dynamiq.API.Mapping.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Dynamiq.API.Repositories
{
    public class RefreshTokenRepo : IRefreshTokenRepo
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public RefreshTokenRepo(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }  

        public async Task<RefreshTokenDto> GetByToken(string token)
        {
            var rt = await _db.RefreshTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Token == token);

            if (rt == null)
                throw new ArgumentException("Token isn't correct");

            return _mapper.Map<RefreshTokenDto>(rt);
        }

        public async Task<RefreshTokenDto> GetByUserId(Guid userId)
        {
            var rt = await _db.RefreshTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.UserId == userId);

            if (rt == null)
                throw new ArgumentException("userId isn't correct");

            return _mapper.Map<RefreshTokenDto>(rt);
        }

        public async Task Insert(RefreshTokenDto token)
        {
            var model = _mapper.Map<RefreshToken>(token);

            _db.RefreshTokens.Add(model);

            await _db.SaveChangesAsync();
        }

        public async Task Revoke(string token)
        {
            var rt = await _db.RefreshTokens.FirstOrDefaultAsync(t => t.Token == token);

            if (rt == null)
                throw new ArgumentException("Token isn't correct");
            
            rt.IsRevoked = true;
            await _db.SaveChangesAsync(); 
        }

        public async Task Update(RefreshTokenDto token)
        {
            var model = _mapper.Map<RefreshToken>(token);

            _db.RefreshTokens.Update(model);
            await _db.SaveChangesAsync();
        }
    }
}
