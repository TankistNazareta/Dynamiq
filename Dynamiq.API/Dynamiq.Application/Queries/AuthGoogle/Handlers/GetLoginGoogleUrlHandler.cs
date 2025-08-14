using Dynamiq.Application.Interfaces.Auth;
using Dynamiq.Application.Queries.AuthGoogle.Queries;
using MediatR;

namespace Dynamiq.Application.Queries.AuthGoogle.Handlers
{
    public class GetLoginGoogleUrlHandler : IRequestHandler<GetLoginGoogleUrlQuery, string>
    {
        private readonly IGoogleOidcService _google;

        public GetLoginGoogleUrlHandler(IGoogleOidcService google)
        {
            _google = google;
        }

        public Task<string> Handle(GetLoginGoogleUrlQuery request, CancellationToken cancellationToken)
        {
            var url = _google.GetLoginGoogleUrlHandler();
            return Task.FromResult(url);
        }
    }
}
