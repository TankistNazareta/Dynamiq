using Dynamiq.API.Extension.DTOs;

namespace Dynamiq.API.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task Insert(RefreshTokenDto token);
        Task<RefreshTokenDto> GetByToken(string token);
        Task<RefreshTokenDto> GetByUserId(Guid userId);
        Task Revoke(string token);
        Task Update(RefreshTokenDto token);
    }
}
