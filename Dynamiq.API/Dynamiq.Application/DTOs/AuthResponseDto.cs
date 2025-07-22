using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynamiq.Application.DTOs
{
    public record class AuthResponseDto(string AccessToken, string RefreshToken);
}
