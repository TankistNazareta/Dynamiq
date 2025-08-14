using MediatR;

namespace Dynamiq.Application.Queries.AuthGoogle.Queries
{
    public record class GetLoginGoogleUrlQuery(string? returnUrl = null) : IRequest<string>;
}
