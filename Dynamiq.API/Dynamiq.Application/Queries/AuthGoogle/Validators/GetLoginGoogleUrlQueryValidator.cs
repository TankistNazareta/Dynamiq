using Dynamiq.Application.Queries.AuthGoogle.Queries;
using FluentValidation;

namespace Dynamiq.Application.Queries.AuthGoogle.Validators
{
    public class GetLoginGoogleUrlQueryValidator : AbstractValidator<GetLoginGoogleUrlQuery>
    {
        public GetLoginGoogleUrlQueryValidator()
        {
            RuleFor(x => x.returnUrl)
                .MaximumLength(2048).WithMessage("ReturnUrl is too long.")
                .Matches(@"^https?://.+").When(x => !string.IsNullOrWhiteSpace(x.returnUrl))
                .WithMessage("ReturnUrl must be a valid URL starting with http:// or https://");
        }
    }
}
