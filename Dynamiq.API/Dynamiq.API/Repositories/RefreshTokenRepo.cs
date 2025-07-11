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
                throw new KeyNotFoundException($"Refresh Token with the token: {token} wasn't found");

            return _mapper.Map<RefreshTokenDto>(rt);
        }

        public async Task<RefreshTokenDto> GetByUserId(Guid id)
        {
            var rt = await _db.RefreshTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.UserId == id);

            if (rt == null)
                throw new KeyNotFoundException($"Refresh Token with the id: {id} wasn't found");

            return _mapper.Map<RefreshTokenDto>(rt);
        }

        public async Task<RefreshTokenDto> Insert(RefreshTokenDto token)
        {
            var model = _mapper.Map<RefreshToken>(token);

            _db.RefreshTokens.Add(model);

            await _db.SaveChangesAsync();

            return _mapper.Map<RefreshTokenDto>(model);
        }

        public async Task Revoke(string token)
        {
            var rt = await _db.RefreshTokens.FirstOrDefaultAsync(t => t.Token == token);

            if (rt == null) 
                throw new KeyNotFoundException($"Refresh Token with the token: {token} wasn't found");

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
