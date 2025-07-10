using Dynamiq.API.Extension.Interfaces;
using Dynamiq.API.Mapping.DTOs;

namespace Dynamiq.API.Interfaces
{
    public interface IEmailVerificationRepo : ICRUD<EmailVerificationDto>
    {
        Task ConfirmEmail(Guid id);
        Task<EmailVerificationDto> GetByToken(string token);
    }
}
