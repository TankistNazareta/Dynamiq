using Dynamiq.API.Extension.Interfaces;
using Dynamiq.API.Mapping.DTOs;

namespace Dynamiq.API.Interfaces
{
    public interface IEmailVerificationRepo : ICrudRepo<EmailVerificationDto>
    {
        Task<EmailVerificationDto> GetByToken(string token);
    }
}
