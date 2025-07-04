using Dynamiq.API.Mapping.DTOs;

namespace Dynamiq.API.Interfaces
{
    public interface IRefreshTokenRepo
    {
        Task Insert(RefreshTokenDto token);
        Task<RefreshTokenDto> GetByToken(string token);
        Task<RefreshTokenDto> GetByUserId(Guid userId);
        Task Revoke(string token);
        Task Update(RefreshTokenDto token);
    }
}
